using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
	internal sealed class UtentiRepository : Repository<User>
	{
		public UtentiRepository(CinemaDbContext db) : base(db) { }
	}
}
