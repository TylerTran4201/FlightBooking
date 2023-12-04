using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightBooking.Data;

namespace FlightBooking.Repository
{
    public class FlightRepository
    {
                private readonly DataContext _context;

        public FlightRepository(DataContext context)
        {
            _context = context;
        }
    }
}