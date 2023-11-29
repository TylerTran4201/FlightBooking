using FlightBooking.Data;
using FlightBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExportReportController : Controller
    {
        public DataContext _context { get; }

        public ExportReportController(DataContext context)
        {
            _context = context;

        }

        public async Task<IActionResult> Index(string searchString, string searchOption, string fromDate, string toDate)
        {

            var bills= from b in _context.Bills
            .Include(b => b.Booking)
                .ThenInclude(a => a.Schedule)
                .ThenInclude(s => s.DepartureAirport)
            .Include(b => b.Booking)
                .ThenInclude(a => a.Schedule)
                .ThenInclude(s => s.DestinationAirport)
            .Include(b => b.Booking)
                .ThenInclude(a => a.Schedule)
                .ThenInclude(s => s.Airline)
                .ThenInclude(al => al.AirlineCompany)
                .ThenInclude(p => p.Photo)
            select b;


            bills = await Searching(bills, searchString, searchOption);
            if (fromDate != null && toDate != null)
            {
                bills = await datefilter(bills, DateTime.Parse(fromDate), DateTime.Parse(toDate));
            }

            
            ViewBag.Bills = bills.ToList();
            ViewBag.TotalPrice = bills.Sum(b => b.TotalPrice);
            ViewBag.TotalTransaction = bills.Count();

            return View();
        }

        public async Task<IQueryable<Bill>> datefilter(IQueryable<Bill> bills, DateTime fromDate, DateTime toDate)
        {
            return bills.Where(b => b.Created > fromDate && b.Created < toDate.AddDays(1));
        }
        public async Task<IQueryable<Bill>> Searching(IQueryable<Bill> bills, string searchString, string searchOption)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchOption)
                {
                    case "Airline": bills = bills.Where(b => b.Booking.Schedule.Airline.AirlineCompany.Name!.Contains(searchString)); break;
                    case "Schedule": bills = bills.Where(b => (b.Booking.Schedule.DepartureAirport.name + " - " + b.Booking.Schedule.DepartureAirport.name)!.Contains(searchString)); break;
                }
            }
            return bills;
        }
    }
}