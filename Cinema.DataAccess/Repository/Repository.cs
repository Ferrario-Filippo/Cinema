using Cinema.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cinema.DataAccess.Repository
{
	public abstract class Repository<T> : IRepository<T> where T : class
	{
		internal DbSet<T> dbSet;

		public Repository(CinemaDbContext db)
		{
			dbSet = db.Set<T>();
		}

		public void Add(T entity)
		{
			dbSet.Add(entity);
		}

		public IEnumerable<T> GetAll(string? includeProperties)
		{
			IQueryable<T> query = dbSet;

			if (!string.IsNullOrWhiteSpace(includeProperties))
			{
				var properties = includeProperties
					.Split(',', StringSplitOptions.RemoveEmptyEntries)
					.Select(p => p.Trim());

				foreach (var p in properties)
					query = query.Include(p);
			}

			return query;
		}

		public T? GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties)
		{
			IQueryable<T> query = dbSet.Where(filter);

			if (!string.IsNullOrWhiteSpace(includeProperties))
			{
				var properties = includeProperties
					.Split(',', StringSplitOptions.RemoveEmptyEntries)
					.Select(p => p.Trim());

				foreach (var p in properties)
					query = query.Include(p);
			}

			return query.FirstOrDefault();
		}

		public void Update(T entity)
		{
			dbSet.Update(entity);
		}

		public void Remove(T entity)
		{
			dbSet.Remove(entity);
		}

		public void RemoveRange(IEnumerable<T> entities)
		{
			dbSet.RemoveRange(entities);
		}
	}
}
