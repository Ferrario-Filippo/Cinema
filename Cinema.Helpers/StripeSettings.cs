namespace Cinema.Helpers
{
	public sealed class StripeSettings
	{
		public string SecretKey { get; set; } = null!;

		public string PublishableKey { get; set; } = null!;
	}
}
