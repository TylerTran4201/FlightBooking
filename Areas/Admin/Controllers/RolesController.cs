using FlightBooking.Data;
using FlightBooking.Helpers;
using FlightBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController: Controller
    {
        private readonly DataContext _context;

        public RolesController(DataContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? page, string sortOrder, string searchString)
        {
            ViewData["RolenameSort"] = String.IsNullOrEmpty(sortOrder) || !String.Equals(sortOrder, "RoleName_desc") ? "RoleName_desc": "RoleName_asc";
            var roles = from a in _context.Roles select a;
            
            if (!String.IsNullOrEmpty(searchString)){
                searchString.Replace('+', ' ');
                roles = roles.Where(a => a.Name!.Contains(searchString));
            }

            roles = await Sorting(roles, sortOrder);
            var pagedList = new PagedList(page ?? 1, roles.Count(), 10);
            ViewBag.PagedList = pagedList;
            return View(await roles.Skip(pagedList.Start).Take(pagedList.Limit).ToListAsync());
        }
        public async Task<IQueryable<AppRole>> Sorting(IQueryable<AppRole> appRoles,string sortOrder){
            switch(sortOrder){
                case "RoleName_desc": appRoles = appRoles.OrderByDescending(a => a.Name); break;
                case "RoleName_asc": appRoles = appRoles.OrderBy(a => a.Name); break;
            }
            return appRoles;
        }  
    }
}