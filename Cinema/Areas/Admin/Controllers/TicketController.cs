using Cinema.DataAccess.Repository.Interfaces;
using Cinema.Models;
using Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Cinema.Constants.Areas;
using static Cinema.Constants.Messages;
using static Cinema.Constants.Roles;

namespace Cinema.Areas.Admin.Controllers
{
	[Area(ADMIN)]
	public sealed class TicketController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public TicketController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[Authorize(Roles = ROLE_ADMIN)]
		public IActionResult Index()
		{
			return View();
		}

		[Authorize(Roles = ROLE_ADMIN)]
		public IActionResult Upsert(int? showId, int? number, char? lane)
		{
			var ticketViewModel = new TicketViewModel()
			{
				Shows = _unitOfWork.Shows
					.GetAll(includeProperties: "Film,Room")
					.Select(s =>
						new SelectListItem($"{s.Film.Title} {s.Room.RoomId} {s.Time}", showId.ToString())
					)
			};

			if (
				showId is not null && showId is not 0 &&
				number is not null && number is not 0 &&
				lane is not null && lane is not '\0'
				)
				ticketViewModel.Ticket = _unitOfWork.Tickets.GetFirstOrDefault(t => 
					t.ShowId == showId && t.Number == number && t.Lane == lane)!;

			ticketViewModel.Ticket ??= new();

			return View(ticketViewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ROLE_ADMIN)]
		public IActionResult Upsert(TicketViewModel ticketViewModel)
		{
			if (!ModelState.IsValid)
				return View(ticketViewModel);

			if (ticketViewModel.Ticket.ShowId is 0 || 
				ticketViewModel.Ticket.Number is 0 || 
				ticketViewModel.Ticket.Lane is '\0')
			{
				_unitOfWork.Tickets.Add(ticketViewModel.Ticket);
				TempData["success"] = CREATE_SUCCESS;
			}
			else
			{
				_unitOfWork.Tickets.Update(ticketViewModel.Ticket);
				TempData["success"] = EDIT_SUCCESS;
			}

			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		// APIs
		[HttpGet]
		public IActionResult GetAll()
		{
			var ticketsList = _unitOfWork.Tickets.GetAll();
			return Json(new { data = ticketsList });
		}

		[HttpDelete]
		[Authorize(Roles = ROLE_ADMIN)]
		public IActionResult Delete(int? showId, int? number, char? lane)
		{
			if (
				showId is null || showId is 0 ||
				number is null || number is 0 ||
				lane is null || lane is '\0'
				)
				return NotFound();

			if (
				_unitOfWork.Tickets.GetFirstOrDefault(t =>
					t.ShowId == showId && t.Number == number && t.Lane == lane
				) is not Ticket ticket
				)
				return Json(new { success = false, message = DELETE_FAIL });

			_unitOfWork.Tickets.Remove(ticket);
			_unitOfWork.Save();

			return Json(new { success = true, message = DELETE_SUCCESS });
		}
	}
}
