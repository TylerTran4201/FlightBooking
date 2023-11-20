using FlightBooking.Data;
using FlightBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Controllers
{
    public class SelectSeatsController:Controller
    {
        public DataContext _context { get; }
        public SelectSeatsController(DataContext _context)
        {
            this._context = _context;
        }
        public async Task<IActionResult> Index(int scheduleId, int numticket){
            var schedule = _context.Schedules.Include(s => s.Airline).ThenInclude(a => a.Seats)
                .Include(s => s.DepartureAirport)
                .Include(s => s.DestinationAirport)
                .Where(s => s.Id == scheduleId).FirstOrDefault();

            // Convert dd/mm to mm/dd
            var departureTime = DateTime.ParseExact(schedule.DepartureTime.ToString(), "dd/MM/yyyy h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
            var destinationTime = DateTime.ParseExact(schedule.DestinationTime.ToString(), "dd/MM/yyyy h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);

            ViewBag.Schedule = schedule;
            ViewBag.DepartureTime = departureTime.ToString("MMMM d | h:mm tt");
            ViewBag.DestinationTime = destinationTime.ToString("MMMM d | h:mm tt");

            ViewBag.BusinessSeats = schedule.Airline.Seats.Where(s => s.TypeSeatId == 1).ToArray();
            ViewBag.EconomySeats = schedule.Airline.Seats.Where(s => s.TypeSeatId == 2).ToArray();
            ViewBag.NumTicket = numticket;
            return View();
        }
    }
}