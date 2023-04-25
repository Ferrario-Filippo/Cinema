using Cinema.DataAccess.Repository.Interfaces;
using Cinema.Models;
using Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using System.Security.Claims;
using static Cinema.Constants.Areas;
using static Cinema.Constants.Messages;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
			if (!TryGetUserIdentity(out var claim))
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

			var filmsWatched = nonRefundable.Select(t => t.Show.Film).Distinct();

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
				!TryGetUserIdentity(out var identity)
				)
				return NotFound();

			var ticket = _unitOfWork.Tickets.GetFirstOrDefault(t =>
				t.UserId == identity!.Value &&
				t.ShowId == showId &&
				t.Lane == lane &&
				t.Number == number
				);

			return View(ticket);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Update(Ticket ticket)
		{
			if (!ModelState.IsValid)
				return View(ticket);

			TryGetUserIdentity(out var identity);
			if (ticket.UserId == identity!.Value)
			{
				_unitOfWork.Tickets.Update(ticket);
				TempData["success"] = EDIT_SUCCESS;
				_unitOfWork.Save();
			}
			else
			{
				TempData["success"] = EDIT_FAIL;
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public IActionResult Delete(int? showId, char? lane, int? number)
		{
			if (
				showId is null || lane is null || number is null ||
				showId is 0 || lane is '\0' || number is 0 ||
				!TryGetUserIdentity(out var identity)
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

			ticket.UserId = null;

			_unitOfWork.Tickets.Update(ticket);
			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		// APIs
		public IActionResult GetAvailableSeats(int? showId)
		{
			if (showId is null || showId is 0)
				return Json(new { Message = "Non trovato" });

			var seats = _unitOfWork.Tickets
				.GetAll(t => t.ShowId == showId && t.UserId == null)
				.Select(t => new { t.Lane, t.Number});

			return Json(new { Data = seats });
		}

		// Local helpers
		private bool TryGetUserIdentity(out Claim? identity)
		{
			identity = null;

			var userIdentity = User.Identity;
			if (userIdentity is null)
				return false;

			var claimsIdentity = (ClaimsIdentity)userIdentity;
			identity = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

			return identity is not null;
		}
	}
}
