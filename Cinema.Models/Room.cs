
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Cinema.Models
{
	[Display(Name = "Sala")]
	public sealed class Room
	{
		[Key]
		public int RoomId { get; set; }

		[Required]
		[Display(Name = "Ha l'ISense")]
		public bool HasISense { get; set; }

		[Required]
		[Display(Name = "Posti")]
		[Range(0, ushort.MaxValue)]
		public ushort Seats { get; set; } = 0;

		[JsonIgnore]
		[ValidateNever]
		public ICollection<Show> Shows { get; set; } = null!;
	}
}
