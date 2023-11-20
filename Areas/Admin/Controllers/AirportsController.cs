using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightBooking.Data;
using FlightBooking.Models;
using FlightBooking.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AirportsController : Controller
    {
        private readonly DataContext _context;

        public AirportsController(DataContext context)
        {
            _context = context;
        }
        // GET: Airports
        public async Task<IActionResult> Index(string searchString, int? page, string sortOrder, string searchOption)
        {
            ViewData["NameSort"] = String.IsNullOrEmpty(sortOrder) || !String.Equals(sortOrder, "Name_desc") ? "Name_desc": "Name_asc";
            ViewData["IATASort"] = String.IsNullOrEmpty(sortOrder)|| !String.Equals(sortOrder, "IATA_desc") ? "IATA_desc": "IATA_asc";
            ViewData["ICAOSort"] = String.IsNullOrEmpty(sortOrder) || !String.Equals(sortOrder, "ICAO_desc") ? "ICAO_desc": "ICAO_asc";
            ViewData["CITYSort"] = String.IsNullOrEmpty(sortOrder) || !String.Equals(sortOrder, "CITY_desc") ? "CITY_desc": "CITY_asc";


            if (_context.Airports == null)
                return Problem("Entity set Airports is null.");
            
            var airports = from a in _context.Airports select a;
            airports = await Searching(airports, searchString, searchOption);

            airports = await Sorting(airports, sortOrder);
            var pagedList = new PagedList(page ?? 1, airports.Count(), 10);
            ViewBag.PagedList = pagedList;

            return View(await airports.Skip(pagedList.Start).Take(pagedList.Limit).ToListAsync());
        }
        public async Task<IQueryable<Airport>> Sorting(IQueryable<Airport> airports,string sortOrder){
            switch(sortOrder){
                case "Name_desc": airports = airports.OrderByDescending(a => a.name); break;
                case "Name_asc": airports = airports.OrderBy(a => a.name); break;
                case "IATA_desc": airports = airports.OrderByDescending(a => a.iata); break;
                case "IATA_asc": airports = airports.OrderBy(a => a.iata); break;
                case "ICAO_desc": airports = airports.OrderByDescending(a => a.icao); break;
                case "ICAO_asc": airports = airports.OrderBy(a => a.icao); break;
                case "CITY_desc": airports = airports.OrderByDescending(a => a.city); break;
                case "CITY_asc": airports = airports.OrderBy(a => a.city); break;
            }
            return airports;
        }
        public async Task<IQueryable<Airport>> Searching(IQueryable<Airport> airports, string searchString, string searchOption){
            if (!String.IsNullOrEmpty(searchString)){
                switch(searchOption){
                    case "Name": airports = airports.Where(a => a.name!.Contains(searchString)); break;
                    case "IATA": airports = airports.Where(a => a.iata!.Contains(searchString)); break;
                    case "ICAO": airports = airports.Where(a => a.icao!.Contains(searchString)); break;
                    case "City": airports = airports.Where(a => a.city!.Contains(searchString)); break;
                }
            }
            return airports;
        }

        // GET: Airports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Airports == null)
            {
                return NotFound();
            }

            var airport = await _context.Airports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (airport == null)
            {
                return NotFound();
            }

            return View(airport);
        }
    }
}
