using RJLG.LauncherAuthServer.Models;
using SNS.Data.DataSerializer;
using SNS.Data.DataSerializer.DataExtensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RJLG.LauncherAuthServer.Controllers
{
    [Authorize]
    public class VersionController : Controller
    {
        public static List<IntelliSEMVersion> Versions = new List<IntelliSEMVersion>();

        private string GetIpAddress()
        {
            string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip ?? "Unknown";
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Versions = Versions;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(string versionName, HttpPostedFileBase zipFile)
        {
            if (zipFile != null && zipFile.ContentLength > 0 && Path.GetExtension(zipFile.FileName).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                var saveDirectory = ConfigurationManager.AppSettings["VersionSavePath"];
                
                if (!Directory.Exists(saveDirectory))
                {
                    Directory.CreateDirectory(saveDirectory);
                }
                
                var savePath = Path.Combine(saveDirectory, zipFile.FileName);
                zipFile.SaveAs(savePath);

                var version = new IntelliSEMVersion(versionName, savePath);
                version.Save();

                AuditLog.Log(
                    User.Identity.Name,
                    "UploadVersion",
                    "Version",
                    version.Version,
                    $"Uploaded version: {versionName}",
                    GetIpAddress()
                );

                TempData["Message"] = "Version uploaded successfully!";
            }
            else
            {
                TempData["Error"] = "Please upload a valid ZIP file.";
            }

            return RedirectToAction("Index");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string version)
        {
            var item = Versions.FirstOrDefault(v => v.Version == version);
            // get customers associated with version
            var customers = CustomersController.Customers.Where(c => c.CurrentVersion.Equals(version)).ToList();
            if (customers.Count > 0)
            {
                TempData["Message"] = $"Version {version} is associated with {customers.Count} users and cannot be deleted.";
                AuditLog.Log(
                    User.Identity.Name,
                    "AttemptDeleteVersion",
                    "Version",
                    item.Version,
                    $"Attempted deleted version: {version}",
                    GetIpAddress()
                );
                return RedirectToAction("Index");
            }

            if (item != null)
            {
                if (System.IO.File.Exists(item.StoredPath))
                {
                    System.IO.File.Delete(item.StoredPath);
                }

                AuditLog.Log(
                    User.Identity.Name,
                    "DeleteVersion",
                    "Version",
                    item.Version,
                    $"Deleted version: {version}",
                    GetIpAddress()
                );

                item.Delete();

                TempData["Message"] = "Version deleted successfully!";
            }

            return RedirectToAction("Index");
        }
    }
}
