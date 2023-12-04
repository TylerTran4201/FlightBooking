using Microsoft.EntityFrameworkCore;
using FlightBooking.Data;
using FlightBooking.Extensions;
using Microsoft.AspNetCore.Identity;
using FlightBooking.Seed;
using FlightBooking.Interface;
using FlightBooking.Repository;
using FlightBooking.Models;
using FlightBooking.Helpers;
using FlightBooking.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DataContextConnection' not found.");
builder.Services.AddDbContext<DataContext>(options => {options.UseSqlServer(connectionString); options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);}, ServiceLifetime.Transient);
//add serviceExtention
builder.Services.AddIdentityServices(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSession();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<IPhotoServices, PhotoService>();

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
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();
app.UseStatusCodePagesWithRedirects("/Errors/PageNotFound");

//seedData
using var scope = app.Services.CreateScope(); 
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await Seed.SeedData(userManager, roleManager, context);
}
catch (Exception ex)
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occureed during migration");
}


app.UseEndpoints(endpoints =>{
    endpoints.MapControllerRoute(
        name: "Admin",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Flights}/{action=Index}/{id?}");   
});

app.MapRazorPages();

app.Run();