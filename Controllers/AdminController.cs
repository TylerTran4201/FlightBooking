using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers
{
    public class AdminController: Controller
    {
        public AdminController()
        {
        
        }
        public IActionResult Index() {
            return View();
        }
    }
}
