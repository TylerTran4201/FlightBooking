using Microsoft.EntityFrameworkCore;
using FlightBooking.Areas.Identity.Data;
using FlightBooking.Extensions;
using Microsoft.AspNetCore.Identity;
using FlightBooking.Seed;
using FlightBooking.Interface;
using FlightBooking.Repository;
using FlightBooking.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DataContextConnection' not found.");
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentityServices(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//seedData
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await Seed.SeedAirports(context);
    await Seed.SeedUsers(userManager, roleManager);
    await Seed.SeedTypeSeat(context);
}
catch (Exception ex)
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occureed during migration");
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Flights}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();