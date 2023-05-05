using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Cinema.Models.ViewModels
{
	public sealed class BuyTicketsViewModel
	{
		[ValidateNever]
		public string FilmTitle { get; set; } = null!;

		public DateTime Date { get; set; } = DateTime.Now;

		[Required]
		public int ShowId { get; set; } = 0;

		public int Capacity { get; set; } = 0;

		public List<TicketInfo> Tickets { get; set; } = null!;
	}

	public sealed class TicketInfo
	{
		[Display(Name = "Fila")]
		public char Lane { get; set; } = '\0';

		[Display(Name = "Numero")]
		public int Number { get; set; } = 0;

		[Display(Name = "Costo")]
		public double Cost { get; set; } = 0.0d;
	}
}
