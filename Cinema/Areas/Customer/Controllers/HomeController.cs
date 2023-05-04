using Cinema.Constants;
using Cinema.DataAccess.Repository.Interfaces;
using Cinema.Helpers;
using Cinema.Models;
using Cinema.Models.Enums;
using Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Stripe.Checkout;
using System.Diagnostics;
using System.Text;
using static Cinema.Constants.Areas;
using static Cinema.Constants.Messages;

namespace Cinema.Areas.Customer.Controllers
{
	[Area(CUSTOMER)]
	public sealed class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		private readonly IUnitOfWork _unitOfWork;

		private readonly IEmailSender _emailSender;

		public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IEmailSender emailSender)
		{
			_logger = logger;
			_unitOfWork = unitOfWork;
			_emailSender = emailSender;
		}

		public IActionResult Index()
		{
			var ratings = _unitOfWork.Reviews
				.GetAll()
				.GroupBy(r => r.FilmId)
				.Select(g => new { FilmId = g.Key, Rating = g.Average(r => r.Rating) });

			var nextWeek = DateTime.Today.AddDays(14);
			var filmIds = _unitOfWork.Shows
				.GetAll(s => s.Time >= DateTime.Today && s.Time < nextWeek)
				.Select(s => s.FilmId)
				.Distinct();

			var filmsList =
				from film in _unitOfWork.Films.GetAll(f => filmIds.Any(id => id == f.FilmId))
				join rating in ratings on film.FilmId equals rating.FilmId into fr
				from ratedFilms in fr.DefaultIfEmpty()
				select new FilmDisplayViewModel()
				{
					Film = film,
					Rating = ratedFilms?.Rating ?? 0
				};

			var genres = EnumHelpers.GetFilmGenres();

			return View(new HomeViewModel()
			{
				FilmDisplays = filmsList,
				Genres = genres,
			});
		}

		public IActionResult Details(int? filmId)
		{
			if (filmId is null || filmId is 0 || _unitOfWork.Films.GetFirstOrDefault(f => f.FilmId == filmId) is not Film film)
				return RedirectToAction(nameof(Index));

			var ratings = _unitOfWork.Reviews.GetAll(r => r.FilmId == filmId);
			var avgRating = ratings.Any() ? ratings.Average(r => r.Rating) : 1;
			var tickets = _unitOfWork.Tickets.GetAll(t => t.UserId == null).GroupBy(t => t.ShowId);
			var shows = _unitOfWork.Shows.GetAll(s => s.FilmId == filmId && s.Time >= DateTime.Today).ToDictionary(s => s.ShowId);
			var rooms = shows.Values.DistinctBy(s => s.RoomId);

			foreach (var group in tickets)
			{
				if (shows.ContainsKey(group.Key))
					shows[group.Key].Tickets = group.Select(g => g);
			}

			var viewModel = new FilmDetailsViewModel()
			{
				Film = film,
				Rating = avgRating,
				Shows = shows.Values,
				Rooms = rooms.Select(r => new SelectListItem(r.RoomId.ToString(), r.RoomId.ToString()))
			};

			return View(viewModel);
		}

		[Authorize]
		public IActionResult Buy(FilmDetailsViewModel viewModel)
		{
			var show = _unitOfWork.Shows.GetFirstOrDefault(s => s.ShowId == viewModel.ShowId, "Room");
			if (show is null)
				return NotFound();

			var buyViewModel = new BuyTicketsViewModel()
			{
				ShowId = show.ShowId,
				Capacity = show.Room.Seats,
				FilmTitle = viewModel.Film.Title,
				Date = show.Time,
				PaymentMethods = EnumHelpers.GetPayments(),
				Tickets = new List<TicketInfo>() { new(), new(), new(), new() }
			};

			return View(buyViewModel);
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Buy(BuyTicketsViewModel viewModel)
		{
			if (!IdentityHelpers.TryGetUserIdentity(User, out var identity))
				return Unauthorized();

			var tickets = new List<Ticket>();
			foreach (var ticket in viewModel.Tickets)
			{
				if (ticket.Lane is not '\0' &&
					ticket.Number is not 0 &&
					viewModel.ShowId is not 0)
				{
					var ticketInDb = _unitOfWork.Tickets.GetFirstOrDefault(t =>
							t.Number == ticket.Number &&
							t.Lane == ticket.Lane &&
							t.ShowId == viewModel.ShowId &&
							t.UserId == null);

					if (ticketInDb is null)
					{
						ViewData["error"] = TICKETS_NO_AVAILABLE;
						return View(viewModel);
					}

					tickets.Add(ticketInDb);
				}
			}

			var user = _unitOfWork.ApplicationUsers.GetFirstOrDefault(u => u.Id == identity!.Value);

			if (user is null)
				return Unauthorized();

			var totalCost = tickets.Sum(t => t.Cost);

			if (viewModel.ChosenPayment is Payment.Residual)
			{
				if (totalCost > user.Credit)
				{
					ViewData["error"] = RESIDUAL_NOT_SUFFICIENT;
					return View(viewModel);
				}

				user.Credit -= totalCost;
				_unitOfWork.ApplicationUsers.Update(user);

				SendConfirmationEmail(user, viewModel.ShowId, tickets);

				return RedirectToAction(nameof(Index));
			}
			else if (!CreditCardHelpers.IsCreditCardValid(
				viewModel.NameOnCard,
				viewModel.CardNumber,
				viewModel.Expire,
				viewModel.CCV,
				out var message))
			{
				ViewData["error"] = message;
				return View(viewModel);
			}

			var order = new Order()
			{
				UserId = user.Id,
				OrderTotal = totalCost
			};

			_unitOfWork.Orders.Add(order);
			_unitOfWork.Save();

			tickets.ForEach(t =>
			{
				t.UserId = identity!.Value;
				_unitOfWork.Tickets.Update(t);
				_unitOfWork.PendingTickets.Add(new()
				{
					OrderId = order.Id,
					Lane = t.Lane,
					Number = t.Number,
					ShowId = t.ShowId,
				});
			});
			_unitOfWork.Save();

			var filmName = _unitOfWork.Shows.GetFirstOrDefault(s => s.ShowId == viewModel.ShowId, "Film")?.Film.Title ?? string.Empty;

			return ExecuteStripePayment(tickets, filmName, order.Id);
		}

		public IActionResult OrderConfirmation(int orderId)
		{
			var order = _unitOfWork.Orders.GetFirstOrDefault(o => o.Id == orderId);
			IEnumerable<PendingTicket> pending = _unitOfWork.PendingTickets.GetAll(pt => pt.OrderId == orderId);

			if (order is null)
			{
				FreeTickets(pending);
				return RedirectToAction(nameof(Index));
			}

			var service = new SessionService();
			var session = service.Get(order.SessionId);

			if (session.PaymentStatus.ToLower() is not "paid")
			{
				FreeTickets(pending);
				return RedirectToAction(nameof(Index));
			}

			_unitOfWork.Orders.UpdateStatus(orderId, OrderStatus.APPROVED, PaymentStatus.APPROVED);
			_unitOfWork.Orders.UpdateStripePaymentIntentId(orderId, session.PaymentIntentId);
			_unitOfWork.PendingTickets.RemoveRange(pending);
			_unitOfWork.Save();

			var user = _unitOfWork.ApplicationUsers.GetFirstOrDefault(u => u.Id == order.UserId);
			SendConfirmationEmail(user!, pending.ElementAt(0).ShowId, pending: pending);

			return View(orderId);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		private StatusCodeResult ExecuteStripePayment(IEnumerable<Ticket> tickets, string filmTitle, int orderId)
		{
			var host = HttpContext.Request.Host.Value;
			var scheme = HttpContext.Request.Scheme;
			var domain = scheme + Uri.SchemeDelimiter + host + "/";

			var options = new SessionCreateOptions
			{
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
				SuccessUrl = domain + $"Customer/Home/OrderConfirmation?id={orderId}",
				CancelUrl = domain + $"Customer/Home/Index",
			};

			foreach (var item in tickets)
			{
				var sessionLineItemOptions = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Cost * 100),
						Currency = "eur",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = $"{filmTitle} - {item.Lane}{item.Number}",
							Description = "Biglietto del cinema"
						},
					},
					Quantity = 1
				};

				options.LineItems.Add(sessionLineItemOptions);
			}

			var service = new SessionService();
			var session = service.Create(options);

			_unitOfWork.Orders.UpdateStripeSessionId(orderId, session.Id);
			_unitOfWork.Save();

			Response.Headers.Add("Location", session.Url);

			return new StatusCodeResult(303);
		}

		private void FreeTickets(IEnumerable<PendingTicket> tickets)
		{
			foreach (var ticket in tickets)
			{
				var matchingTicket = _unitOfWork.Tickets
					.GetFirstOrDefault(t =>
						t.Lane == ticket.Lane &&
						t.Number == ticket.Number &&
						t.ShowId == ticket.ShowId);

				if (matchingTicket != null)
				{
					matchingTicket.UserId = null;
					_unitOfWork.Tickets.Update(matchingTicket);
				}
			}

			_unitOfWork.PendingTickets.RemoveRange(tickets);
			_unitOfWork.Save();
		}

		private void SendConfirmationEmail(User user, int showId, IEnumerable<Ticket>? tickets = null, IEnumerable<PendingTicket>? pending = null)
		{
			if (user.Email is not null && user.EmailConfirmed)
			{
				var show = _unitOfWork.Shows.GetFirstOrDefault(s => s.ShowId == showId, "Film");
				var content = new StringBuilder("<p>Il tuo acquisto è avvenuto con successo!<br/>Ecco il riepilogo dell'ordine:<br/>" +
					$"Film: {show?.Film.Title ?? string.Empty}<br/>" +
					$"Ora: {show?.Time ?? DateTime.Now}<br/>Sala: {show?.RoomId ?? 1}<br/><ul>");

				if (tickets is not null)
					foreach (var t in tickets)
						content.Append($"<li>Posto: {t.Lane}{t.Number}</li>");
				else if (pending is not null)
					foreach (var t in pending)
						content.Append($"<li>Posto: {t.Lane}{t.Number}</li>");

				content.Append("</ul></p>");

				_emailSender.SendEmailAsync(
					user.Email,
					"Acquisto confermato - Cinema Ferrario",
					content.ToString());
			}
		}
	}
}