using Cinema.DataAccess.Repository.Interfaces;
using Cinema.Helpers;
using Cinema.Models;
using Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Cinema.Constants.Areas;
using static Cinema.Constants.Messages;

namespace Cinema.Areas.Customer.Controllers
{
	[Area(CUSTOMER)]
	[Authorize]
	public sealed class UserController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public UserController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			if (!IdentityHelpers.TryGetUserIdentity(User, out var claim))
				return NotFound();

			var tickets = _unitOfWork.Tickets.GetAll(t => t.UserId == claim!.Value, "Show");
			var joinedFilms = tickets.Join(
				_unitOfWork.Films.GetAll(),
				t => t.Show.FilmId,
				f => f.FilmId,
				(t, f) =>
				{
					t.Show.Film = f;
					return t;
				}
			);

			var nextHour = DateTime.Now.AddHours(1);
			var refundable = new List<Ticket>();
			var nonRefundable = new List<Ticket>();

			foreach (var ticket in joinedFilms)
			{
				if (ticket.Show.Time > nextHour)
					refundable.Add(ticket);
				else
					nonRefundable.Add(ticket);
			}

			var filmsWatched =
				from film in nonRefundable.Select(t => t.Show.Film).Distinct()
				join rating in _unitOfWork.Reviews.GetAll(r => r.UserId == claim!.Value) on film.FilmId equals rating.FilmId into fr
				from ratedFilms in fr.DefaultIfEmpty()
				select new UserReviewViewModel()
				{
					Film = film,
					Rating = ratedFilms?.Rating ?? 0
				};

			return View(new UserHistoryViewModel()
			{
				RefundableTickets = refundable,
				NonRefundableTickets = nonRefundable,
				FilmsWatched = filmsWatched,
			});
		}

		public IActionResult Update(int? showId, char? lane, int? number)
		{
			if (
				showId is null || lane is null || number is null ||
				showId is 0 || lane is '\0' || number is 0 ||
				!IdentityHelpers.TryGetUserIdentity(User, out var identity)
				)
				return NotFound();

			var ticket = _unitOfWork.Tickets.GetFirstOrDefault(t =>
				t.UserId == identity!.Value &&
				t.ShowId == showId &&
				t.Lane == lane &&
				t.Number == number
				);

			if (ticket is null)
				return NotFound();

			return View(new EditTicketViewModel()
			{
				NewTicket = ticket,
				OldTicket = new TicketInfo()
				{
					Lane = ticket.Lane,
					Number = ticket.Number,
					Cost = ticket.Cost,
				}
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Update(EditTicketViewModel ticket)
		{
			if (!ModelState.IsValid)
				return View(ticket);

			IdentityHelpers.TryGetUserIdentity(User, out var identity);
			if (ticket.NewTicket.UserId == identity!.Value &&
				_unitOfWork.ApplicationUsers.GetFirstOrDefault(u => u.Id == identity.Value) is User user)
			{
				var oldTicket = _unitOfWork.Tickets.GetFirstOrDefault(t =>
					t.Number == ticket.OldTicket.Number &&
					t.Lane == ticket.OldTicket.Lane &&
					t.ShowId == ticket.NewTicket.ShowId);

				var difference = ticket.OldTicket.Cost - ticket.NewTicket.Cost;
				if (difference < user.Credit || true /*TODO: Remove this true*/)
					user.Credit -= difference;
				else
				{
					// TODO: Require payment
				}

				if (oldTicket is not null)
				{
					oldTicket.UserId = null;
					_unitOfWork.Tickets.Update(oldTicket);
					_unitOfWork.Tickets.Update(ticket.NewTicket);

					TempData["success"] = EDIT_SUCCESS;
					_unitOfWork.Save();

					return RedirectToAction(nameof(Index));
				}
			}

			TempData["error"] = EDIT_FAIL;
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public IActionResult Delete(int? showId, char? lane, int? number)
		{
			if (
				showId is null || lane is null || number is null ||
				showId is 0 || lane is '\0' || number is 0 ||
				!IdentityHelpers.TryGetUserIdentity(User, out var identity)
				)
				return NotFound();

			var ticket = _unitOfWork.Tickets.GetFirstOrDefault(t =>
				t.UserId == identity!.Value &&
				t.ShowId == showId &&
				t.Lane == lane &&
				t.Number == number
				);

			var user = _unitOfWork.ApplicationUsers.GetFirstOrDefault(u => u.Id == identity!.Value);

			if (ticket is null || user is null)
				return NotFound();

			ticket.UserId = null;
			user.Credit += ticket.Cost;

			_unitOfWork.Tickets.Update(ticket);
			_unitOfWork.ApplicationUsers.Update(user);
			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		public IActionResult SaveReview(int? filmId, int? rating)
		{
			if (
				filmId is null || filmId is 0 ||
				rating < 1 || rating > 5 ||
				!IdentityHelpers.TryGetUserIdentity(User, out var identity)
				)
				return Json(new { Success = false });

			var reviewInDb = _unitOfWork.Reviews.GetFirstOrDefault(r => r.FilmId == filmId && r.UserId == identity!.Value);

			if (reviewInDb is null)
			{
				_unitOfWork.Reviews.Add(new Review()
				{
					FilmId = (int)filmId,
					UserId = identity!.Value,
					Rating = (byte)rating!
				});
			}
			else
			{
				reviewInDb.Rating = (byte)rating!;
				_unitOfWork.Reviews.Update(reviewInDb);
			}

			_unitOfWork.Save();
			return Json(new { Success = true });
		}
	}
}
