using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightBooking.Data;
using FlightBooking.Models;
using FlightBooking.Helpers;
using FlightBooking.Interface;
using Microsoft.AspNetCore.Authorization;

namespace FlightBooking.Areas.Admin.Controllers
{ 
    [Area("Admin")]
    public class AirlineCompaniesController : Controller
    {
        private readonly DataContext _context;
        private readonly IPhotoServices _photoService;

        public AirlineCompaniesController(DataContext context, IPhotoServices photoService)
        {
            _photoService = photoService;
            _context = context;
        }

        [Authorize(Policy = "AirlineCompaniesView")]
        public async Task<IActionResult> Index(string searchString, int? page, string sortOrder)
        {
            if (_context.AirlineCompanies == null)
                return Problem("Entity set Airline Companies  is null.");

            ViewData["NameSort"] = String.IsNullOrEmpty(sortOrder) || !String.Equals(sortOrder, "Name_desc") ? "Name_desc": "Name_asc";     

            var airlineCompanies = from a in _context.AirlineCompanies select a;

            if (!String.IsNullOrEmpty(searchString))
                airlineCompanies = airlineCompanies.Where(a => a.Name!.Contains(searchString));

            airlineCompanies = await Sorting(airlineCompanies, sortOrder);
            var pagedList = new PagedList(page ?? 1, airlineCompanies.Count(), 6);
            ViewBag.PagedList = pagedList;

            return View(await airlineCompanies.Include(a => a.Photo).Skip(pagedList.Start).Take(pagedList.Limit).ToListAsync());
        }
        public async Task<IQueryable<AirlineCompany>> Sorting(IQueryable<AirlineCompany> airlineCompanies,string sortOrder){
            switch(sortOrder){
                case "Name_desc": airlineCompanies = airlineCompanies.OrderByDescending(a => a.Name); break;
                case "Name_asc": airlineCompanies = airlineCompanies.OrderBy(a => a.Name); break;
            }
            return airlineCompanies;
        }

        // GET: AirlineCompanies/Create
        [Authorize(Policy = "AirlineCompaniesCreate")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AirlineCompaniesCreate")]
        public async Task<IActionResult> Create([Bind("Id,Name")] AirlineCompany airlineCompany, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(file);
                if (result.Error != null) return BadRequest(result.Error.Message);

                var photo = new Photo
                {
                    Url = result.SecureUrl.AbsoluteUri,
                    PublicId = result.PublicId
                };
                airlineCompany.Photo = photo;
                _context.Add(airlineCompany);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(airlineCompany);
        }

        [Authorize(Policy = "AirlineCompaniesEdit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AirlineCompanies == null)
            {
                return NotFound();
            }

            var airlineCompany = await _context.AirlineCompanies.Include(a => a.Photo).Where(a => a.Id == id).FirstOrDefaultAsync();
            ViewBag.Icon = airlineCompany.Photo.Url;
            if (airlineCompany == null)
            {
                return NotFound();
            }
            return View(airlineCompany);
        }

        
        [Authorize(Policy = "AirlineCompaniesEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] AirlineCompany airlineCompany)
        {
            if (id != airlineCompany.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(airlineCompany);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AirlineCompanyExists(airlineCompany.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(airlineCompany);
        }

        [Authorize(Policy = "AirlineCompaniesDelete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AirlineCompanies == null)
            {
                return NotFound();
            }

            var airlineCompany = await _context.AirlineCompanies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (airlineCompany == null)
            {
                return NotFound();
            }

            return View(airlineCompany);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AirlineCompaniesDelete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AirlineCompanies == null)
            {
                return Problem("Entity set 'DataContext.AirlineCompanies'  is null.");
            }
            var airlineCompany = await _context.AirlineCompanies.Include(a => a.Photo).Where(a => a.Id == id).FirstOrDefaultAsync();
            if (airlineCompany != null)
            {
                if(airlineCompany.Photo.PublicId != null){
                    var result = await _photoService.DeletePhotoAsync(airlineCompany.Photo.PublicId);
                    if(result.Error != null) return BadRequest(result.Error.Message);
                }
                _context.AirlineCompanies.Remove(airlineCompany);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AirlineCompanyExists(int id)
        {
            return _context.AirlineCompanies.Any(e => e.Id == id);
        }
    }
}
