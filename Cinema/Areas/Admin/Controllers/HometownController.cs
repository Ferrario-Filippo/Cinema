using Microsoft.AspNetCore.Mvc;
using static Cinema.Constants.Areas;

namespace Cinema.Areas.Admin.Controllers
{
	[Area(ADMIN)]
	public class HometownController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
