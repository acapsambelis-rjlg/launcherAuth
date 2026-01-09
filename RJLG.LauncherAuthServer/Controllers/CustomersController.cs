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
    public class CustomersController : Controller
    {
        public static List<CustomerAccount> Customers = new List<CustomerAccount>();

        private string GetIpAddress()
        {
            string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip ?? "Unknown";
        }

        public ActionResult Index(string search = null)
        {
            var customers = Customers;
            
            if (!string.IsNullOrEmpty(search))
            {
                customers = customers.Where(c => 
                    c.CustomerName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (c.ContactName != null && c.ContactName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (c.ContactEmail != null && c.ContactEmail.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (c.CurrentVersion != null && c.CurrentVersion.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                ).ToList();
                ViewBag.Search = search;
            }
            
            ViewBag.Customers = customers.OrderBy(c => c.CustomerName).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(string customerName, string versionId, int? keysAllowed, string contactName, string contactEmail, string timezone)
        {
            if (!string.IsNullOrEmpty(customerName))
            {
                var intelliSEMVersion = VersionController.Versions.FirstOrDefault(v => v.Version.Equals(versionId));
                var customer = new CustomerAccount(customerName, intelliSEMVersion, contactName, contactEmail, timezone, keysAllowed);
                customer.Save();
                
                AuditLog.Log(
                    User.Identity.Name,
                    "CreateCustomer",
                    "CustomerAccount",
                    customer.ID.ToString(),
                    $"Created customer '{customerName}'",
                    GetIpAddress()
                );
                
                TempData["Message"] = "Customer created successfully!";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(int customerId)
        {
            var item = Customers.FirstOrDefault(k => k.ID == customerId);
            if (item != null)
            {
                AuditLog.Log(
                    User.Identity.Name,
                    "DeleteCustomer",
                    "CustomerAccount",
                    customerId.ToString(),
                    $"Deleted customer '{item.CustomerName}'",
                    GetIpAddress()
                );
                
                item.Delete();
                TempData["Message"] = "Customer deleted successfully.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Details(int? customerId)
        {
            ViewBag.AvailableVersions = VersionController.Versions;
            if (!customerId.HasValue)
            {
                return RedirectToAction("Index");
            }
            var customer = Customers.FirstOrDefault(c => c.ID == customerId);
            if (customer == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.CustomerFiles = CustomerFile.LoadByCustomerId(customerId.Value);
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateVersion(int id, string currentVersion)
        {
            var customer = Customers.FirstOrDefault(c => c.ID == id);
            if (customer != null)
            {
                var oldVersion = customer.CurrentVersion;
                customer.CurrentVersion = currentVersion;
                customer.Update();
                
                AuditLog.Log(
                    User.Identity.Name,
                    "UpdateVersion",
                    "CustomerAccount",
                    id.ToString(),
                    $"Updated version from '{oldVersion}' to '{currentVersion}' for customer '{customer.CustomerName}'",
                    GetIpAddress()
                );
                
                TempData["Message"] = "Version updated successfully.";
            }
            return RedirectToAction("Details", new { customerId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateContact(int ID, string ContactName, string ContactEmail, string Timezone, string Notes)
        {
            var customer = Customers.FirstOrDefault(c => c.ID == ID);
            if (customer != null)
            {
                customer.ContactName = ContactName;
                customer.ContactEmail = ContactEmail;
                customer.Timezone = Timezone;
                customer.Notes = Notes;
                customer.Update();
                
                AuditLog.Log(
                    User.Identity.Name,
                    "UpdateCustomer",
                    "CustomerAccount",
                    ID.ToString(),
                    $"Updated contact info for customer '{customer.CustomerName}'",
                    GetIpAddress()
                );
                
                TempData["Message"] = "Contact info updated successfully.";
            }
            return RedirectToAction("Details", new { customerId = ID });
        }
    }
}
