using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cinema.Models.ViewModels
{
	public sealed class HomeViewModel
	{
		public IEnumerable<FilmDisplayViewModel> FilmDisplays { get; set; } = null!;

		public IEnumerable<SelectListItem> Genres { get; set; } = null!;
	}
}
