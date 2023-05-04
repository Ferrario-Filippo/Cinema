using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cinema.Models.ViewModels
{
	public sealed class ShowViewModel
	{
		public Show Show { get; set; } = null!;

		[ValidateNever]
		public IEnumerable<SelectListItem> Films { get; set; } = null!;

		[ValidateNever]
		public IEnumerable<SelectListItem> Rooms { get; set; } = null!;
	}
}
