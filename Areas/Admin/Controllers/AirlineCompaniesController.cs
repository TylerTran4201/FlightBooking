using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightBooking.Data;
using FlightBooking.Models;
using FlightBooking.Helpers;

namespace FlightBooking.Areas.Admin.Controllers
{ 
    [Area("Admin")]
    public class AirlineCompaniesController : Controller
    {
        private readonly DataContext _context;

        public AirlineCompaniesController(DataContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(string searchString, int? page, string sortOrder)
        {
            if (_context.AirlineCompanies == null)
                return Problem("Entity set Airline Companies  is null.");

            ViewData["NameSort"] = String.IsNullOrEmpty(sortOrder) || !String.Equals(sortOrder, "Name_desc") ? "Name_desc": "Name_asc";     

            var airlineCompanies = from a in _context.AirlineCompanies select a;

            if (!String.IsNullOrEmpty(searchString))
                airlineCompanies = airlineCompanies.Where(a => a.Name!.Contains(searchString));

            airlineCompanies = await Sorting(airlineCompanies, sortOrder);
            var pagedList = new PagedList(page ?? 1, airlineCompanies.Count(), 6);
            ViewBag.PagedList = pagedList;

            return View(await airlineCompanies.Skip(pagedList.Start).Take(pagedList.Limit).ToListAsync());
        }
        public async Task<IQueryable<AirlineCompany>> Sorting(IQueryable<AirlineCompany> airlineCompanies,string sortOrder){
            switch(sortOrder){
                case "Name_desc": airlineCompanies = airlineCompanies.OrderByDescending(a => a.Name); break;
                case "Name_asc": airlineCompanies = airlineCompanies.OrderBy(a => a.Name); break;
            }
            return airlineCompanies;
        }

        // GET: AirlineCompanies/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] AirlineCompany airlineCompany, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                _context.Add(airlineCompany);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(airlineCompany);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AirlineCompanies == null)
            {
                return NotFound();
            }

            var airlineCompany = await _context.AirlineCompanies.FindAsync(id);
            if (airlineCompany == null)
            {
                return NotFound();
            }
            return View(airlineCompany);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] AirlineCompany airlineCompany)
        {
            if (id != airlineCompany.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(airlineCompany);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AirlineCompanyExists(airlineCompany.Id))
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
            return View(airlineCompany);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AirlineCompanies == null)
            {
                return NotFound();
            }

            var airlineCompany = await _context.AirlineCompanies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (airlineCompany == null)
            {
                return NotFound();
            }

            return View(airlineCompany);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AirlineCompanies == null)
            {
                return Problem("Entity set 'DataContext.AirlineCompanies'  is null.");
            }
            var airlineCompany = await _context.AirlineCompanies.FindAsync(id);
            if (airlineCompany != null)
            {
                _context.AirlineCompanies.Remove(airlineCompany);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AirlineCompanyExists(int id)
        {
            return _context.AirlineCompanies.Any(e => e.Id == id);
        }
    }
}
