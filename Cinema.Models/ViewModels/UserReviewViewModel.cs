namespace Cinema.Models.ViewModels
{
	public sealed class UserReviewViewModel
	{
		public Film Film { get; set; } = null!;

		public int Rating { get; set; } = 0;
	}
}
