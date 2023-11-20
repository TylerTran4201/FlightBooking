using FlightBooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightBooking.Data;

public class DataContext : IdentityDbContext<AppUser, AppRole, int,
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }
    public DbSet<AirlineCompany> AirlineCompanies { get; set; }
    public DbSet<Airline> Airlines { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<TypeSeat> TypeSeats { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Airport> Airports { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Bill> Bills { get; set; }
    public DbSet<PassengerInformation> PassengerInformations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // AppUser - AppRole
        builder.Entity<AppUser>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        builder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        // Arline - AirlineCompany (one to many)
        builder.Entity<Airline>()
             .HasOne(e => e.AirlineCompany)
             .WithMany(e => e.Airlines)
             .HasForeignKey(e => e.AirlineComanyId)
             .IsRequired();

        // Seat - Airline (one to many)
        builder.Entity<Seat>()
            .HasOne(e => e.Airline)
            .WithMany(e => e.Seats)
            .HasForeignKey(e => e.AirlineId)
            .IsRequired();

        // Seat - TypeSeat (one to many)
        builder.Entity<Seat>()
            .HasOne(e => e.TypeSeat)
            .WithMany(e => e.Seats)
            .HasForeignKey(e => e.TypeSeatId)
            .IsRequired();

        // Schedule - DepartureAirport (one to many)
        builder.Entity<Schedule>()
             .HasOne(e => e.DepartureAirport)
             .WithMany(e => e.DepartureSchedules)
             .HasForeignKey(e => e.DepartureAirportId)
             .OnDelete(DeleteBehavior.NoAction);

        // Schedule - DestinationAirport (one to many)
        builder.Entity<Schedule>()
             .HasOne(e => e.DestinationAirport)
             .WithMany(e => e.DestinationSchedules)
             .HasForeignKey(e => e.DestinationAirportId)
             .OnDelete(DeleteBehavior.NoAction);

        // Ticket - Seat (one to one)
        builder.Entity<Ticket>()
            .HasOne(e=> e.Seat)
            .WithOne()
            .HasForeignKey<Ticket>(e=> e.SeatId)
            .OnDelete(DeleteBehavior.NoAction);

        //Booking - Ticket (one to many)
        builder.Entity<Booking>()
            .HasMany(e=> e.Tickets)
            .WithOne(e=> e.Booking)
            .HasForeignKey(e=> e.BookingId)
            .OnDelete(DeleteBehavior.NoAction);

        //User - booking (one to many)
        builder.Entity<AppUser>()
            .HasMany(u => u.Bookings)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.NoAction);
            
        //Ticket - PassengerInfomation (one to one)
        builder.Entity<Ticket>()
            .HasOne(t => t.PassengerInformation)
            .WithOne(p => p.Ticket)
            .HasForeignKey<Ticket>(e => e.PassengerInformationId)
            .IsRequired();

        // Airline - Schedule (one to one)
        builder.Entity<Airline>()
            .HasOne(e=> e.Schedule)
            .WithOne(e=> e.Airline)
            .HasForeignKey<Schedule>(e => e.AirlineId)
            .IsRequired();

        //Order - Booking (one to one)
        builder.Entity<Bill>()
            .HasOne(o => o.Booking)
            .WithOne()
            .HasForeignKey<Bill>(o => o.BookingId)
            .IsRequired();

        builder.ApplyConfiguration(new AppUserConfig());
    }
}

public class AppUserConfig : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(u => u.FirstName).HasMaxLength(255);
        builder.Property(u => u.LastName).HasMaxLength(255);
    }
}
