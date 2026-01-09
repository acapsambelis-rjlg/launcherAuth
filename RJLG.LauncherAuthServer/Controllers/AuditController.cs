using RJLG.LauncherAuthServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace RJLG.LauncherAuthServer.Controllers
{
    [Authorize]
    public class AuditController : Controller
    {
        public ActionResult Index(string action = null, string username = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var logs = AuditLog.LoadAll();

            if (!string.IsNullOrEmpty(action))
            {
                logs = logs.Where(l => l.Action == action).ToList();
            }

            if (!string.IsNullOrEmpty(username))
            {
                logs = logs.Where(l => l.Username.Contains(username)).ToList();
            }

            if (fromDate.HasValue)
            {
                logs = logs.Where(l => l.Timestamp >= fromDate.Value).ToList();
            }

            if (toDate.HasValue)
            {
                logs = logs.Where(l => l.Timestamp <= toDate.Value.AddDays(1)).ToList();
            }

            ViewBag.SelectedAction = action;
            ViewBag.SelectedUsername = username;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            var actions = AuditLog.LoadAll().Select(l => l.Action).Distinct().OrderBy(a => a).ToList();
            ViewBag.Actions = actions;

            return View(logs);
        }
    }
}
