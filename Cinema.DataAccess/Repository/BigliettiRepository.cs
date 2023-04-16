using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
	internal sealed class BigliettiRepository : Repository<Ticket>
	{
		public BigliettiRepository(CinemaDbContext db) : base(db) { }
	}
}
