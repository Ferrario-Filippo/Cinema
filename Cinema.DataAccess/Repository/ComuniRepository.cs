using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
	internal sealed class ComuniRepository : Repository<Hometown>
	{
		public ComuniRepository(CinemaDbContext db) : base(db) { }
	}
}
