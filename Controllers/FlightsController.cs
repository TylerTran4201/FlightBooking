using FlightBooking.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers
{
    public class FlightsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public FlightsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Airports = await _unitOfWork.airportRepository.GetAirports();
            return View();
        }
        public IActionResult SearchFlight(){
            return View();
        }
        //get Airports from API
        //private async void GetAirPorts()
        //{
        //    var client = new HttpClient();
        //    var request = new HttpRequestMessage
        //    {
        //        Method = HttpMethod.Get,
        //        RequestUri = new Uri("https://flight-radar1.p.rapidapi.com/airports/list"),
        //        Headers = {
        //            { "X-RapidAPI-Key", "3bfa4b8116msh7409148eb912242p1b082ajsndf2ebc3f8187" },
        //            { "X-RapidAPI-Host", "flight-radar1.p.rapidapi.com" },
        //        }
        //    };
        //    using (var response = await client.SendAsync(request)) {
        //        response.EnsureSuccessStatusCode();
        //        var body = await response.Content.ReadAsStringAsync();
        //        Console.WriteLine(body);
        //        var result = JsonConvert.DeserializeObject<List<Airports>>(body);
        //        Console.WriteLine(result);
        //    }
        //}
    }
}