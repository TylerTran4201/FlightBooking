using Microsoft.AspNetCore.Identity;
namespace FlightBooking.Models
{

    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string City { get; set; }
        public string Country { get; set; }
        public Photo Photo { get; set; } = new();

        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}