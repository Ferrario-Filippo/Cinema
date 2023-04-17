using Cinema.DataAccess.Repository.Interfaces;
using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
	public sealed class UnitOfWork : IUnitOfWork
	{
		private readonly CinemaDbContext _db;

		public IRepository<User> ApplicationUsers { get; init; }
		
		public IRepository<Film> Films { get; init; }
		
		public IRepository<Review> Reviews { get; init; }
		
		public IRepository<Room> Rooms { get; init; }
		
		public IRepository<Show> Shows { get; init; }
		
		public IRepository<Ticket> Tickets { get; init; }
		
		public IRepository<Hometown> Towns { get; init; }		

		public UnitOfWork(CinemaDbContext db)
		{
			_db = db;

			ApplicationUsers = new UsersRepository(db);
			Films = new FilmsRepository(db);
			Shows = new ShowsRepository(db);
			Reviews = new ReviewsRepository(db);
			Rooms = new RoomsRepository(db);
			Tickets = new TicketsRepository(db);
			Towns = new TownsRepository(db);
		}

		public void Save()
		{
			_db.SaveChanges();
		}
	}
}
