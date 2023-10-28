using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController: Controller
    {
        public IActionResult Index() {
            ViewBag.LinkActive = true;
            return View();
        }
    }
}