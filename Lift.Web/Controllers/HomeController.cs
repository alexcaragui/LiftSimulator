using Lift.Data;
using Lift.Data.Models;
using Lift.Simulator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lift.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly LiftProcess _lift;
        private readonly LiftDbContext _db;

        public HomeController(LiftProcess lift, LiftDbContext db)
        {
            _lift = lift;
            _db = db;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            ViewBag.LiftState = _lift.CurrentState.ToString();
            ViewBag.CurrentLevel = (int)_lift.CurrentLevel;
            ViewBag.TargetLevel = _lift.TargetLevel.HasValue ? (int)_lift.TargetLevel : 0;
            ViewBag.Lamps = _lift.Lamps;

            return View();
        }

        [HttpPost]
        public IActionResult SelectLevel(int level)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");

            _lift.SelectLevel(level);
            LogEvent($"Nivel {level} selectat de {HttpContext.Session.GetString("Username")}", "Movement");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult MoveToBase()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");

            _lift.MoveToBase();
            LogEvent($"Coborare la baza initiata de {HttpContext.Session.GetString("Username")}", "Movement");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EmergencyStop()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Admin")
                return Forbid();

            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            _lift.EmergencyStop();

            var interruption = new ProcessInterruption
            {
                StoppedByUserId = userId,
                StoppedAt = DateTime.Now
            };
            _db.ProcessInterruptions.Add(interruption);
            _db.SaveChanges();

            LogEvent($"OPRIRE URGENTA de {HttpContext.Session.GetString("Username")}", "Emergency");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RestartProcess()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Admin")
                return Forbid();

            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            _lift.MoveToBase();

            var activeInterruptions = _db.ProcessInterruptions
                .Where(p => p.RestartedAt == null)
                .ToList();

            foreach (var interruption in activeInterruptions)
            {
                interruption.RestartedByUserId = userId;
                interruption.RestartedAt = DateTime.Now;
            }
            _db.SaveChanges();

            LogEvent($"Proces repornit de {HttpContext.Session.GetString("Username")}", "Movement");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetStatus()
        {
            return Json(new
            {
                state = _lift.CurrentState.ToString(),
                currentLevel = (int)_lift.CurrentLevel,
                targetLevel = _lift.TargetLevel.HasValue ? (int)_lift.TargetLevel : 0,
                lamps = _lift.Lamps
            });
        }

        [HttpGet]
        public IActionResult GetEvents()
        {
            using var scope = HttpContext.RequestServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LiftDbContext>();

            var events = db.LiftEvents
                .OrderByDescending(e => e.Timestamp)
                .Take(20)
                .Select(e => new
                {
                    message = e.Message,
                    timestamp = e.Timestamp.ToString("HH:mm:ss"),
                    eventType = e.EventType
                })
                .ToList();

            return Json(events);
        }

        private void LogEvent(string message, string eventType)
        {
            _db.LiftEvents.Add(new LiftEvent
            {
                Message = message,
                EventType = eventType,
                Timestamp = DateTime.Now
            });
            _db.SaveChanges();
        }
    }
}