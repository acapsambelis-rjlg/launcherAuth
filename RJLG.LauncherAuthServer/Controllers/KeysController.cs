using RJLG.LauncherAuthServer.Models;
using SNS.Data.DataSerializer.DataExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RJLG.LauncherAuthServer.Controllers
{
    [Authorize]
    public class KeysController : Controller
    {
        public static List<LoginKey> Keys = new List<LoginKey>();

        private string GetIpAddress()
        {
            string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip ?? "Unknown";
        }

        public ActionResult Index()
        {
            ViewBag.Customers = CustomersController.Customers;
            return View(Keys);
        }

        public ActionResult Expiring()
        {
            var thirtyDaysFromNow = DateTime.UtcNow.AddDays(30);
            var expiringKeys = Keys
                .Where(k => k.Expiration.HasValue && k.Expiration.Value <= thirtyDaysFromNow && k.Expiration.Value > DateTime.UtcNow)
                .OrderBy(k => k.Expiration)
                .ToList();
            ViewBag.Customers = CustomersController.Customers;
            return View(expiringKeys);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(string loginKey, int? instanceCount, DateTime? expiration, int? customerId)
        {
            if (!string.IsNullOrEmpty(loginKey))
            {
                var customer = CustomersController.Customers.FirstOrDefault(c => c.ID == customerId);
                if (customer != null)
                {
                    var newKey = new LoginKey(loginKey, customerId, instanceCount, expiration);
                    newKey.Save();
                    
                    AuditLog.Log(
                        User.Identity.Name,
                        "CreateKey",
                        "LoginKey",
                        loginKey,
                        $"Created key for customer '{customer.CustomerName}' with instance limit: {instanceCount?.ToString() ?? "Unlimited"}, expiration: {expiration?.ToString("yyyy-MM-dd") ?? "Never"}",
                        GetIpAddress()
                    );
                    
                    TempData["Message"] = "Key saved to customer!";
                }
                else
                {
                    TempData["Error"] = "Customer not found!";
                }
            }
            else
            {
                TempData["Error"] = "Login key cannot be empty!";
            }
            return RedirectToAction("Details", "Customers", new { customerId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(int keyId, int customerId, string returnUrl = null)
        {
            var customer = CustomersController.Customers.FirstOrDefault(c => c.ID == customerId);
            if (customer != null)
            {
                var item = customer.LoginKeys?.FirstOrDefault(k => k.ID == keyId);
                if (item != null)
                {
                    AuditLog.Log(
                        User.Identity.Name,
                        "DeleteKey",
                        "LoginKey",
                        item.Key,
                        $"Deleted key from customer '{customer.CustomerName}'",
                        GetIpAddress()
                    );
                    
                    item.Delete();
                    TempData["Message"] = "Key removed successfully.";
                }
            }
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Details", "Customers", new { customerId });
        }
    }
}
