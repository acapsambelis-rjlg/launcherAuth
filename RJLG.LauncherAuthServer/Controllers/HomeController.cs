using RJLG.LauncherAuthServer.Models;
using SNS.Data.DataSerializer.DataExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Logger = RJLG.LauncherAuthServer.Models.AuditLog;

namespace RJLG.LauncherAuthServer.Controllers
{
    public class HomeController : Controller
    {
        public static Dictionary<string, UsageStatistic> UsageStatistics = new Dictionary<string, UsageStatistic>();

        private string GetIpAddress()
        {
            string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip ?? "Unknown";
        }

        [Authorize]
        public ActionResult Index()
        {
            var thirtyDaysFromNow = DateTime.UtcNow.AddDays(30);
            var expiringKeys = KeysController.Keys
                .Where(k => k.Expiration.HasValue && k.Expiration.Value <= thirtyDaysFromNow && k.Expiration.Value > DateTime.UtcNow)
                .OrderBy(k => k.Expiration)
                .ToList();

            ViewBag.CustomerCount = CustomersController.Customers.Count;
            ViewBag.KeyCount = KeysController.Keys.Count;
            ViewBag.VersionCount = VersionController.Versions.Count;
            ViewBag.ExpiringKeys = expiringKeys;
            ViewBag.ExpiringKeyCount = expiringKeys.Count;
            ViewBag.Customers = CustomersController.Customers;

            ViewBag.LoginCount = UsageStatistics.ContainsKey("Lifetime IntelliSEM Logins") ? UsageStatistics["Lifetime IntelliSEM Logins"].Count : 0;
            ViewBag.DataTransferred = UsageStatistics.ContainsKey("Lifetime Data Transferred") ? UsageStatistics["Lifetime Data Transferred"].Value ?? "0 B" : "0 B";

            ViewBag.RecentAuditLogs = Logger.LoadRecent(10);

            return View();
        }

        [Authorize]
        public ActionResult AuditLog(string filter = null)
        {
            var logs = Logger.LoadAll();

            if (!string.IsNullOrEmpty(filter))
            {
                logs = logs.Where(l => l.Action == filter).ToList();
                ViewBag.Filter = filter;
            }

            ViewBag.AuditLogs = logs.Take(100).ToList();
            ViewBag.Actions = Logger.LoadAll().Select(l => l.Action).Distinct().OrderBy(a => a).ToList();

            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string username, string password)
        {
            if (AdminAccounts.IsValid(username, password))
            {
                FormsAuthentication.SetAuthCookie(username, false);
                Logger.Log(username, "Login", "Admin", null, "Logged in successfully", GetIpAddress());
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = "Incorrect username or password.";
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Logger.Log(User.Identity.Name, "Logout", "Admin", null, "Logged out", GetIpAddress());
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAdmin(string username, string password)
        {
            AdminAccounts.StoreAdminAccount(username, password);
            Logger.Log(User.Identity.Name, "CreateAdmin", "Admin", username, $"Created admin account: {username}", GetIpAddress());
            TempData["Message"] = "Admin account created successfully.";
            return RedirectToAction("Settings");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string password)
        {
            AdminAccounts.UpdateAdminPassword(User.Identity.Name, password);
            Logger.Log(User.Identity.Name, "ChangePassword", "Admin", User.Identity.Name, "Password changed", GetIpAddress());
            TempData["Message"] = "Password changed successfully.";
            return RedirectToAction("Settings");
        }
    }
}
