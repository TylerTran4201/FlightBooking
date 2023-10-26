using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlightBooking.Areas.Identity.Data;
using FlightBooking.Models;

namespace FlightBooking.Controllers
{
    public class AirlineCompaniesController : Controller
    {
        private readonly DataContext _context;

        public AirlineCompaniesController(DataContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(string searchString)
        {
            if (_context.AirlineCompanies == null)
                return Problem("Entity set Airline Companies  is null.");

            var airlineCompanies = from a in _context.AirlineCompanies select a;

            if (!String.IsNullOrEmpty(searchString))
                airlineCompanies = airlineCompanies.Where(a => a.Name!.Contains(searchString));

            return View(await airlineCompanies.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
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

        // GET: AirlineCompanies/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] AirlineCompany airlineCompany)
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
