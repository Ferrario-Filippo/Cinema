using Cinema.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cinema.Models
{
	[Display(Name = "Utente")]
	public sealed class User : IdentityUser
	{
		[Required]
		[MaxLength(32)]
		[Display(Name = "Nome")]
		public string Name { get; set; } = string.Empty;

		[Required]
		[MaxLength(32)]
		[Display(Name = "Cognome")]
		public string Surname { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Data di nascita")]
		public DateTime BirthData { get; set; } = DateTime.Now;

		[Required]
		[Display(Name = "Sesso")]
		public Gender Gender { get; set; }

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