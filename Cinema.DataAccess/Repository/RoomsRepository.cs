using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
	internal sealed class RoomsRepository : Repository<Room>
	{
		public RoomsRepository(CinemaDbContext db) : base(db){ }
	}
}
