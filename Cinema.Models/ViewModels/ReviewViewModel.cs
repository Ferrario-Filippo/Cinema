using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cinema.Models.ViewModels
{
	public sealed class ReviewViewModel
	{
		public Review Review { get; set; } = null!;

		[ValidateNever]
		public IEnumerable<SelectListItem> Films { get; set; } = null!;

		[ValidateNever]
		public IEnumerable<SelectListItem> Users { get; set; } = null!;
	}
}
