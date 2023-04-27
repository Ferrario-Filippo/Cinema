using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static Cinema.Constants.Messages;

namespace Cinema.Models
{
	[Display(Name = "Proiezione")]
	public sealed class Show
	{
		public int ShowId { get; set; }

		[Required(ErrorMessage = REQUIRED)]
		[Display(Name = "Data e ora")]
		public DateTime Time { get; set; } = DateTime.Now;

		[Required(ErrorMessage = REQUIRED)]
		[Display(Name = "È in 3D")]
		public bool Is3D { get; set; }

		[Required(ErrorMessage = REQUIRED)]
		[Display(Name = "Film")]
		public int FilmId { get; set; }

		[ValidateNever]
		public Film Film { get; set; } = null!;

		[Required(ErrorMessage = REQUIRED)]
		[Display(Name = "Sala")]
		public int RoomId { get; set; }

		[ValidateNever]
		public Room Room { get; set; } = null!;

		[JsonIgnore]
		[ValidateNever]
		public IEnumerable<Ticket> Tickets { get; set; } = null!;
	}
}
