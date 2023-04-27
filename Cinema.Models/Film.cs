using Cinema.Models.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static Cinema.Constants.Messages;
using static Cinema.Constants.Time;

namespace Cinema.Models
{
	public sealed class Film
	{
		private const string MAX_LENGTH_500 = "Il campo ha lunghezza massima 500";
		private const string MAX_LENGTH_250 = "Il campo ha lunghezza massima 250";
		private const string DURATION_MESSAGE = "La durata deve essere compresa tra 10 e 300";
		private const string YEAR_MESSAGE = $"L'anno di produzione deve essere compreso tra 1849 e 2030";

		[Key]
		public int FilmId { get; set; }

		[Required(ErrorMessage = REQUIRED)]
		[MaxLength(32, ErrorMessage = MAX_LENGTH_32)]
		[Display(Name = "Titolo")]
		public string Title { get; set; } = string.Empty;

		[Required(ErrorMessage = REQUIRED)]
		[MaxLength(500, ErrorMessage = MAX_LENGTH_500)]
		[Display(Name = "Descrizione")]
		public string Description { get; set; } = string.Empty;

		[Required(ErrorMessage = REQUIRED)]
		[Display(Name = "Genere")]
		public FilmGenre Genre { get; set; }

		[Required(ErrorMessage = REQUIRED)]
		[Range(10, 300, ErrorMessage = DURATION_MESSAGE)]
		[Display(Name = "Durata")]
		public ushort Duration { get; set; } = 120;

		[Required(ErrorMessage = REQUIRED)]
		[Range(MIN_YEAR, MAX_YEAR, ErrorMessage = YEAR_MESSAGE)]
		[Display(Name = "Anno di produzione")]
		public ushort Year { get; set; } = (ushort)DateTime.Now.Year;

		[MaxLength(250, ErrorMessage = MAX_LENGTH_250)]
		[Display(Name = "Url immagine copertina")]
		public string? ImageUrl { get; set; }

		[JsonIgnore]
		[ValidateNever]
		public ICollection<Review> Reviews { get; set; } = null!;

		[JsonIgnore]
		[ValidateNever]
		public ICollection<Show> Shows { get; set; } = null!;
	}
}
