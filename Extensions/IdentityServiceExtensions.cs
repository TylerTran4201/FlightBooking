using FlightBooking.Data;
using FlightBooking.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
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
                    .AddRoles<AppRole>()
                    .AddEntityFrameworkStores<DataContext>()
                    .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(config => {
                config.LoginPath= "/Identity/Account/Login";
                config.AccessDeniedPath = "/Errors/AccessDenied";
            });
            
            services.AddAuthorization(opt=>{
                opt.AddPolicy("Member", policy => policy.RequireRole("Admin","Member"));

                opt.AddPolicy("UsersView", policy => policy.RequireRole("Admin","UsersView"));
                opt.AddPolicy("UsersCreate", policy => policy.RequireRole("Admin","UsersCreate"));
                opt.AddPolicy("UsersEdit", policy => policy.RequireRole("Admin","UsersEdit"));
                opt.AddPolicy("UsersDelete", policy => policy.RequireRole("Admin","UsersDelete"));

                opt.AddPolicy("RolesView", policy => policy.RequireRole("Admin","RolesView"));

                opt.AddPolicy("AirlineCompaniesView", policy => policy.RequireRole("Admin","AirlineCompaniesView"));
                opt.AddPolicy("AirlineCompaniesCreate", policy => policy.RequireRole("Admin","AirlineCompaniesCreate"));
                opt.AddPolicy("AirlineCompaniesEdit", policy => policy.RequireRole("Admin","AirlineCompaniesEdit"));
                opt.AddPolicy("AirlineCompaniesDelete", policy => policy.RequireRole("Admin,AirlineCompaniesDelete"));

                opt.AddPolicy("AirlinesView", policy => policy.RequireRole("Admin","AirlinesView"));
                opt.AddPolicy("AirlinesCreate", policy => policy.RequireRole("Admin","AirlinesCreate"));
                opt.AddPolicy("AirlinesEdit", policy => policy.RequireRole("Admin","AirlinesEdit"));
                opt.AddPolicy("AirlinesDelete", policy => policy.RequireRole("Admin","AirlinesDelete"));

                opt.AddPolicy("SeatsView", policy => policy.RequireRole("Admin","SeatsView"));

                opt.AddPolicy("TypeSeatsView", policy => policy.RequireRole("Admin","TypeSeatsView"));

                opt.AddPolicy("SchedulesView", policy => policy.RequireRole("Admin","SchedulesView"));
                opt.AddPolicy("SchedulesCreate", policy => policy.RequireRole("Admin","SchedulesCreate"));
                opt.AddPolicy("SchedulesEdit", policy => policy.RequireRole("Admin","SchedulesEdit"));
                opt.AddPolicy("SchedulesDelete", policy => policy.RequireRole("Admin","SchedulesDelete"));

                opt.AddPolicy("AirportsView", policy => policy.RequireRole("Admin","AirportsView"));
            });
            return services;
        }
    }
}

