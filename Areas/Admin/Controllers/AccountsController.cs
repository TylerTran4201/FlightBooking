using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightBooking.Models;
using FlightBooking.Helpers;
using FlightBooking.Data;
using Microsoft.AspNetCore.Identity;
using FlightBooking.DTOs;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsController : Controller
    {
        private readonly DataContext _context;
        public readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserStore<AppUser> _userStore;

        public AccountsController(DataContext context, RoleManager<AppRole> roleManager, UserManager<AppUser> userManager, IUserStore<AppUser> userStore)
        {
            _userManager = userManager;
            _userStore = userStore;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: Accounts
        public async Task<IActionResult> Index(string searchString, int? page, string sortOrder, string searchOption)
        {
            ViewData["IdSort"] = String.IsNullOrEmpty(sortOrder) || !String.Equals(sortOrder, "Id_desc") ? "Id_desc" : "Id_asc";
            ViewData["FullNameSort"] = String.IsNullOrEmpty(sortOrder) || !String.Equals(sortOrder, "FullName_desc") ? "FullName_desc" : "FullName_asc";
            ViewData["UserNameSort"] = String.IsNullOrEmpty(sortOrder) || !String.Equals(sortOrder, "UserName_desc") ? "UserName_desc" : "UserName_asc";
            ViewData["GenderSort"] = String.IsNullOrEmpty(sortOrder) || !String.Equals(sortOrder, "Gender_desc") ? "Gender_desc" : "Gender_asc";
            ViewData["CreatedSort"] = String.Equals(sortOrder, "Date") || !String.Equals(sortOrder, "Created_desc") ? "Created_desc" : "Created_asc";
            
            if (_context.Users == null)
                return Problem("Entity set Users is null.");

            var users = from a in _context.Users select a;
            users = await Searching(users, searchString, searchOption);

            users = await Sorting(users, sortOrder);

            var pagedList = new PagedList(page ?? 1, users.Count(), 6);
            ViewBag.PagedList = pagedList;

            return View(await users.Skip(pagedList.Start).Take(pagedList.Limit).ToListAsync());
        }
        public async Task<IQueryable<AppUser>> Sorting(IQueryable<AppUser> users, string sortOrder)
        {
            switch (sortOrder)
            {
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
        public async Task<IQueryable<AppUser>> Searching(IQueryable<AppUser> airports, string searchString, string searchOption)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString.Replace('+', ' ');
                switch (searchOption)
                {
                    case "FullName": airports = airports.Where(a => (a.FirstName + " " + a.LastName)!.Contains(searchString)); break;
                    case "UserName": airports = airports.Where(a => a.UserName!.Contains(searchString)); break;
                }
            }
            return airports;
        }
        public async Task<IActionResult> SetRoles(int? id)
        {
            if (id == null || _context.Users == null)
                return NotFound();

            var user = await _context.Users.Include(a => a.UserRoles).ThenInclude(s => s.Role).Where(a => a.Id == id).FirstOrDefaultAsync();


            var roles = _roleManager.Roles.ToList();
            if (roles == null)
                return NotFound();
                
            ViewBag.User = user.UserName;
            ViewBag.Roles = roles;
            ViewBag.CurrentRoles = GetCurrentRoles(user);
            return View();
        }

        public List<int> GetCurrentRoles(AppUser user)
        {
            var rolesSelect = new List<int>();
            foreach (var item in user.UserRoles)
            {
                rolesSelect.Add(item.Role.Id);
            }
            return rolesSelect;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetRoles(int id, List<int> selectedRolesId)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var users = from a in _context.Users select a;
            var user = _context.Users.Find(id);
            if (selectedRolesId == null)
            {
                await RemoveAllRolesAsync(user);
                return RedirectToAction(nameof(Index));
            }

            var roles = _context.Roles;

            await RemoveAllRolesAsync(user);
            List<string> rolesNeedToAdd = new List<string>();
            AppRole role = null;
            foreach (var item in selectedRolesId)
            {
                role = roles.Find(item);
                if (role != null)
                {
                    rolesNeedToAdd.Add(role.Name);
                }
            }
            await _userManager.AddToRolesAsync(user, rolesNeedToAdd);
            return RedirectToAction(nameof(Index));
        }

        public async Task RemoveAllRolesAsync(AppUser user)
        {
            if (user != null)
            {
                // Get the list of roles the user is currently in
                var roles = await _userManager.GetRolesAsync(user);

                // Detach the user from the context
                _context.Entry(user).State = EntityState.Detached;

                // Remove the user from each role
                foreach (var role in roles)
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }

            }
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
        public async Task<IActionResult> Create([Bind("FirstName,LastName,UserName,Gender")] AppUser appUser, string password)
        {
            if (ModelState.IsValid || password == null)
            {
                if (await UserExists(appUser.UserName))
                {
                    ModelState.AddModelError(string.Empty, "Username is taken");
                    return View(appUser);
                }

                appUser.UserName = appUser.UserName.ToLower();
                appUser.IsAdmin = true;
                await _userManager.CreateAsync(appUser, password);


                return RedirectToAction(nameof(Index));
            }
            return View(appUser);
        }
        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
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
        public async Task<IActionResult> Edit(int id, [Bind("FirstName,LastName,Id,DateOfBirth,Gender,City,Country,UserName,Email,PhoneNumber")] AppUser appUser)
        {
            if (id != appUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = _context.Users.Find(id);
                    if(user.UserName != appUser.UserName && await UserExists(appUser.UserName)){
                            ModelState.AddModelError(string.Empty, "Username is taken");
                            return View(appUser);
                    }

                    user.FirstName = appUser.FirstName;
                    user.LastName = appUser.LastName;
                    user.DateOfBirth = appUser.DateOfBirth;
                    user.Gender = appUser.Gender;
                    user.City = appUser.City;
                    user.Country = appUser.Country;
                    user.UserName = appUser.UserName;
                    user.Email = appUser.Email;
                    user.PhoneNumber = appUser.PhoneNumber;
                    _context.Update(user);
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
