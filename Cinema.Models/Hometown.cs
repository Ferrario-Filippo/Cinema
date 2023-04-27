using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static Cinema.Constants.Messages;

namespace Cinema.Models
{
	[Display(Name = "Comune")]
	public sealed class Hometown
	{
		[Key]
		public int HometownId { get; set; }
		
		[Required(ErrorMessage = REQUIRED)]
		[MaxLength(32, ErrorMessage = MAX_LENGTH_32)]
		[Display(Name = "Nome")]
		public string Name { get; set; } = string.Empty;

		[JsonIgnore]
		[ValidateNever]
		public ICollection<User> Users { get; set; } = null!;
	}
}
