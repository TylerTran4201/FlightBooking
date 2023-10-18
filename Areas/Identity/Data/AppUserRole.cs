using Microsoft.AspNetCore.Identity;

namespace FlightBooking.Areas.Identity.Data
{
    public class AppUserRole: IdentityUserRole<int>
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }
}
