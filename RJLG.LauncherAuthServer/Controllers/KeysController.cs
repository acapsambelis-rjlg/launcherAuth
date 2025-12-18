using RJLG.LauncherAuthServer.Models;
using SNS.Data.DataSerializer.DataExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RJLG.LauncherAuthServer.Controllers
{
    public class KeysController : Controller
    {
        // POST: /Keys/Save
        [HttpPost]
        [Authorize]
        public ActionResult Save(string loginKey, int? instanceCount, DateTime? expiration, int? customerId)
        {
            if (!string.IsNullOrEmpty(loginKey))
            {
                var customer = CustomersController.Customers.FirstOrDefault(c => c.ID == customerId);
                if (customer != null)
                {
                    var newKey = new LoginKey(loginKey, customerId, instanceCount, expiration);
                    newKey.Save();
                    customer.LoginKeys.Add(newKey);
                    TempData["Message"] = "Key saved to customer!";
                    IncrementKeyUsageStatistic();
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

        // POST: /Keys/Remove
        [HttpPost]
        [Authorize]
        public ActionResult Remove(int keyId, int customerId, string returnUrl = null)
        {
            var customer = CustomersController.Customers.FirstOrDefault(c => c.ID == customerId);
            if (customer != null)
            {
                var item = customer.LoginKeys.FirstOrDefault(k => k.ID == keyId);
                if (item != null)
                {
                    customer.LoginKeys.Remove(item);
                    item.Delete();
                }
            }
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index");
        }

        private void IncrementKeyUsageStatistic()
        {
            HomeController.UsageStatistics["Total Keys"].Count += 1;
        }
    }
}