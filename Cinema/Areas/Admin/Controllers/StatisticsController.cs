using Cinema.DataAccess.Repository.Interfaces;
using Cinema.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Cinema.Constants.Areas;
using static Cinema.Constants.Roles;

namespace Cinema.Areas.Admin.Controllers
{
    [Area(ADMIN)]
	[Authorize(Roles = ROLE_ADMIN)]
	public sealed class StatisticsController : Controller
    {
        private readonly IUnitOfWork _unitOFWork;

        public StatisticsController(IUnitOfWork unitOfWork)
        {
            _unitOFWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

		public IActionResult GetAverageDailyRevenues(int? day, int? month, int? year)
        {
            (day, month, year) = DateHelpers.GetTodayIfInvalid(day, month, year);

            var tickets = _unitOFWork.Tickets.GetAll().Where(t => t.UserId is not null);
            var shows = _unitOFWork.Shows
                .GetAll()
                .Where(s => 
                    s.Time.Day == day && 
                    s.Time.Month == month && 
                    s.Time.Year == year);

            var joined = tickets
                .Join(
                    shows, t => t.ShowId,
                    s => s.ShowId,
                    (t, s) => new
                    {
                        t.Cost,
                        s.FilmId
                    })
                .GroupBy(g => g.FilmId)
                .Select(g => new { FilmId = g.Key, Sum = g.Sum(t => t.Cost) })
                .Join(
                    _unitOFWork.Films.GetAll(),
                    g => g.FilmId,
                    f => f.FilmId,
                    (g, f) => new
                    {
                        f.FilmId,
                        f.Title,
                        Revenues = g.Sum
                    });

            return Json(new { data = joined });
        }

        public IActionResult GetBookedSeatsPerRoom(int? day, int? month, int? year)
        {
			(day, month, year) = DateHelpers.GetTodayIfInvalid(day, month, year);
			var shows = _unitOFWork.Shows
                .GetAll(includeProperties: "Room")
                .Where(s => 
                    s.Time.Day == day && 
                    s.Time.Month == month && 
                    s.Time.Year == year);

            var tickets = _unitOFWork.Tickets
                .GetAll()
                .Where(t => t.UserId is not null)
                .GroupBy(t => t.ShowId)
                .Select(g => new { ShowId = g.Key, Booked = g.Count() });

            var joined = tickets
                .Join(
                    shows,
                    t => t.ShowId,
                    s => s.ShowId,
                    (t, s) => new
                    {
                        t.ShowId,
                        t.Booked,
                        s.Room.Seats
                    });

            return Json(new { data = joined });
        }
    }
}
