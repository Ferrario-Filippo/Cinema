using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Cinema.Models
{
	[Display(Name = "Recensione")]
	public sealed class Review
	{
		[Key]
		public int ReviewId { get; set; }

		[Required]
		[Range(1, 5)]
		[Display(Name = "Valutazione")]
		public byte Rating { get; set; }

		[Required]
		[MaxLength(450)]
		[Display(Name = "Utente")]
		public string UserId { get; set; } = string.Empty;

		[ValidateNever]
		public User User { get; set; } = null!;

		[Required]
		[Display(Name = "Film")]
		public int FilmId { get; set; }

		[ValidateNever]
		public Film Film { get; set; } = null!;
	}
}
