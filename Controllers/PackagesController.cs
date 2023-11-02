using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers
{
    [Authorize]
    public class PackagesController: Controller
    {
        public PackagesController()
        {

        }

        [Authorize(Policy = "UsersView")]
        public IActionResult Index(){
            return View();
        }  
    }
}