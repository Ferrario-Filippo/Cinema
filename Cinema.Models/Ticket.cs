using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Cinema.Models
{
	[Display(Name = "Biglietto")]
	public sealed class Ticket
	{
		[Required]
		[Range(0, 64)]
		[Display(Name = "Numero")]
		public ushort Number { get; set; }

		[Required]
		[Display(Name = "Fila")]
		public char Lane { get; set; } = '\0';

		[Required]
		[Range(1.00, 39.99)]
		[Display(Name = "Costo")]
		public double Cost { get; set; }

		[Required]
		[Display(Name = "Proiezione")]
		public int ShowId { get; set; }

		[ValidateNever]
		public Show Show { get; set; } = null!;

		[MaxLength(450)]
		[Display(Name = "User")]
		public string? UserId { get; set; } = null;
		
		[ValidateNever]
		public User? User { get; set;}
	}
}
