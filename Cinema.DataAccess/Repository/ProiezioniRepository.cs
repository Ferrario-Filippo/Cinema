using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
	internal sealed class ProiezioniRepository : Repository<Show>
	{ 
		public ProiezioniRepository(CinemaDbContext db) : base(db) { }
	}
}
