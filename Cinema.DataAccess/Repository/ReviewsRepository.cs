using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
	internal sealed class ReviewsRepository : Repository<Review>
	{
		public ReviewsRepository(CinemaDbContext context) : base(context) { }
	}
}
