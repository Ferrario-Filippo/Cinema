using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
	internal sealed class PendingTicketsRepository : Repository<PendingTicket>
	{
		public PendingTicketsRepository(CinemaDbContext dbContext) : base(dbContext) { }
	}
}
