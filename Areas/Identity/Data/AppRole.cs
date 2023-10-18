using Microsoft.AspNetCore.Identity;

namespace FlightBooking.Areas.Identity.Data
{
    public class AppRole: IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
