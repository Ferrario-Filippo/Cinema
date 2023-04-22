using Cinema.DataAccess.Repository.Interfaces;
using Cinema.Models;
using Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Cinema.Constants.Areas;

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
			var userIdentity = User.Identity;
			if (userIdentity is null)
				return NotFound();

			var claimsIdentity = (ClaimsIdentity)userIdentity;
			var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

			if (claim is null)
				return NotFound();

			var tickets = _unitOfWork.Tickets.GetAll(t => t.UserId == claim.Value, "Show");
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
	}
}
