using FlightBooking.Data;
using FlightBooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FlightBooking.Seed
{
    public class Seed
    {


        public static async Task SeedData(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, DataContext _context){
            await Seed.SeedRoles(roleManager);
            await Seed.SeedUsers(userManager);
            await Seed.SeedAirports(_context);
            await Seed.SeedTypeSeat(_context);
        }

        private static async Task SeedRoles(RoleManager<AppRole> roleManager){
            if (await roleManager.Roles.AnyAsync()) return;
            var roles = new List<AppRole>{
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Member"},
                
                new AppRole{Name = "UsersView"},
                new AppRole{Name = "UsersCreate"},
                new AppRole{Name = "UsersEdit"},
                new AppRole{Name = "UsersDelete"},

                new AppRole{Name = "RolesView"},

                new AppRole{Name = "AirlineCompaniesView"},
                new AppRole{Name = "AirlineCompaniesCreate"},
                new AppRole{Name = "AirlineCompaniesEdit"},
                new AppRole{Name = "AirlineCompaniesDelete"},

                new AppRole{Name = "AirlinesView"},
                new AppRole{Name = "AirlinesCreate"},
                new AppRole{Name = "AirlinesEdit"},
                new AppRole{Name = "AirlinesDelete"},

                new AppRole{Name = "SeatsView"},

                new AppRole{Name = "TypeSeatsView"},

                new AppRole{Name = "SchedulesView"},
                new AppRole{Name = "SchedulesCreate"},
                new AppRole{Name = "SchedulesEdit"},
                new AppRole{Name = "SchedulesDelete"},

                new AppRole{Name = "AirportsView"},
            };

             foreach(var role in roles){
                await roleManager.CreateAsync(role);
             }
        }

        private static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Seed/UserSeedData.json");

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");
            }

            //create new and set role admin account
            var admin = new AppUser
            {
                UserName = "admin"
            };
            await userManager.CreateAsync(admin, "Abc123!");

            await userManager.AddToRolesAsync(admin, new[] { "Admin" , "Member"});

            var user1 = new AppUser
            {
                UserName = "admin1"
            };
            await userManager.CreateAsync(user1, "Abc123!");

            await userManager.AddToRolesAsync(user1, new[] { "Member" });
        }

        private static async Task SeedAirports(DataContext _context) {
            if (await _context.Airports.AnyAsync())
                return;

            
            using (StreamReader sr = File.OpenText("Seed/AirportSeedData.json")) { 
                var airports = JsonSerializer.Deserialize<List<Airport>>(sr.ReadToEnd());
                foreach (Airport airport in airports) {
                    _context.Add(airport);
                }
            }
            await _context.SaveChangesAsync();
        }
        private static async Task SeedTypeSeat(DataContext _context){
            if(await _context.TypeSeats.AnyAsync())
                return;
            var typeSeats = new List<TypeSeat>{
                new TypeSeat{
                    Name = "Business Seat",
                    Price = 156.99
                },
                new TypeSeat{
                    Name = "Economy Seat",
                    Price = 0
                }
            };
            
            foreach(var typeSeat in typeSeats){
                _context.TypeSeats.Add(typeSeat);
            }
            await _context.SaveChangesAsync();
        }
    }
}

