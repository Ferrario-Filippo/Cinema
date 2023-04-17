using Cinema.DataAccess;
using Cinema.DataAccess.Repository;
using Cinema.DataAccess.Repository.Interfaces;
using Cinema.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("CinemaDbContextConnection");

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.AddDbContext<CinemaDbContext>(
	options => options.UseSqlServer(connectionString)
#if DEBUG
	.LogTo(Console.WriteLine, LogLevel.Information)
	.EnableSensitiveDataLogging()
	.EnableDetailedErrors()
#endif
);

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
	.AddDefaultTokenProviders()
	.AddEntityFrameworkStores<CinemaDbContext>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.Cookie.HttpOnly = true;
	options.SlidingExpiration = true;
	options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
	options.LoginPath = "/Identity/Account/Login";
	options.LogoutPath = "/Identity/Account/Logout";
	options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
	name: "default",
	pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.UseRequestLocalization("it-IT");

app.Run();
