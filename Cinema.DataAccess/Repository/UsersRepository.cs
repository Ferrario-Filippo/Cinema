using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
	internal sealed class UsersRepository : Repository<User>
	{
		public UsersRepository(CinemaDbContext db) : base(db) { }
	}
}
