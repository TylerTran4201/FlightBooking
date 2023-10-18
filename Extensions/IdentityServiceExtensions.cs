using FlightBooking.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace FlightBooking.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
                    .AddEntityFrameworkStores<DataContext>()
                    .AddDefaultTokenProviders();
            return services;
        }
    }
}

