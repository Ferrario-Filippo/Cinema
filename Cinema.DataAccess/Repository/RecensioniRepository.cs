using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
	internal sealed class RecensioniRepository : Repository<Review>
	{
		public RecensioniRepository(CinemaDbContext context) : base(context) { }
	}
}
