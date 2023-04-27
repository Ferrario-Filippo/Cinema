using Cinema.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static Cinema.Constants.Messages;

namespace Cinema.Models
{
	[Display(Name = "Utente")]
	public sealed class User : IdentityUser
	{
		private const string CREDIT_MESSAGE = "Il credito deve essere compreso tra 0,00 e 100.000,00";

		[Required(ErrorMessage = REQUIRED)]
		[MaxLength(32, ErrorMessage = MAX_LENGTH_32)]
		[Display(Name = "Nome")]
		public string Name { get; set; } = string.Empty;

		[Required(ErrorMessage = REQUIRED)]
		[MaxLength(32, ErrorMessage = MAX_LENGTH_32)]
		[Display(Name = "Cognome")]
		public string Surname { get; set; } = string.Empty;

		[Required(ErrorMessage = REQUIRED)]
		[Display(Name = "Data di nascita")]
		public DateTime BirthData { get; set; } = DateTime.Now;

		[Required(ErrorMessage = REQUIRED)]
		[Display(Name = "Sesso")]
		public Gender Gender { get; set; }

		[Display(Name = "Credito")]
		[Range(0.0d, 100_000.0d, ErrorMessage = CREDIT_MESSAGE)]
		public double Credit { get; set; } = 0.0d;

		[Display(Name = "Comune di residenza")]
		public int HometownId { get; set; }

		[ValidateNever]
		[ForeignKey(nameof(HometownId))]
		public Hometown Hometown { get; set; } = null!;

		[JsonIgnore]
		[ValidateNever]
		public ICollection<Ticket> Tickets { get; set; } = null!;

		[JsonIgnore]
		[ValidateNever]
		public ICollection<Review> Reviews { get; set; } = null!;
	}
}