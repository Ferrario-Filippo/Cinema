using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static Cinema.Constants.Messages;

namespace Cinema.Models
{
	[Display(Name = "Sala")]
	public sealed class Room
	{
		private const string SEATS_MESSAGE = "Il numero di posti deve essere compreso tra 0 e 1000";

		[Key]
		public int RoomId { get; set; }

		[Required(ErrorMessage = REQUIRED)]
		[Display(Name = "Ha l'ISense")]
		public bool HasISense { get; set; }

		[Required(ErrorMessage = REQUIRED)]
		[Display(Name = "Posti")]
		[Range(0, 1000, ErrorMessage = SEATS_MESSAGE)]
		public ushort Seats { get; set; } = 100;

		[JsonIgnore]
		[ValidateNever]
		public ICollection<Show> Shows { get; set; } = null!;
	}
}
