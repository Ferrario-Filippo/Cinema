using Cinema.DataAccess;
using Cinema.Models;
using Microsoft.EntityFrameworkCore;

namespace Cinema
{
	public static class SeedHometowns
	{
		public static void Initialize(IServiceProvider provider)
		{
			using var unitOfWork = new CinemaDbContext(
				provider.GetRequiredService<DbContextOptions<CinemaDbContext>>());

			if (unitOfWork is null || unitOfWork.Towns is null || unitOfWork.Towns.Any())
				return;

			string path =
#if DEBUG
				"./wwwroot/comuni.txt"
#else
				"./comuni.txt"
#endif
				;

			using (var reader = new StreamReader(path))
			{
				while (!reader.EndOfStream)
				{
					unitOfWork.Towns.Add(new Hometown()
					{
						HometownId = 0,
						Name = reader.ReadLine() ?? string.Empty
					});
				}
			}

			unitOfWork.SaveChanges();
		}
	}
}
