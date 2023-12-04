using FlightBooking.Data;
using FlightBooking.Interface;
using FlightBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Repository
{
    public class AirportRepository : IAirportRepository
    {
        private readonly DataContext _context;

        public AirportRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Airport>> GetAirports()
        {
            return await _context.Airports.ToListAsync();
        }
        public async Task<IEnumerable<Airport>> DeleteAirport(int id){
            int test = id;
            return await _context.Airports.ToListAsync();
        }
        public async Task<IEnumerable<Airport>> UpdateAirport(int id){
            int test = id;
            return await _context.Airports.ToListAsync();
        }
        public async Task<IEnumerable<Airport>> AddAirport(int id){
            int test = id;
            return await _context.Airports.ToListAsync();
        }
    }
}