using Cinema.Models.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Cinema.Models
{
	public sealed class Film
	{
		[Key]
		public int FilmId { get; set; }

		[Required]
		[MaxLength(32)]
		[Display(Name = "Titolo")]
		public string Title { get; set; } = string.Empty;

		[Required]
		[MaxLength(500)]
		[Display(Name = "Descrizione")]
		public string Description { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Genere")]
		public FilmGenre Genre { get; set; }

		[Required]
		[Range(10, 300)]
		[Display(Name = "Durata")]
		public ushort Duration { get; set; }

		[Required]
		[Range(1849, 2030)]
		[Display(Name = "Anno di produzione")]
		public ushort Year { get; set; }

		[MaxLength(250)]
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
