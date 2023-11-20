using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlightBooking.Data;
using FlightBooking.Models;
using FlightBooking.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SchedulesController : Controller
    {
        private readonly DataContext _context;

        public SchedulesController(DataContext context)
        {
            _context = context;
        }

        // GET: Schedules

        public async Task<IActionResult> Index(int? page, string searchString, string sortOrder, string searchOption)
        {
            ViewData["AirlineNameSort"] = String.IsNullOrEmpty(sortOrder) || !String.Equals(sortOrder, "AirlineName_desc") ? "AirlineName_desc": "AirlineName_asc";
            ViewData["FlightTimeSort"] = String.Equals(sortOrder, "Date")|| !String.Equals(sortOrder, "FlightTime_desc") ? "FlightTime_desc": "FlightTime_asc";

             if (_context.Schedules == null)
                return Problem("Entity set Schedules is null.");
            var schedules = from a in _context.Schedules.Include(s => s.DepartureAirport).Include(s => s.DestinationAirport).Include(s => s.Airline) select a ;

            schedules = await Searching(schedules, searchString, searchOption);

            schedules = await Sorting(schedules, sortOrder);
            var pagedList = new PagedList(page ?? 1, schedules.Count(), 6);
            ViewBag.PagedList = pagedList;
            return View(await schedules.Skip(pagedList.Start).Take(pagedList.Limit).ToListAsync());
        }
        public async Task<IQueryable<Schedule>> Sorting(IQueryable<Schedule> schedules, string sortOrder)
        {
            switch (sortOrder)
            {
                case "AirlineName_desc": schedules = schedules.OrderByDescending(a => a.Airline.Name); break;
                case "AirlineName_asc": schedules = schedules.OrderBy(a => a.Airline.Name); break;
                case "FlightTime_desc": schedules = schedules.OrderByDescending(a => a.FlightTime); break;
                case "FlightTime_asc": schedules = schedules.OrderBy(a => a.FlightTime); break;
            }
            return schedules;
        }

        public async Task<IQueryable<Schedule>> Searching(IQueryable<Schedule> schedules, string searchString, string searchOption){
            if (!String.IsNullOrEmpty(searchString)){
                switch(searchOption){
                    case "AirlineName": schedules = schedules.Where(a => a.Airline.Name!.Contains(searchString)); break;
                    case "DepartureName": schedules = schedules.Where(a => a.DepartureAirport.name!.Contains(searchString)); break;
                    case "DestinationName": schedules = schedules.Where(a => a.DestinationAirport.name!.Contains(searchString)); break;
                }
            }
            return schedules;
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
                .Include(s => s.Airline)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }
            ViewBag.FlightTime = schedule.FlightTime.Hours + " hours and " + schedule.FlightTime.Minutes + " minutes";
            return View(schedule);
        }
        // GET: Schedules/Create
        public IActionResult Create()
        {
            ViewData["Airlines"] = new SelectList(_context.Airlines.Where(x=> x.Status == false), "Id", "Name");
            ViewData["DepartureAirport"] = new SelectList(_context.Airports, "Id", "name");
            ViewData["DestinationAirport"] = new SelectList(_context.Airports, "Id", "name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DestinationAirportId,DepartureAirportId,DepartureTime,DestinationTime,AirlineId")] Schedule schedule, int hours, int minutes)
        {
            if (ModelState.IsValid)
            {

                schedule.FlightTime = new TimeSpan(hours, minutes, 0);
                _context.Add(schedule);
                await _context.SaveChangesAsync();

                var airline = _context.Airlines.Find(schedule.AirlineId);
                airline.Status = true;
                airline.ScheduleId = schedule.Id;
                _context.Update(airline);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartureAirport"] = new SelectList(_context.Airports, "Id", "name", schedule.DepartureAirport);
            ViewData["DestinationAirport"] = new SelectList(_context.Airports, "Id", "name", schedule.DestinationAirport);
            ViewData["Airlines"] = new SelectList(_context.Airlines.Where(x=> x.Status == false), "Id", "Name", schedule.Airline);

            return View(schedule);
        }
        // GET: Schedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Schedules == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules.Include(x => x.DepartureAirport).Include(x => x.DestinationAirport).Include(x => x.Airline).FirstOrDefaultAsync(x => x.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }


            ViewData["TimeFlight"] = new
            {
                Hour = schedule.FlightTime.Hours,
                Minutes = schedule.FlightTime.Hours
            };
            var airlines = await _context.Airlines.Where(a => a.Status == false).ToListAsync();
            airlines.Add(schedule.Airline);
            ViewData["Airlines"] = new SelectList(airlines, "Id", "Name", schedule.Airline);
            ViewData["DestinationAirport"] = new SelectList(_context.Airports, "Id", "name", schedule.DestinationAirport);
            ViewData["DepartureAirport"] = new SelectList(_context.Airports, "Id", "name", schedule.DepartureAirport);
            return View(schedule);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DestinationAirportId,DepartureAirportId,DepartureTime,DestinationTime, AirlineId")] Schedule schedule, int hours, int minutes)
        {
            if (id != schedule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var airlineOff = _context.Airlines.Where(a => a.ScheduleId == schedule.Id).FirstOrDefault();

                    if (airlineOff.Id != schedule.AirlineId)
                    {
                        var airlineOn = _context.Airlines.Find(schedule.AirlineId);
                        airlineOff.Status = false;
                        airlineOn.Status = true;
                        _context.Update(airlineOff);
                        _context.Update(airlineOn);
                    }

                    
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
            var airlines = await _context.Airlines.Where(a => a.Status == false).ToListAsync();
            airlines.Add(schedule.Airline);
            ViewData["Airlines"] = new SelectList(airlines, "Id", "Name", schedule.Airline);
            ViewData["DestinationAirport"] = new SelectList(_context.Airports, "Id", "name", schedule.DestinationAirport);
            ViewData["DepartureAirport"] = new SelectList(_context.Airports, "Id", "name", schedule.DepartureAirport);
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
                var airlineOff = _context.Airlines.Where(a => a.ScheduleId == schedule.Id).FirstOrDefault();
                airlineOff.Status = false;
                _context.Airlines.Update(airlineOff);
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
