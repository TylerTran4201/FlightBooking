using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightBooking.Areas.Identity.Data;
using FlightBooking.Interface;

namespace FlightBooking.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;

        public UnitOfWork(DataContext _context)
        {
            this._context = _context;
            
        }
        public IAirportRepository airportRepository => new AirportRepository(_context);
        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}