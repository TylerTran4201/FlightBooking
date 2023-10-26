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
    public class TypeSeatsController : Controller
    {
        private readonly DataContext _context;

        public TypeSeatsController(DataContext context)
        {
            _context = context;
        }

        // GET: TypeSeats
        public async Task<IActionResult> Index()
        {
              return View(await _context.TypeSeats.ToListAsync());
        }

        // GET: TypeSeats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TypeSeats == null)
            {
                return NotFound();
            }

            var typeSeat = await _context.TypeSeats
                .FirstOrDefaultAsync(m => m.Id == id);
            if (typeSeat == null)
            {
                return NotFound();
            }

            return View(typeSeat);
        }

        // GET: TypeSeats/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TypeSeats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price")] TypeSeat typeSeat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(typeSeat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(typeSeat);
        }

        // GET: TypeSeats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TypeSeats == null)
            {
                return NotFound();
            }

            var typeSeat = await _context.TypeSeats.FindAsync(id);
            if (typeSeat == null)
            {
                return NotFound();
            }
            return View(typeSeat);
        }

        // POST: TypeSeats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price")] TypeSeat typeSeat)
        {
            if (id != typeSeat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(typeSeat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TypeSeatExists(typeSeat.Id))
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
            return View(typeSeat);
        }

        // GET: TypeSeats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TypeSeats == null)
            {
                return NotFound();
            }

            var typeSeat = await _context.TypeSeats
                .FirstOrDefaultAsync(m => m.Id == id);
            if (typeSeat == null)
            {
                return NotFound();
            }

            return View(typeSeat);
        }

        // POST: TypeSeats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TypeSeats == null)
            {
                return Problem("Entity set 'DataContext.TypeSeats'  is null.");
            }
            var typeSeat = await _context.TypeSeats.FindAsync(id);
            if (typeSeat != null)
            {
                _context.TypeSeats.Remove(typeSeat);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TypeSeatExists(int id)
        {
          return _context.TypeSeats.Any(e => e.Id == id);
        }
    }
}
