using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Cinema.Models
{
	[Display(Name = "Proiezione")]
	public sealed class Show
	{
		public int ShowId { get; set; }

		[Required]
		[Display(Name = "Data e ora")]
		public DateTime Time { get; set; }

		[Required]
		[Display(Name = "Film")]
		public int FilmId { get; set; }

		[ValidateNever]
		public Film Film { get; set; } = null!;

		[Required]
		[Display(Name = "Sala")]
		public int RoomId { get; set; }

		[ValidateNever]
		public Room Room { get; set; } = null!;

		[JsonIgnore]
		[ValidateNever]
		public ICollection<Ticket> Tickets { get; set; } = null!;
	}
}
