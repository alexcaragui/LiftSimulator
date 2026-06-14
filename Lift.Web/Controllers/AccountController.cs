using Lift.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lift.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly LiftDbContext _db;

        public AccountController(LiftDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Username") != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ViewBag.Error = "Username sau parola incorecta.";
                return View();
            }

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetInt32("UserId", user.Id);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}