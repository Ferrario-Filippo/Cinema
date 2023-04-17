using Cinema.DataAccess.Repository.Interfaces;
using Cinema.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Cinema.Constants.Areas;
using static Cinema.Constants.Messages;
using static Cinema.Constants.Roles;

namespace Cinema.Areas.Admin.Controllers
{
	[Area(ADMIN)]
	public class RoomController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public RoomController(IUnitOfWork unitOfWork)
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
			Room? room = null;

			if (id is not null && id is not 0)
				room = _unitOfWork.Rooms.GetFirstOrDefault(r => r.RoomId == id);

			return View(room ?? new Room());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ROLE_ADMIN)]
		public IActionResult Upsert(Room room)
		{
			if (!ModelState.IsValid)
				return View(room);

			if (room.RoomId is 0)
			{
				_unitOfWork.Rooms.Add(room);
				TempData["success"] = CREATE_SUCCESS;
			}
			else
			{
				_unitOfWork.Rooms.Update(room);
				TempData["success"] = EDIT_SUCCESS;
			}

			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		// APIs
		[HttpGet]
		public IActionResult GetAll()
		{
			var rooms = _unitOfWork.Rooms.GetAll();
			return Json(new { data = rooms });
		}

		[HttpDelete]
		[Authorize(Roles = ROLE_ADMIN)]
		public IActionResult Delete(int? id)
		{
			if (id is null || id is 0)
				return NotFound();

			if (_unitOfWork.Rooms.GetFirstOrDefault(r => r.RoomId == id) is not Room room)
				return Json(new { success = false, message = DELETE_FAIL });

			_unitOfWork.Rooms.Remove(room);
			_unitOfWork.Save();

			return Json(new { success = true, message = DELETE_SUCCESS });
		}
	}
}
