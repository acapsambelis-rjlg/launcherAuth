using RJLG.LauncherAuthServer.Models;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RJLG.LauncherAuthServer.Controllers
{
    public class HomeController : Controller
    {
        public static Dictionary<string, UsageStatistic> UsageStatistics = new Dictionary<string, UsageStatistic>();

        // GET: /Home/
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.UsageStatistics = UsageStatistics;
            return View();
        }

        // GET: /Home/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Home/Login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string username, string password)
        {
            if (AdminAccounts.IsValid(username, password))
            {
                FormsAuthentication.SetAuthCookie(username, false);
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = "Incorrect username or password.";
            return View();
        }

        // GET: /Home/Logout
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        // GET: /Home/Settings
        [HttpGet]
        public ActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateAdmin(string username, string password)
        {
            AdminAccounts.StoreAdminAccount(username, password);
            ViewBag.Message = "Settings updated successfully.";
            return RedirectToAction("Settings");
        }

        [HttpPost]
        public ActionResult ChangePassword(string password)
        {
            AdminAccounts.UpdateAdminPassword(User.Identity.Name, password);
            ViewBag.Message = "Settings updated successfully.";
            return RedirectToAction("Settings");
        }
    }
}