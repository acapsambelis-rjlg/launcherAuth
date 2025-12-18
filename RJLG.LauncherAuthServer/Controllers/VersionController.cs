using RJLG.LauncherAuthServer.Models;
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
    public class VersionController : Controller
    {
        public static List<IntelliSEMVersion> Versions = new List<IntelliSEMVersion>();

        // GET: Version
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            // Pass version strings to the view
            ViewBag.Versions = Versions;
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Upload(string versionName, HttpPostedFileBase zipFile)
        {
            if (zipFile != null && zipFile.ContentLength > 0 && Path.GetExtension(zipFile.FileName).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                var saveDirectory = ConfigurationManager.AppSettings["VersionSavePath"];
                var savePath = Path.Combine(saveDirectory, zipFile.FileName);
                zipFile.SaveAs(savePath);

                // Add to versions list
                var version = new IntelliSEMVersion(versionName, savePath);
                version.Save();
                Versions.Add(version);
            }
            // Redirect back to index
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        [Authorize]
        public ActionResult Delete(string version)
        {
            // Find the version object by version string
            var item = Versions.FirstOrDefault(v => v.Version == version);
            if (item != null)
            {
                // Remove from list
                Versions.Remove(item);
                item.Delete();

                // Delete the file from disk if it exists
                if (System.IO.File.Exists(item.StoredPath))
                {
                    System.IO.File.Delete(item.StoredPath);
                }
            }
            // Redirect back to index
            return RedirectToAction("Index");
        }
    }
}