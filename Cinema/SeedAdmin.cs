using Cinema.Constants;
using Cinema.DataAccess;
using Cinema.Models.Enums;
using Cinema.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Cinema
{
	public static class SeedAdmin
	{
		public static async void Initialize(IServiceProvider provider)
		{
			using var unitOfWork = new CinemaDbContext(
				provider.GetRequiredService<DbContextOptions<CinemaDbContext>>());

			if (unitOfWork is null || unitOfWork.ApplicationUsers is null || unitOfWork.ApplicationUsers.Any())
				return;

			using var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
			using var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
			using var userStore = provider.GetRequiredService<IUserStore<IdentityUser>>();
			using var emailStore = (IUserEmailStore<IdentityUser>)userStore;

			var user = Activator.CreateInstance<User>();

			var email = "admin@mail.com";

			await userStore.SetUserNameAsync(user, email, CancellationToken.None);
			await emailStore.SetEmailAsync(user, email, CancellationToken.None);
			user.Name = "admin";
			user.Surname = "admin";
			user.Gender = Gender.NonBinary;
			user.HometownId = 1;
			user.BirthData = DateTime.Now;

			var result = await userManager.CreateAsync(user, "Informatica123!");

			if (result.Succeeded)
				await userManager.AddToRoleAsync(user, Roles.ROLE_ADMIN);
		}
	}
}
