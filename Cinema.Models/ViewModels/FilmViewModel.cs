using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cinema.Models.ViewModels
{
	public class FilmViewModel
	{
		public Film Film { get; set; } = null!;

		[ValidateNever]
		public IEnumerable<SelectListItem> Genres { get; set; } = null!;
	}
}
