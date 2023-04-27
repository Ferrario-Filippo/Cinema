using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using static Cinema.Constants.Messages;

namespace Cinema.Models
{
	[Display(Name = "Recensione")]
	public sealed class Review
	{
		private const string RATING_MESSAGE = "La valutazione deve essere compresa tra 1 e 5";

		[Key]
		public int ReviewId { get; set; }

		[Required(ErrorMessage = REQUIRED)]
		[Range(1, 5, ErrorMessage = RATING_MESSAGE)]
		[Display(Name = "Valutazione")]
		public byte Rating { get; set; } = 1;

		[Required(ErrorMessage = REQUIRED)]
		[MaxLength(450, ErrorMessage = USER_ID_MAX_LENGTH)]
		[Display(Name = "Utente")]
		public string UserId { get; set; } = string.Empty;

		[ValidateNever]
		public User User { get; set; } = null!;

		[Required(ErrorMessage = REQUIRED)]
		[Display(Name = "Film")]
		public int FilmId { get; set; }

		[ValidateNever]
		public Film Film { get; set; } = null!;
	}
}
