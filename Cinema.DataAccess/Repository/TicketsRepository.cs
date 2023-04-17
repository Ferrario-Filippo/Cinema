using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
	internal sealed class TicketsRepository : Repository<Ticket>
	{
		public TicketsRepository(CinemaDbContext db) : base(db) { }
	}
}
