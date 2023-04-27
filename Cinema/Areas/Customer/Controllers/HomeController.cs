using Cinema.DataAccess.Repository.Interfaces;
using Cinema.Helpers;
using Cinema.Models;
using Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using static Cinema.Constants.Areas;

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


            var nextWeek = DateTime.Today.AddDays(7);
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

            var avgRating = _unitOfWork.Reviews.GetAll(r => r.FilmId == filmId).Average(r => r.Rating);
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