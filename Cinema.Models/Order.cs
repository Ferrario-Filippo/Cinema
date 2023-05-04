using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cinema.Models
{
	public sealed class Order
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(450)]
		public string UserId { get; set; } = null!;

		[ForeignKey(nameof(UserId))]
		[ValidateNever]
		public User User { get; set; } = null!;

		public DateTime OrderDate { get; set; } = DateTime.Now;

		public double OrderTotal { get; set; }

		public string? OrderStatus { get; set; } = Constants.OrderStatus.PENDING;

		public string? PaymentStatus { get; set; } = Constants.PaymentStatus.PENDING;

		public string? SessionId { get; set; }

		public string? PaymentIntentId { get; set; }
	}
}
