using Cinema.DataAccess.Repository.Interfaces;
using Cinema.Helpers;
using Cinema.Models;
using Cinema.Models.Enums;
using Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using static Cinema.Constants.Areas;
using static Cinema.Constants.Messages;

namespace Cinema.Areas.Customer.Controllers
{
	[Area(CUSTOMER)]
	public sealed class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		private readonly IUnitOfWork _unitOfWork;

		public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
		{
			_logger = logger;
			_unitOfWork = unitOfWork;
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

			if (viewModel.ChosenPayment is Payment.Residual)
			{
				var totalCost = tickets.Sum(t => t.Cost);
				if (totalCost > user.Credit)
				{
					ViewData["error"] = RESIDUAL_NOT_SUFFICIENT;
					return View(viewModel);
				}

				user.Credit -= totalCost;
				_unitOfWork.ApplicationUsers.Update(user);
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

			tickets.ForEach(t =>
			{
				t.UserId = identity!.Value;
				_unitOfWork.Tickets.Update(t);
			});
			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
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
	}
}