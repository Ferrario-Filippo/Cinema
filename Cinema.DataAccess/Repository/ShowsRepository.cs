using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
	internal sealed class ShowsRepository : Repository<Show>
	{ 
		public ShowsRepository(CinemaDbContext db) : base(db) { }
	}
}
