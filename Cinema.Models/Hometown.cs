using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Cinema.Models
{
	[Display(Name = "Comune")]
	public sealed class Hometown
	{
		[Key]
		public int HometownId { get; set; }
		
		[Required]
		[MaxLength(32)]
		[Display(Name = "Nome")]
		public string Name { get; set; } = string.Empty;

		[JsonIgnore]
		[ValidateNever]
		public ICollection<User> Users { get; set; } = null!;
	}
}
