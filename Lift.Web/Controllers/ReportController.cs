using Lift.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lift.Web.Controllers
{
    public class ReportController : Controller
    {
        private readonly LiftDbContext _db;

        public ReportController(LiftDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Account");

            var interruptions = await _db.ProcessInterruptions
                .Include(p => p.StoppedByUser)
                .Include(p => p.RestartedByUser)
                .OrderByDescending(p => p.StoppedAt)
                .ToListAsync();

            return View(interruptions);
        }
    }
}