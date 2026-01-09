using RJLG.LauncherAuthServer.Models;
using SNS.Data.DataSerializer.DataExtensions;
using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace RJLG.LauncherAuthServer.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        private const long MaxFileSize = 50 * 1024 * 1024; // 50 MB

        private string GetIpAddress()
        {
            string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip ?? "Unknown";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(int customerId, HttpPostedFileBase file, string description)
        {
            if (file == null || file.ContentLength == 0)
            {
                TempData["Error"] = "Please select a file to upload.";
                return RedirectToAction("Details", "Customers", new { customerId });
            }

            if (file.ContentLength > MaxFileSize)
            {
                TempData["Error"] = "File size exceeds the 50MB limit.";
                return RedirectToAction("Details", "Customers", new { customerId });
            }

            try
            {
                var uploadDir = ConfigurationManager.AppSettings["FileSavePath"];
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadDir, uniqueFileName);

                file.SaveAs(filePath);

                var customerFile = new CustomerFile
                {
                    FileName = uniqueFileName,
                    OriginalName = file.FileName,
                    ContentType = file.ContentType,
                    FileSize = file.ContentLength,
                    Description = description,
                    UploadedAt = DateTime.UtcNow,
                    UploadedBy = User.Identity.Name,
                    CustomerAccountId = customerId
                };

                customerFile.Save();

                AuditLog.Log(
                    User.Identity.Name,
                    "UploadFile",
                    "CustomerFile",
                    customerFile.ID.ToString(),
                    $"Uploaded file '{file.FileName}' for customer {customerId}",
                    GetIpAddress()
                );

                TempData["Success"] = "File uploaded successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to upload file: " + ex.Message;
            }

            return RedirectToAction("Details", "Customers", new { customerId });
        }

        public ActionResult Download(int id)
        {
            var customerFile = CustomerFile.LoadById(id);
            if (customerFile == null)
            {
                return HttpNotFound();
            }

            var filePath = Path.Combine(ConfigurationManager.AppSettings["FileSavePath"], customerFile.FileName);
            if (!System.IO.File.Exists(filePath))
            {
                return HttpNotFound();
            }

            AuditLog.Log(
                User.Identity.Name,
                "DownloadFile",
                "CustomerFile",
                id.ToString(),
                $"Downloaded file '{customerFile.OriginalName}'",
                GetIpAddress()
            );

            return File(filePath, customerFile.ContentType ?? "application/octet-stream", customerFile.OriginalName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, int customerId)
        {
            var customerFile = CustomerFile.LoadById(id);
            if (customerFile == null)
            {
                TempData["Error"] = "File not found.";
                return RedirectToAction("Details", "Customers", new { customerId });
            }

            try
            {
                var filePath = Path.Combine(ConfigurationManager.AppSettings["FileSavePath"], customerFile.FileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                AuditLog.Log(
                    User.Identity.Name,
                    "DeleteFile",
                    "CustomerFile",
                    id.ToString(),
                    $"Deleted file '{customerFile.OriginalName}' from customer {customerId}",
                    GetIpAddress()
                );

                customerFile.Delete();

                TempData["Success"] = "File deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to delete file: " + ex.Message;
            }

            return RedirectToAction("Details", "Customers", new { customerId });
        }
    }
}
