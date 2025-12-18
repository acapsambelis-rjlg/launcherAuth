using RJLG.LauncherAuthServer.Models;
using SNS.Data.DataSerializer.DataExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RJLG.LauncherAuthServer.Controllers
{
    public class CustomersController : Controller
    {
        public static List<CustomerAccount> Customers = new List<CustomerAccount>();

        // GET: Customer
        public ActionResult Index()
        {
            ViewBag.Customers = Customers;
            return View();
        }

        // POST: /Customers/Save
        [HttpPost]
        [Authorize]
        public ActionResult Save(string customerName, string version, int? keysAllowed, string contactName, string contactEmail, string timezone)
        {
            if (!string.IsNullOrEmpty(customerName))
            {
                var intelliSEMVersion = VersionController.Versions.FirstOrDefault(v => v.Version.Equals(version));
                Customers.Add(new CustomerAccount(customerName, intelliSEMVersion, contactName, contactEmail, timezone, keysAllowed));
            }
            TempData["Message"] = "Customer saved!";
            IncrementCustomerUsageStatistic();
            return RedirectToAction("Index");
        }

        // POST: /Customers/Remove
        [HttpPost]
        [Authorize]
        public ActionResult Remove(int keyId)
        {
            var item = Customers.FirstOrDefault(k => k.ID == keyId);
            if (item != null)
            {
                Customers.Remove(item);
            }
            TempData["Message"] = "Key removed!";
            return RedirectToAction("Index");
        }

        // GET: /Customers/Details
        [HttpGet]
        [Authorize]
        public ActionResult Details(int? customerId)
        {
            ViewBag.AvailableVersions = VersionController.Versions;
            if (!customerId.HasValue)
            {
                return Redirect("Index");
            }
            var customer = Customers.FirstOrDefault(c => c.ID == customerId);
            return View(customer);
        }

        // POST: /Customers/UpdateVersion
        [HttpPost]
        [Authorize]
        public ActionResult UpdateVersion(int id, string currentVersion)
        {
            var customer = Customers.FirstOrDefault(c => c.ID == id);
            if (customer != null)
            {
                customer.CurrentVersion = currentVersion;
                customer.Save();
            }
            return RedirectToAction("Details", new { customerId = id });
        }

        // POST: /Customers/UpdateContact
        [HttpPost]
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
            }
            return RedirectToAction("Details", new { customerId = ID });
        }

        private void IncrementCustomerUsageStatistic()
        {
            HomeController.UsageStatistics["Total Customers"].Count += 1;
        }
    }
}