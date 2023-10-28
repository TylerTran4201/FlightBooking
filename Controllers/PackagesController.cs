using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers
{
    public class PackagesController: Controller
    {
        public IActionResult Index(){
            return View();
        }
    }
}