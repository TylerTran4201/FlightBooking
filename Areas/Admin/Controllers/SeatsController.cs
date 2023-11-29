using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightBooking.Data;
using FlightBooking.Models;
using FlightBooking.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SeatsController : Controller
    {
        private readonly DataContext _context;

        public SeatsController(DataContext context)
        {
            _context = context;
        }
        // GET: Seats
        [Authorize(Policy = "SeatsView")]
        public async Task<IActionResult> Index(int? page, string sortOrder, string searchString)
        {
            ViewData["AirlineNameSort"] = String.IsNullOrEmpty(sortOrder) || !String.Equals(sortOrder, "AirlineName_desc") ? "AirlineName_desc": "AirlineName_asc";
            var airlines = from a in _context.Airlines.Include(a=> a.Seats).ThenInclude(s => s.TypeSeat) select a;
            if (!String.IsNullOrEmpty(searchString)){
                searchString.Replace('+', ' ');
                airlines = airlines.Where(a => a.Name!.Contains(searchString));
            }
            airlines = await Sorting(airlines, sortOrder);
            var pagedList = new PagedList(page ?? 1, airlines.Count(), 10);
            ViewBag.PagedList = pagedList;
            return View(await airlines.Skip(pagedList.Start).Take(pagedList.Limit).ToListAsync());
        }
        // GET: Seats/Details/5
        public async Task<IQueryable<Airline>> Sorting(IQueryable<Airline> airlines,string sortOrder){
            switch(sortOrder){
                case "AirlineName_desc": airlines = airlines.OrderByDescending(a => a.Name); break;
                case "AirlineName_asc": airlines = airlines.OrderBy(a => a.Name); break;
            }
            return airlines;
        }      
        [Authorize(Policy = "SeatsView")]  
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Seats == null)
            {
                return NotFound();
            }

            var seat = await _context.Seats
                .Include(s => s.Airline)
                .Include(s => s.TypeSeat)
                .Include(s => s.Airline)
                .ThenInclude(a => a.AirlineCompany)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seat == null)
            {
                return NotFound();
            }

            return View(seat);
        }
    }
}