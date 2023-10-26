using Microsoft.AspNetCore.Identity;

namespace FlightBooking.Models
{
    public class AppRole: IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
