using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
	internal sealed class FilmsRepository : Repository<Film>
	{
		public FilmsRepository(CinemaDbContext db) : base(db) { }
	}
}
