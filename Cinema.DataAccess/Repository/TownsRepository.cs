using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
	internal sealed class TownsRepository : Repository<Hometown>
	{
		public TownsRepository(CinemaDbContext db) : base(db) { }
	}
}
