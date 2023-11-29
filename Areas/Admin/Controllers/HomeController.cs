using FlightBooking.Data;
using FlightBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public DataContext _context { get; }

        public HomeController(DataContext context)
        {
            _context = context;

        }
        public async Task<IActionResult> Index(string searchString, string searchOption, string fromDate, string toDate)
        {
            if (_context.Bills == null)
                return Problem("Entity set Bills is null.");

            var bills= from b in _context.Bills
                .Include(b => b.Booking)
                    .ThenInclude(a => a.Schedule)
                    .ThenInclude(s => s.DepartureAirport)
                .Include(b => b.Booking)
                    .ThenInclude(a => a.Schedule)
                    .ThenInclude(s => s.DestinationAirport)
                .Include(b => b.Booking)
                    .ThenInclude(a => a.Schedule)
                    .ThenInclude(s => s.Airline)
                    .ThenInclude(al => al.AirlineCompany)
                    .ThenInclude(p => p.Photo)
                select b;
            bills = await Searching(bills, searchString, searchOption);
            if(fromDate != null &&  toDate != null){
                bills = await datefilter(bills, DateTime.Parse(fromDate), DateTime.Parse(toDate));
            }


            Chart chart1 = getChart1(bills.ToList());
            Chart chart2 = getChart2(bills.ToList());
            ViewBag.ChartLabel1 = chart1.Label;
            ViewBag.Data1 = chart1.Data;
            ViewBag.ChartLabel2 = chart2.Label;
            ViewBag.Data2 = chart2.Data;

            ViewBag.Bills = bills.ToList();
            ViewBag.TotalPrice = bills.Sum(b => b.TotalPrice);
            ViewBag.TotalTransaction = bills.Count();


            ViewBag.SearchString = searchString;
            ViewBag.SearchOption = searchOption;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return View();
        }

        public async Task<IQueryable<Bill>> datefilter(IQueryable<Bill> bills, DateTime fromDate, DateTime toDate)
        {
            return bills.Where(b => b.Created > fromDate && b.Created < toDate.AddDays(1)); 
        }
        public async Task<IQueryable<Bill>> Searching(IQueryable<Bill> bills, string searchString, string searchOption)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchOption)
                {
                    case "Airline": bills = bills.Where(b => b.Booking.Schedule.Airline.AirlineCompany.Name!.Contains(searchString)); break;
                    case "Schedule": bills = bills.Where(b => (b.Booking.Schedule.DepartureAirport.name + " - " + b.Booking.Schedule.DepartureAirport.name)!.Contains(searchString)); break;
                }
            }
            return bills;
        }
        public Chart getChart2(List<Bill> bills)
        {
            DateTime today = DateTime.Now;
            DateTime lastWeek = today.AddDays(-7);

            var label = bills
                .Where(item => item.Created >= lastWeek)  // Filter bills for the last 7 days
                .Select(item => item.Created.ToString("yyyy-MM-dd"))
                .Distinct()
                .ToList();

            var data = label.Select(date =>
            {
                var totalPrice = bills
                    .Where(bill => date.CompareTo(bill.Created.ToString("yyyy-MM-dd")) == 0)
                    .Sum(bill => bill.TotalPrice);
                return totalPrice.ToString();
            }).ToList();

            string labelFinal = string.Join(", ", label.Select(date => $"'{date}'"));
            string dataFinal = string.Join(", ", data);

            return new Chart
            {
                Label = labelFinal,
                Data = dataFinal
            };
        }
        public Chart getChart1(List<Bill> bills)
        {

            var label = bills.Select(item => item.Booking.Schedule.Airline.Name).Distinct().ToList();

            var data = label.Select(item => bills.Count(bill => item.Equals(bill.Booking.Schedule.Airline.Name))).ToList();

            string labelFinal = string.Join(", ", label.Select(airline => $"'{airline}'"));
            string dataFinal = string.Join(", ", data);

            return new Chart
            {
                Label = labelFinal,
                Data = dataFinal
            };
        }
    }
}