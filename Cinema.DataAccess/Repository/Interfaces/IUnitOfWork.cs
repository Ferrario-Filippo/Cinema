using Cinema.Models;

namespace Cinema.DataAccess.Repository.Interfaces
{
	public interface IUnitOfWork
	{
		IRepository<Ticket> Biglietti { get; }

		IRepository<Hometown> Comuni { get; }
		
		IRepository<Film> Film { get; }
		
		IRepository<Show> Proiezioni { get; }

		IRepository<Review> Recensioni { get; }
		
		IRepository<Room> Sale { get; }

		IRepository<User> Utenti { get; }

		void Save();
	}
}
