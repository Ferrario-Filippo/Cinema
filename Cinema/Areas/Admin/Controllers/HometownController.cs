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
	public sealed class HometownController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public HometownController(IUnitOfWork unitOfWork)
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
			Hometown? town = null;

			if (id is not null && id is not 0)
				town = _unitOfWork.Towns.GetFirstOrDefault(t => t.HometownId == id);

			return View(town ?? new Hometown());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ROLE_ADMIN)]
		public IActionResult Upsert(Hometown town)
		{
			if (!ModelState.IsValid)
				return View(town);

			if (town.HometownId is 0)
			{
				_unitOfWork.Towns.Add(town);
				TempData["success"] = CREATE_SUCCESS;
			}
			else
			{
				_unitOfWork.Towns.Update(town);
				TempData["success"] = EDIT_SUCCESS;
			}

			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		// APIs
		[HttpGet]
		public IActionResult GetAll()
		{
			var towns = _unitOfWork.Towns.GetAll();
			return Json(new { data = towns });
		}

		[HttpDelete]
		[Authorize(Roles = ROLE_ADMIN)]
		public IActionResult Delete(int? id)
		{
			if (id is null || id is 0)
				return NotFound();

			if (_unitOfWork.Towns.GetFirstOrDefault(t => t.HometownId == id) is not Hometown town)
				return Json(new { success = false, message = DELETE_FAIL });

			_unitOfWork.Towns.Remove(town);
			_unitOfWork.Save();

			return Json(new { success = true, message = DELETE_SUCCESS });
		}
	}
}
