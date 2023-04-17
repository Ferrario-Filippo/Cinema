using Cinema.Models;

namespace Cinema.DataAccess.Repository.Interfaces
{
	public interface IUnitOfWork
	{
		IRepository<User> ApplicationUsers { get; }
		
		IRepository<Film> Films { get; }
		
		IRepository<Review> Reviews { get; }
		
		IRepository<Room> Rooms { get; }
		
		IRepository<Show> Shows { get; }

		IRepository<Ticket> Tickets { get; }

		IRepository<Hometown> Towns { get; }

		void Save();
	}
}
