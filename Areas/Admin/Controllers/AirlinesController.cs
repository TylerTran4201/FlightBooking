using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlightBooking.Data;
using FlightBooking.Models;
using FlightBooking.Helpers;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AirlinesController : Controller
    {
        private readonly DataContext _context;

        public AirlinesController(DataContext context)
        {
            _context = context;
        }
        // GET: Airlines
        public async Task<IActionResult> Index(string searchString, int? page, string sortOrder, string searchOption)
        {
            if (_context.Airlines == null)
                return Problem("Entity set Airlines  is null.");

            ViewData["NameSort"] = String.IsNullOrEmpty(sortOrder) || !String.Equals(sortOrder, "Name_desc") ? "Name_desc": "Name_asc";
            ViewData["AirlineCompanySort"] = String.IsNullOrEmpty(sortOrder)|| !String.Equals(sortOrder, "AirlineCompany_desc") ? "AirlineCompany_desc": "AirlineCompany_asc";            
            var airlines = from a in _context.Airlines.Include(a => a.Seats).Include(a => a.AirlineCompany) select a ;

            airlines = await Searching(airlines, searchString, searchOption);

            airlines = await Sorting(airlines, sortOrder);

            var pagedList = new PagedList(page ?? 1, airlines.Count(), 6);
            ViewBag.PagedList = pagedList;

            return View(await airlines.Skip(pagedList.Start).Take(pagedList.Limit).ToListAsync());

        }
        public async Task<IQueryable<Airline>> Sorting(IQueryable<Airline> airlines,string sortOrder){
            switch(sortOrder){
                case "Name_desc": airlines = airlines.OrderByDescending(a => a.Name); break;
                case "Name_asc": airlines = airlines.OrderBy(a => a.Name); break;
                case "IATA_desc": airlines = airlines.OrderByDescending(a => a.AirlineCompany.Name); break;
                case "IATA_asc": airlines = airlines.OrderBy(a => a.AirlineCompany.Name); break;
            }
            return airlines;
        } 
        public async Task<IQueryable<Airline>> Searching(IQueryable<Airline> airlines, string searchString, string searchOption){
            if (!String.IsNullOrEmpty(searchString)){
                switch(searchOption){
                    case "AirlineName": airlines = airlines.Where(a => a.Name!.Contains(searchString)); break;
                    case "AirlineCompanyName": airlines = airlines.Where(a => a.AirlineCompany.Name!.Contains(searchString)); break;
                }
            }
            return airlines;
        }                  

        // GET: Airlines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Airlines == null)
            {
                return NotFound();
            }

            var airline = await _context.Airlines
                .Include(a => a.AirlineCompany)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (airline == null)
            {
                return NotFound();
            }

            return View(airline);
        }

        // GET: Airlines/Create
        public IActionResult Create()
        {
            ViewData["AirlineComanyId"] = new SelectList(_context.AirlineCompanies, "Id", "Name");
            return View();
        }

        // POST: Airlines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,AirlineComanyId")] Airline airline)
        {
            if (ModelState.IsValid)
            {
                _context.Add(airline);
                var typeSeats = _context.TypeSeats.ToArray();
                await _context.SaveChangesAsync();

                // Create Business Seat
                for (int i = 1; i <= 20; i++)
                {
                    var seat = new Seat
                    {
                        Name = "Bu" + i,
                        AirlineId = airline.Id,
                        TypeSeatId = typeSeats[0].Id
                    };
                    _context.Add(seat);
                }

                // Create Economy Seat
                for (int i = 1; i <= 40; i++)
                {
                    var seat = new Seat
                    {
                        Name = "Eco" + i,
                        AirlineId = airline.Id,
                        TypeSeatId = typeSeats[1].Id
                    };
                    _context.Add(seat);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AirlineComanyId"] = new SelectList(_context.AirlineCompanies, "Id", "Name", airline.AirlineComanyId);
            return View(airline);
        }

        // GET: Airlines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Airlines == null)
            {
                return NotFound();
            }

            var airline = await _context.Airlines.FindAsync(id);
            if (airline == null)
            {
                return NotFound();
            }
            ViewData["AirlineComanyId"] = new SelectList(_context.AirlineCompanies, "Id", "Name", airline.AirlineComanyId);
            return View(airline);
        }

        // POST: Airlines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,AirlineComanyId")] Airline airline)
        {
            if (id != airline.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(airline);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AirlineExists(airline.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AirlineComanyId"] = new SelectList(_context.AirlineCompanies, "Id", "Name", airline.AirlineComanyId);
            return View(airline);
        }

        // GET: Airlines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Airlines == null)
            {
                return NotFound();
            }

            var airline = await _context.Airlines
                .Include(a => a.AirlineCompany)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (airline == null)
            {
                return NotFound();
            }

            return View(airline);
        }

        // POST: Airlines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Airlines == null)
            {
                return Problem("Entity set 'DataContext.Airlines'  is null.");
            }
            var airline = await _context.Airlines.FindAsync(id);
            if (airline != null)
            {
                _context.Airlines.Remove(airline);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AirlineExists(int id)
        {
            return _context.Airlines.Any(e => e.Id == id);
        }
    }
}
