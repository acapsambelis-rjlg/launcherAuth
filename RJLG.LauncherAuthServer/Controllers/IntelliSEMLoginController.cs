using RJLG.LauncherAuthServer.Models;
using SNS.Data.DataSerializer.DataExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace RJLG.LauncherAuthServer.Controllers
{
    [AllowAnonymous]
    public class IntelliSEMLoginController : Controller
    {
        [HttpPost]
        public JsonResult ValidateKey(string loginKey, string macAddress)
        {
            // Log user's IP address
            string userIp = Request.UserHostAddress;
            System.Diagnostics.Debug.WriteLine($"ValidateKey called from IP: {userIp}");

            if (string.IsNullOrEmpty(loginKey))
            {
                return Json(new { success = false, message = "Key is required." });
            }

            if (IsValidKey(loginKey, out var key))
            {
                LogUser(key, macAddress);
                IncrementLoginUsageStatistic();
                return Json(new { success = true, message = "Key is valid.", version = GetVersion(key) });
            }
            else
            {
                return Json(new { success = false, message = "Key is invalid." });
            }
        }

        private void LogUser(LoginKey key, string mac)
        {
            var existingUser = IntelliSEMUser.LoadOne(mac);
            if (existingUser != null)
            {
                var now = DateTime.Now;
                existingUser.LastLogin = now;
                existingUser.Update();
            }
            else
            {
                var newUser = new IntelliSEMUser(mac, "", key);
                newUser.Save();
            }

            key.Update();
        }

        private void IncrementLoginUsageStatistic()
        {
            HomeController.UsageStatistics["Lifetime IntelliSEM Logins"].Count += 1;
            HomeController.UsageStatistics["Lifetime IntelliSEM Logins"].Update();
        }

        [HttpPost]
        public ActionResult DownloadVersionZip(string version, string loginKey)
        {
            if (!IsValidKey(loginKey, out var key))
            {
                return new HttpStatusCodeResult(403, "Invalid login key.");
            }
            // Find the version object by version string
            var item = VersionController.Versions.FirstOrDefault(v => v.Version == version);
            if (item == null || !item.CurrentlyExistsOnDisk)
            {
                return HttpNotFound("Version not found or file missing.");
            }

            var filePath = item.StoredPath;
            var fileName = Path.GetFileName(filePath);

            var fileSize = new FileInfo(filePath).Length;
            IncrementDownloadUsageStatistic((int)fileSize);

            return File(filePath, "application/zip", fileName);
        }

        private void IncrementDownloadUsageStatistic(int bytes)
        {
            var stat = HomeController.UsageStatistics["Lifetime Data Transferred"];
            stat.Count += bytes;

            // Set Value to a human-readable string (e.g., MB, GB)
            long totalBytes = stat.Count.Value;
            string readableValue;
            if (totalBytes >= 1024 * 1024 * 1024)
                readableValue = $"{Math.Round(totalBytes / (1024.0 * 1024 * 1024), 2)} GB";
            else if (totalBytes >= 1024 * 1024)
                readableValue = $"{Math.Round(totalBytes / (1024.0 * 1024), 2)} MB";
            else if (totalBytes >= 1024)
                readableValue = $"{Math.Round(totalBytes / 1024.0, 2)} KB";
            else
                readableValue = $"{totalBytes} B";

            stat.Value = readableValue;
            stat.Update();
        }

        private bool IsValidKey(string loginKey, out LoginKey key)
        {
            key = null;
            if (string.IsNullOrEmpty(loginKey))
            {

                return false;
            }
            foreach (CustomerAccount customer in CustomersController.Customers)
            {
                var customerKey = customer.LoginKeys.FirstOrDefault(k => k.Key == loginKey);
                if (customerKey != null)
                {
                    key = customerKey; // Set out parameter
                    return customerKey.IsValid();
                }
            }

            return false;
        }

        private string GetVersion(LoginKey key)
        {
            if (key.CustomerAccount.HasValue)
            {
                var customer = CustomersController.Customers.FirstOrDefault(c => c.ID == key.CustomerAccount.Value);
                if (customer != null)
                {
                    var version = VersionController.Versions.FirstOrDefault(v => v.Version == customer.CurrentVersion);
                    if (version != null)
                    {
                        return version.Version;
                    }
                }
            }

            return "Unknown";
        }

    }
}