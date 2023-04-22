using System.Linq.Expressions;

namespace Cinema.DataAccess.Repository.Interfaces
{
	public interface IRepository<T> where T : class
	{
		T? GetFirstOrDefault(Expression<Func<T, bool>> filterFunction, string? includeProperties = null);

		IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

		void Add(T entity);

		void Update(T entity);

		void Remove(T entity);

		void RemoveRange(IEnumerable<T> entities);
	}
}
