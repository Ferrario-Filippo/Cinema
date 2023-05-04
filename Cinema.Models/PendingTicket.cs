namespace Cinema.Models
{
	public sealed class PendingTicket
	{
		public int OrderId { get; set; }

		public int ShowId { get; set; }

		public char Lane { get; set; }

		public int Number { get; set; }

		public double Cost { get; set; }
	}
}
