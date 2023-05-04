using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cinema.Models.ViewModels
{
	public sealed class TicketViewModel
	{
		public Ticket Ticket { get; set; } = null!;

		[ValidateNever]
		public IEnumerable<SelectListItem> Shows { get; set; } = null!;
	}
}
