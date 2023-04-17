using Cinema.DataAccess.Repository.Interfaces;
using Cinema.Helpers;
using Cinema.Models;
using Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Cinema.Constants.Areas;
using static Cinema.Constants.Messages;
using static Cinema.Constants.Roles;

namespace Cinema.Areas.Admin.Controllers
{
	[Area(ADMIN)]
	public class FilmController : Controller
	{
		private readonly string _imageStore = Path.Combine("images", "film");

		private readonly IUnitOfWork _unitOfWork;

		private readonly IWebHostEnvironment _hostEnvironment;

		public FilmController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_hostEnvironment = hostEnvironment;
		}

		[Authorize(Roles = ROLE_ADMIN)]
		public IActionResult Index()
		{
			return View();
		}

		[Authorize(Roles = ROLE_ADMIN)]
		public IActionResult Upsert(int? id)
		{
			var filmViewModel = new FilmViewModel()
			{
				Film = new(),
				Genres = EnumHelpers.GetFilmGenres()
			};

			if (id is not null && id is not 0)
			{
				var filmInDb = _unitOfWork.Films.GetFirstOrDefault(f => f.FilmId == id);
				if (filmInDb is not null)
					filmViewModel.Film = filmInDb;
			}

			return View(filmViewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ROLE_ADMIN)]
		public IActionResult Upsert(FilmViewModel filmViewModel, IFormFile? file)
		{
			if (!ModelState.IsValid)
				return View(filmViewModel);

			SaveImageIfExists(filmViewModel.Film, file);

			if (filmViewModel.Film.FilmId is 0)
			{
				_unitOfWork.Films.Add(filmViewModel.Film);
				TempData["success"] = CREATE_SUCCESS;
			}
			else
			{
				_unitOfWork.Films.Update(filmViewModel.Film);
				TempData["success"] = EDIT_SUCCESS;
			}

			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		// APIs
		[HttpGet]
		public IActionResult GetAll()
		{
			var ratings = _unitOfWork.Reviews
				.GetAll()
				.GroupBy(r => r.FilmId)
				.Select(g => new { FilmId = g.Key, Rating = g.Average(r => r.Rating) });

			var filmsList =
				from film in _unitOfWork.Films.GetAll()
				join rating in ratings on film.FilmId equals rating.FilmId into fr
				from ratedFilms in fr.DefaultIfEmpty()
				select new
				{
					film.FilmId,
					film.Title,
					Description = film.Description[..30],
					film.Duration,
					film.Genre,
					film.Year,
					Rating = ratedFilms?.Rating ?? 0.0
				};

			return Json(new { data = filmsList });
		}

		[HttpDelete]
		[Authorize(Roles = ROLE_ADMIN)]
		public IActionResult Delete(int? id)
		{
			if (id is null || id is 0)
				return NotFound();

			if (_unitOfWork.Films.GetFirstOrDefault(f => f.FilmId == id) is not Film film)
				return Json(new { success = false, message = DELETE_FAIL });

			if (!string.IsNullOrWhiteSpace(film.ImageUrl))
				FileHelpers.DeleteImageIfExists(_hostEnvironment.WebRootPath, film.ImageUrl);

			_unitOfWork.Films.Remove(film);
			_unitOfWork.Save();

			return Json(new { success = true, message = DELETE_SUCCESS });
		}

		// Local helpers
		private void SaveImageIfExists(Film film, IFormFile? file)
		{
			if (file is null)
				return;

			var wwwRootPath = _hostEnvironment.WebRootPath;

			if (!string.IsNullOrWhiteSpace(film.ImageUrl))
				FileHelpers.DeleteImageIfExists(wwwRootPath, film.ImageUrl);

			var fileName = Guid.NewGuid().ToString();
			var fileExtension = Path.GetExtension(file.FileName);
			var fileUrlString = Path.Combine(_imageStore, fileName + fileExtension);
			var filePath = Path.Combine(wwwRootPath, fileUrlString);

			using (var fileStream = new FileStream(filePath, FileMode.Create))
				file.CopyTo(fileStream);

			film.ImageUrl = fileUrlString.Replace(@"\\", @"\");
		}
	}
}
