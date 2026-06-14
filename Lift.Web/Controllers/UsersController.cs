using Lift.Data;
using Lift.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lift.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly LiftDbContext _db;

        public UsersController(LiftDbContext db)
        {
            _db = db;
        }

        private bool IsAdmin() => HttpContext.Session.GetString("Role") == "Admin";

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var users = await _db.Users.ToListAsync();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string username, string password, string role)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (await _db.Users.AnyAsync(u => u.Username == username))
            {
                ViewBag.Error = "Username-ul există deja.";
                return View();
            }

            _db.Users.Add(new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role,
                CreatedAt = DateTime.Now
            });
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string username, string password, string role)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Username = username;
            user.Role = role;
            if (!string.IsNullOrEmpty(password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var user = await _db.Users.FindAsync(id);
            if (user != null)
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}