using Cinema.DataAccess.Repository.Interfaces;
using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
	public sealed class UnitOfWork : IUnitOfWork
	{
		private readonly CinemaDbContext _db;

		public IRepository<Ticket> Biglietti { get; init; }
		
		public IRepository<Hometown> Comuni { get; init; }

		public IRepository<Film> Film { get; init; }
		
		public IRepository<Show> Proiezioni { get; init; }
		
		public IRepository<Room> Sale { get; init; }
		
		public IRepository<Review> Recensioni { get; init; }
		
		public IRepository<User> Utenti { get; init; }

		public UnitOfWork(CinemaDbContext db)
		{
			_db = db;

			Biglietti = new BigliettiRepository(db);
			Comuni = new ComuniRepository(db);
			Film = new FilmRepository(db);
			Proiezioni = new ProiezioniRepository(db);
			Recensioni = new RecensioniRepository(db);
			Sale = new SaleRepository(db);
			Utenti = new UtentiRepository(db);
		}

		public void Save()
		{
			_db.SaveChanges();
		}
	}
}
