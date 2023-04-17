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
	public class ReviewController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public ReviewController(IUnitOfWork unitOfWork)
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
			var reviewViewModel = new ReviewViewModel()
			{
				Films = _unitOfWork.Films
					.GetAll()
					.Select(f =>
						new SelectListItem(f.Title, f.FilmId.ToString())
					),
				Users = _unitOfWork.ApplicationUsers
					.GetAll()
					.Select(u =>
						new SelectListItem(u.Email, u.Id)
					)
			};

			if (id is not null && id is not 0)
				reviewViewModel.Review = _unitOfWork.Reviews.GetFirstOrDefault(r => r.ReviewId == id)!;

			reviewViewModel.Review ??= new();

			return View(reviewViewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ROLE_ADMIN)]
		public IActionResult Upsert(ReviewViewModel reviewViewModel)
		{
			if (!ModelState.IsValid)
				return View(reviewViewModel);

			if (reviewViewModel.Review.ReviewId is 0)
			{
				_unitOfWork.Reviews.Add(reviewViewModel.Review);
				TempData["success"] = CREATE_SUCCESS;
			}
			else
			{
				_unitOfWork.Reviews.Update(reviewViewModel.Review);
				TempData["success"] = EDIT_SUCCESS;
			}

			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		// APIs
		[HttpGet]
		public IActionResult GetAll()
		{
			var reviewsList = _unitOfWork.Reviews.GetAll();
			return Json(new { data = reviewsList });
		}

		[HttpDelete]
		[Authorize(Roles = ROLE_ADMIN)]
		public IActionResult Delete(int? id)
		{
			if (id is null || id is 0)
				return NotFound();

			if (_unitOfWork.Reviews.GetFirstOrDefault(r => r.ReviewId == id) is not Review review)
				return Json(new { success = false, message = DELETE_FAIL });

			_unitOfWork.Reviews.Remove(review);
			_unitOfWork.Save();

			return Json(new { success = true, message = DELETE_SUCCESS });
		}
	}
}
