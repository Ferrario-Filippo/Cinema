using Cinema.DataAccess.Repository.Interfaces;
using Cinema.Helpers;
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
	public sealed class ShowController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public ShowController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[Authorize(Roles = ROLE_ADMIN)]
		public IActionResult Index()
		{
			return View();
		}

		[Authorize(Roles = ROLE_ADMIN)]
		public IActionResult Upsert(int? id)
		{
			var viewModel = new ShowViewModel()
			{
				Films = _unitOfWork.Films
					.GetAll()
					.Select(f =>
						new SelectListItem(f.Title, f.FilmId.ToString())
					),
				Rooms = _unitOfWork.Rooms
					.GetAll()
					.Select(r =>
						new SelectListItem(r.RoomId.ToString(), r.RoomId.ToString())
					)
			};

			if (id is not null && id is not 0)
				viewModel.Show = _unitOfWork.Shows.GetFirstOrDefault(s => s.ShowId == id)!;

			viewModel.Show ??= new();

			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ROLE_ADMIN)]
		public IActionResult Upsert(ShowViewModel showViewModel)
		{
			if (!ModelState.IsValid)
				return View(showViewModel);

			if (showViewModel.Show.ShowId is 0)
			{
				_unitOfWork.Shows.Add(showViewModel.Show);
				_unitOfWork.Save();

				var associatedRoom = _unitOfWork.Rooms.GetFirstOrDefault(r => r.RoomId == showViewModel.Show.RoomId);
				foreach (var ticket in TicketHelpers.CreateTicketsForShow(showViewModel.Show, associatedRoom!))
					_unitOfWork.Tickets.Add(ticket);

				TempData["success"] = CREATE_SUCCESS;
			}
			else
			{
				_unitOfWork.Shows.Update(showViewModel.Show);
				TempData["success"] = EDIT_SUCCESS;
			}

			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		// APIs
		[HttpGet]
		public IActionResult GetAll()
		{
			var showsList = _unitOfWork.Shows.GetAll(includeProperties: "Film");
			return Json(new { data = showsList });
		}

		[HttpDelete]
		[Authorize(Roles = ROLE_ADMIN)]
		public IActionResult Delete(int? id)
		{
			if (id is null || id is 0)
				return NotFound();

			if (_unitOfWork.Shows.GetFirstOrDefault(s => s.ShowId == id) is not Show show)
				return Json(new { success = false, message = DELETE_FAIL });

			_unitOfWork.Shows.Remove(show);
			_unitOfWork.Save();

			return Json(new { success = true, message = DELETE_SUCCESS });
		}
	}
}
