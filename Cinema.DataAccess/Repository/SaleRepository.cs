using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
	internal sealed class SaleRepository : Repository<Room>
	{
		public SaleRepository(CinemaDbContext db) : base(db){ }
	}
}
