using Cinema.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess
{
	public sealed class CinemaDbContext : IdentityDbContext<IdentityUser>
	{
		public DbSet<User> ApplicationUsers { get; set; } = null!;

		public DbSet<Film> Films { get; set; } = null!;

		public DbSet<Review> Reviews { get; set; } = null!;

		public DbSet<Room> Rooms { get; set; } = null!;

		public DbSet<Show> Shows { get; set; } = null!;

		public DbSet<Ticket> Tickets { get; set; } = null!;

		public DbSet<Hometown> Towns { get; set; } = null!;

		public CinemaDbContext(DbContextOptions<CinemaDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<Ticket>()
				.HasKey(t => new { t.Lane, t.Number, t.ShowId });

			builder.Entity<Ticket>()
				.HasOne(t => t.Show)
				.WithMany(s => s.Tickets)
				.HasForeignKey(t => t.ShowId);

			builder.Entity<Show>()
				.HasOne(s => s.Film)
				.WithMany(f => f.Shows)
				.HasForeignKey(s => s.FilmId);
			builder.Entity<Show>()
				.HasOne(s => s.Room)
				.WithMany(r => r.Shows)
				.HasForeignKey(s => s.RoomId);

			builder.Entity<Review>()
				.HasOne(r => r.User)
				.WithMany(u => u.Reviews)
				.HasForeignKey(r => r.UserId);
			builder.Entity<Review>()
				.HasOne(r => r.Film)
				.WithMany(f => f.Reviews)
				.HasForeignKey(r => r.FilmId);
		}
	}
}