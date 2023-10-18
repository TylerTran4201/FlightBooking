using FlightBooking.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FlightBooking.Seed
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {

            if (await userManager.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Seed/UserSeedData.json");

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            var roles = new List<AppRole>{
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Staff"},
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();

                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");
            }

            var admin = new AppUser
            {
                UserName = "admin"
            };
            await userManager.CreateAsync(admin, "Admin@2");

            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Staff" });

            var user1 = new AppUser
            {
                UserName = "user"
            };
            await userManager.CreateAsync(user1, "Admin@2");

            await userManager.AddToRolesAsync(user1, new[] { "Member" });

            var staff = new AppUser
            {
                UserName = "staff"
            };
            await userManager.CreateAsync(staff, "Admin@2");

            await userManager.AddToRolesAsync(staff, new[] { "Staff" });
        }
    }
}