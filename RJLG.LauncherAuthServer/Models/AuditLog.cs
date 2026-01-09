using SNS.Data.DataSerializer;
using SNS.Data.DataSerializer.DataExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RJLG.LauncherAuthServer.Models
{
    [DataClass("AuditLogs")]
    public class AuditLog : IGenericDataObject<AuditLog>
    {
        private int id;
        private DateTime timestamp;
        private string username;
        private string action;
        private string entityType;
        private string entityId;
        private string details;
        private string ipAddress;

        [DataProperty("ID", Key = true)]
        public int ID { get => id; set => id = value; }

        [DataProperty("Timestamp")]
        public DateTime Timestamp { get => timestamp; set => timestamp = value; }

        [DataProperty("Username")]
        public string Username { get => username; set => username = value; }

        [DataProperty("Action")]
        public string Action { get => action; set => action = value; }

        [DataProperty("EntityType")]
        public string EntityType { get => entityType; set => entityType = value; }

        [DataProperty("EntityId")]
        public string EntityId { get => entityId; set => entityId = value; }

        [DataProperty("Details")]
        public string Details { get => details; set => details = value; }

        [DataProperty("IpAddress")]
        public string IpAddress { get => ipAddress; set => ipAddress = value; }

        private bool wasLoaded = false;
        public bool WasLoaded { get => wasLoaded; set => wasLoaded = value; }

        public AuditLog() 
        {
            Timestamp = DateTime.UtcNow;
        }

        public AuditLog(string username, string action, string entityType = null, string entityId = null, string details = null, string ipAddress = null)
        {
            Timestamp = DateTime.UtcNow;
            Username = username;
            Action = action;
            EntityType = entityType;
            EntityId = entityId;
            Details = details;
            IpAddress = ipAddress;
        }

        public static List<AuditLog> LoadAll()
        {
            return Data<AuditLog>.LoadAll().OrderByDescending(a => a.Timestamp).ToList();
        }

        public static List<AuditLog> LoadRecent(int count = 100)
        {
            return Data<AuditLog>.LoadAll().OrderByDescending(a => a.Timestamp).Take(count).ToList();
        }

        public static List<AuditLog> LoadByAction(string action)
        {
            return Data<AuditLog>.LoadAll().Where(a => a.Action == action).OrderByDescending(a => a.Timestamp).ToList();
        }

        public static List<AuditLog> LoadByUser(string username)
        {
            return Data<AuditLog>.LoadAll().Where(a => a.Username == username).OrderByDescending(a => a.Timestamp).ToList();
        }

        public static void Log(string username, string action, string entityType = null, string entityId = null, string details = null, string ipAddress = null)
        {
            var log = new AuditLog(username, action, entityType, entityId, details, ipAddress);
            log.Save();
        }
    }
}
