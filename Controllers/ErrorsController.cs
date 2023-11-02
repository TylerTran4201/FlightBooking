using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers
{
    public class ErrorsController:Controller
    {
        public ErrorsController()
        {
        }

        public IActionResult AccessDenied(){
            return View();
        } 
        public IActionResult PageNotFound(){
            return View();
        } 
    }
}