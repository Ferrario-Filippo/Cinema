using Cinema.Models.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Cinema.Models.ViewModels
{
	public class BuyTicketsViewModel
	{
		[ValidateNever]
		public string FilmTitle { get; set; } = null!;

		public DateTime Date { get; set; } = DateTime.Now;

		[Required]
		public int ShowId { get; set; } = 0;

		public int Capacity { get; set; } = 0;

		public IEnumerable<TicketInfo> Tickets { get; set; } = null!;

		[Required]
		[Display(Name = "Nome sulla carta")]
		public string NameOnCard { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Numero della carta")]
		public string CardNumber { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Scadenza")]
		public string Expire { get; set;} = string.Empty;

		[Required]
		[Display(Name = "CCV")]
		public string CCV { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Metodo di pagamento")]
		public Payment ChosenPayment { get; set; } = Payment.Credit;

		[ValidateNever]
		public IEnumerable<SelectListItem> PaymentMethods { get; set; } = null!;
	}

	public sealed class TicketInfo
	{
		public char Lane { get; set; } = '\0';

		public int Number { get; set; } = 0;

		public double Cost { get; set; } = 5.0d;
	}
}
