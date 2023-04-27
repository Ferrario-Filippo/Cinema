using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using static Cinema.Constants.Messages;

namespace Cinema.Models
{
	[Display(Name = "Biglietto")]
	public sealed class Ticket
	{
		private const string TICKET_NUMBER = "Il numero del biglietto deve essere compreso tra 0 e 64";
		private const string TICKET_COST = "Il costo del biglietto deve essere compreso tra 1.00 e 39.99";

		[Required(ErrorMessage = REQUIRED)]
		[Range(0, 64, ErrorMessage = TICKET_NUMBER)]
		[Display(Name = "Numero")]
		public ushort Number { get; set; }

		[Required(ErrorMessage = REQUIRED)]
		[Display(Name = "Fila")]
		public char Lane { get; set; } = '\0';

		[Required(ErrorMessage = REQUIRED)]
		[Range(1.00, 39.99, ErrorMessage = TICKET_COST)]
		[Display(Name = "Costo")]
		public double Cost { get; set; }

		[Required(ErrorMessage = REQUIRED)]
		[Display(Name = "Proiezione")]
		public int ShowId { get; set; }

		[ValidateNever]
		public Show Show { get; set; } = null!;

		[MaxLength(450, ErrorMessage = USER_ID_MAX_LENGTH)]
		[Display(Name = "User")]
		public string? UserId { get; set; } = null;
		
		[ValidateNever]
		public User? User { get; set;}
	}
}
