using FlightBooking.Data;

namespace FlightBooking.Repository
{
    public class AirlineRepository
    {
                private readonly DataContext _context;

        public AirlineRepository(DataContext context)
        {
            _context = context;
        }
    }
}