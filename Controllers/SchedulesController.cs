using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlightBooking.Areas.Identity.Data;
using FlightBooking.Models;
using FlightBooking.Helpers;

namespace FlightBooking.Controllers
{
    public class SchedulesController : Controller
    {
        private readonly DataContext _context;

        public SchedulesController(DataContext context)
        {
            _context = context;
        }

        // GET: Schedules
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Schedules.Include(s => s.DepartureAirport).Include(s => s.DestinationAirport);
            return View(await dataContext.ToListAsync());
        }

        // GET: Schedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Schedules == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.DepartureAirport)
                .Include(s => s.DestinationAirport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }
        // GET: Schedules/Create
        public IActionResult Create()
        {
            ViewData["DepartureAirport"] = new SelectList(_context.Airports, "Id", "name");
            ViewData["DestinationAirport"] = new SelectList(_context.Airports, "Id", "name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DestinationAirportId,DepartureAirportId,DepartureTime,DestinationTime")] Schedule schedule, int hours, int minutes)
        {
            if (ModelState.IsValid)
            {
                schedule.FlightTime = new TimeSpan(hours, minutes, 0);
                _context.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartureAirport"] = new SelectList(_context.Airports, "Id", "name", schedule.DepartureAirport);
            ViewData["DestinationAirport"] = new SelectList(_context.Airports, "Id", "name", schedule.DestinationAirport);

            return View(schedule);
        }
        // GET: Schedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Schedules == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules.Include(x => x.DepartureAirport).Include(x => x.DestinationAirport).FirstOrDefaultAsync(x => x.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }


            ViewData["TimeFlight"] = new
            {
                Hour = schedule.FlightTime.Hours,
                Minutes = schedule.FlightTime.Hours
            };
            ViewData["DestinationAirport"] = new SelectList(_context.Airports, "Id", "name", schedule.DestinationAirport);
            ViewData["DepartureAirport"] = new SelectList(_context.Airports, "Id", "name", schedule.DepartureAirport);
            return View(schedule);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DestinationAirportId,DepartureAirportId,DepartureTime,DestinationTime")] Schedule schedule, int hours, int minutes)
        {
            if (id != schedule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    schedule.FlightTime = new TimeSpan(hours, minutes, 0);
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.Id))
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
            ViewData["DepartureAirport"] = new SelectList(_context.Airports, "Id", "Name", schedule.DepartureAirport);
            ViewData["DestinationAirport"] = new SelectList(_context.Airports, "Id", "Name", schedule.DestinationAirport);
            return View(schedule);
        }

        // GET: Schedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Schedules == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.DepartureAirport)
                .Include(s => s.DestinationAirport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: Schedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Schedules == null)
            {
                return Problem("Entity set 'DataContext.Schedules'  is null.");
            }
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedules.Any(e => e.Id == id);
        }

        [HttpPost]
        public IActionResult CalculateDestinationTime(int hours, int minutes, string departureTime)
        {
            var temp = new TimeSpan(hours, minutes, 0);
            var test = DateTimeConverter.GetDateTimeFromString(departureTime);
            DateTime final = Schedule.GetDestinationTime(test, temp);
            return Json(new { final });
        }
    }
}
