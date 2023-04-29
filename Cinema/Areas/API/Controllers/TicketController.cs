using Cinema.DataAccess.Repository.Interfaces;
using Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Areas.API.Controllers
{
	[Area(Constants.Areas.API)]
	[Authorize]
	public sealed class TicketController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public TicketController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult GetAvailableSeats(int? showId)
		{
			if (showId is null || showId is 0)
				return Json(new { Message = "Non trovato" });

			var seats = _unitOfWork.Tickets
				.GetAll(t => t.ShowId == showId && t.UserId == null)
				.Select(t => new TicketInfo
				{
					Lane = t.Lane,
					Number = t.Number,
					Cost = t.Cost
				});

			return Json(new { Data = seats });
		}
	}
}
