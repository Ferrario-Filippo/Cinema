using Cinema.DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Areas.API.Controllers
{
	[Area(Constants.Areas.API)]
	[Authorize]
	public class RoomController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public RoomController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult HasISense(int? roomId)
		{
			if (roomId is null || roomId is 0)
				return Json(new { HasISense = false });

			var hasISense = _unitOfWork.Rooms.GetFirstOrDefault(r => r.RoomId == roomId)?.HasISense ?? false;

			return Json(new { HasIsense = hasISense });
		}
	}
}
