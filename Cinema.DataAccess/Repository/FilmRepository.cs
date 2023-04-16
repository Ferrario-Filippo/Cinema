using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
	internal sealed class FilmRepository : Repository<Film>
	{
		public FilmRepository(CinemaDbContext db) : base(db) { }
	}
}
