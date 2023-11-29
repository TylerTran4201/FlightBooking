using FlightBooking.Data;
using FlightBooking.DTOs;
using FlightBooking.Interface;
using FlightBooking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace FlightBooking.Controllers
{
    public class FlightsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;

        public FlightsController(IUnitOfWork unitOfWork, DataContext context, UserManager<AppUser> userManager )
        {
            _userManager = userManager;
            _context = context;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Airports = await _context.Airports.ToListAsync();
            return View();
        }
        [BindProperty]
        public SearchFlightDto SearchFlightDto { get; set; }

        public async Task<IActionResult> SearchFlight(){
            if(ModelState.IsValid){
                if (_context.Schedules == null)
                    return Problem("Entity set Schedules is null.");
                var schedules = from s in _context.Schedules.Include(s => s.DepartureAirport).Include(s => s.DestinationAirport).Include(s=>s.Airline).ThenInclude(a => a.AirlineCompany).ThenInclude(ac => ac.Photo) select s;

                schedules = schedules.Where(s => s.DepartureAirport.name.CompareTo(SearchFlightDto.Departure) == 0
                    && s.DestinationAirport.name.CompareTo(SearchFlightDto.Destination) == 0);

                var filteredSchedules = schedules.AsEnumerable()
                    .Where(s => DateOnly.FromDateTime(s.DepartureTime) == SearchFlightDto.FlightDate).ToList();
                ViewBag.BusinessPrice = Math.Round((double) _context.TypeSeats.Where(p => p.Id == 1).Select(p => p.Price).FirstOrDefault(), 1);

                var temp = _context.Airports.Where(a => a.name.CompareTo(SearchFlightDto.Departure) == 0).FirstOrDefault();
                var departureSelect = _context.Airports.Where(a=>a.name.CompareTo(SearchFlightDto.Departure) == 0).FirstOrDefault();
                var destinationsSelect = _context.Airports.Where(a=>a.name.CompareTo(SearchFlightDto.Destination) == 0).FirstOrDefault();

                ViewBag.NumOfTickets = SearchFlightDto.NumOfTickets;
                return View(filteredSchedules);                
            }return BadRequest("test");

        }
        [Authorize(Roles = "Member")]
        public IActionResult PassengerInfomation(int scheduleId, int numtickets){
            AddViewBag(scheduleId, numtickets);
            return View();
        }

        
        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> PassengerInfomation([FromForm] List<PassengerInformationDto> passengerDto, int scheduleId, int numtickets){
            
            AddViewBag(scheduleId, numtickets);
            ViewBag.PassengerInfomation = passengerDto;
            foreach(var item in passengerDto){
                if(item.FullName == "" || item.IdCard == "" || item.FullName == null || item.IdCard == null)
                {
                    ViewBag.PassengerInfomation = null;
                    return View();
                }   
            }
            var user = await _context.Users.Where(u => u.UserName.CompareTo( _userManager.GetUserName(User)) == 0).FirstOrDefaultAsync();
            double newPassengerInforTempId = double.Parse(Regex.Replace(user.Id.ToString() + DateTime.UtcNow.ToString(), "[^.0-9]", ""));
            GlobalVariables.PassengerInformationDtos = passengerDto;
            return View();
        }
        private void AddViewBag(int scheduleId, int numtickets)
        {
            ViewBag.Schedule = _context.Schedules.Include(s => s.Airline).ThenInclude(a => a.AirlineCompany).ThenInclude(c => c.Photo).Where(s => s.Id == scheduleId).FirstOrDefault();
            ViewBag.NumOfTickets = numtickets;
        }
    }
}