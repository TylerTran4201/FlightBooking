using FlightBooking.Models;

namespace FlightBooking.Interface
{
    public interface IAirportRepository
    {
        Task<IEnumerable<Airport>> GetAirports();
    }
}