using SNS.Data.DataSerializer;
using SNS.Data.DataSerializer.DataExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RJLG.LauncherAuthServer.Models
{
    [DataClass("CustomerFiles")]
    public class CustomerFile : IIDDataObject<CustomerFile>
    {
        private int id;
        private string fileName;
        private string originalName;
        private string contentType;
        private long fileSize;
        private string description;
        private DateTime uploadedAt;
        private string uploadedBy;
        private int customerAccountId;

        [DataProperty("ID", Key = true)]
        public int ID { get => id; set => id = value; }

        [DataProperty("FileName")]
        public string FileName { get => fileName; set => fileName = value; }

        [DataProperty("OriginalName")]
        public string OriginalName { get => originalName; set => originalName = value; }

        [DataProperty("ContentType")]
        public string ContentType { get => contentType; set => contentType = value; }

        [DataProperty("FileSize")]
        public long FileSize { get => fileSize; set => fileSize = value; }

        [DataProperty("Description")]
        public string Description { get => description; set => description = value; }

        [DataProperty("UploadedAt")]
        public DateTime UploadedAt { get => uploadedAt; set => uploadedAt = value; }

        [DataProperty("UploadedBy")]
        public string UploadedBy { get => uploadedBy; set => uploadedBy = value; }

        [DataProperty("CustomerAccountId")]
        public int CustomerAccountId { get => customerAccountId; set => customerAccountId = value; }

        private bool wasLoaded = false;
        public bool WasLoaded { get => wasLoaded; set => wasLoaded = value; }

        public CustomerFile()
        {
            UploadedAt = DateTime.UtcNow;
        }

        public static List<CustomerFile> LoadByCustomerId(int customerId)
        {
            return Data<CustomerFile>.LoadAll()
                .Where(f => f.CustomerAccountId == customerId)
                .OrderByDescending(f => f.UploadedAt)
                .ToList();
        }

        public static CustomerFile LoadById(int id)
        {
            return Data<CustomerFile>.LoadAll().FirstOrDefault(f => f.ID == id);
        }

        public string GetFileSizeDisplay()
        {
            if (FileSize < 1024) return $"{FileSize} B";
            if (FileSize < 1024 * 1024) return $"{FileSize / 1024.0:F1} KB";
            return $"{FileSize / (1024.0 * 1024.0):F1} MB";
        }
    }
}
