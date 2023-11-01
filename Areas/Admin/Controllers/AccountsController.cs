using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightBooking.Models;
using FlightBooking.Helpers;
using FlightBooking.Data;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsController : Controller
    {
        private readonly DataContext _context;

        public AccountsController(DataContext context)
        {
            _context = context;
        }

        // GET: Accounts
        public async Task<IActionResult> Index(string searchString, int? page, string sortOrder, string searchOption)
        {
            ViewData["IdSort"] = String.IsNullOrEmpty(sortOrder) || !String.Equals(sortOrder, "Id_desc") ? "Id_desc": "Id_asc";
            ViewData["FullNameSort"] = String.IsNullOrEmpty(sortOrder) || !String.Equals(sortOrder, "FullName_desc") ? "FullName_desc": "FullName_asc";
            ViewData["UserNameSort"] = String.IsNullOrEmpty(sortOrder)|| !String.Equals(sortOrder, "UserName_desc") ? "UserName_desc": "UserName_asc";
            ViewData["GenderSort"] = String.IsNullOrEmpty(sortOrder) || !String.Equals(sortOrder, "Gender_desc") ? "Gender_desc": "Gender_asc";
            ViewData["CreatedSort"] = String.Equals(sortOrder, "Date") || !String.Equals(sortOrder, "Created_desc") ? "Created_desc": "Created_asc";

            if (_context.Users == null)
                return Problem("Entity set Users is null.");

            var users = from a in _context.Users select a;
            users = await Searching(users, searchString, searchOption);

            users = await Sorting(users, sortOrder);

            var pagedList =  new PagedList(page ?? 1, users.Count(), 6);
            ViewBag.PagedList = pagedList;

            return View(await users.Skip(pagedList.Start).Take(pagedList.Limit).ToListAsync());
        }
        public async Task<IQueryable<AppUser>> Sorting(IQueryable<AppUser> users,string sortOrder){
            switch(sortOrder){
                case "Id_desc": users = users.OrderByDescending(a => a.Id); break;
                case "Id_asc": users = users.OrderBy(a => a.Id); break;
                case "FullName_desc": users = users.OrderByDescending(a => a.LastName); break;
                case "FullName_asc": users = users.OrderBy(a => a.LastName); break;
                case "UserName_desc": users = users.OrderByDescending(a => a.UserName); break;
                case "UserName_asc": users = users.OrderBy(a => a.UserName); break;
                case "Gender_desc": users = users.OrderByDescending(a => a.Gender); break;
                case "Gender_asc": users = users.OrderBy(a => a.Gender); break;
                case "Created_desc": users = users.OrderByDescending(a => a.Created); break;
                case "Created_asc": users = users.OrderBy(a => a.Created); break;
            }
            return users;
        }
        public async Task<IQueryable<AppUser>> Searching(IQueryable<AppUser> airports, string searchString, string searchOption){
            if (!String.IsNullOrEmpty(searchString)){
                searchString.Replace('+', ' ');
                switch(searchOption){
                    case "FullName": airports = airports.Where(a => (a.FirstName + " "+ a.LastName)!.Contains(searchString)); break;
                    case "UserName": airports = airports.Where(a => a.UserName!.Contains(searchString)); break;
                }
            }
            return airports;
        }                

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var appUser = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,DateOfBirth,Gender,Created,LastActive,City,Country,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(appUser);
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var appUser = await _context.Users.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }
            return View(appUser);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FirstName,LastName,DateOfBirth,Gender,Created,LastActive,City,Country,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AppUser appUser)
        {
            if (id != appUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserExists(appUser.Id))
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
            return View(appUser);
        }

        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var appUser = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'DataContext.Users'  is null.");
            }
            var appUser = await _context.Users.FindAsync(id);
            if (appUser != null)
            {
                _context.Users.Remove(appUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppUserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
