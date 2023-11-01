using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightBooking.Data;
using FlightBooking.Models;
using FlightBooking.Helpers;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TypeSeatsController : Controller
    {
        private readonly DataContext _context;

        public TypeSeatsController(DataContext context)
        {
            _context = context;
        }

        // GET: TypeSeats
        public async Task<IActionResult> Index(int? page, string sortOrder)
        {
            var typeSeats = from a in _context.TypeSeats select a;
            ViewData["NameSort"] = String.IsNullOrEmpty(sortOrder) || !String.Equals(sortOrder, "Name_desc") ? "Name_desc": "Name_asc";
            ViewData["PriceSort"] = String.IsNullOrEmpty(sortOrder)|| !String.Equals(sortOrder, "Price_desc") ? "Price_desc": "Price_asc";

            typeSeats = await Sorting(typeSeats, sortOrder);

            var pagedList =  new PagedList(page ?? 1, typeSeats.Count(), 10);
            ViewBag.PagedList = pagedList;
            return View(await typeSeats.Skip(pagedList.Start).Take(pagedList.Limit).ToListAsync());
        }

        public async Task<IQueryable<TypeSeat>> Sorting(IQueryable<TypeSeat> typeSeats,string sortOrder){
            switch(sortOrder){
                case "Name_desc": typeSeats = typeSeats.OrderByDescending(a => a.Name); break;
                case "Name_asc": typeSeats = typeSeats.OrderBy(a => a.Name); break;
                case "Price_desc": typeSeats = typeSeats.OrderByDescending(a => a.Price); break;
                case "Price_asc": typeSeats = typeSeats.OrderBy(a => a.Price); break;
            }
            return typeSeats;
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
