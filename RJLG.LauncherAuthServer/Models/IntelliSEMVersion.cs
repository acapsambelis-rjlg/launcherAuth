using SNS.Data.DataSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RJLG.LauncherAuthServer.Models
{
    [DataClass("IntelliSEMVersion")]
    public class IntelliSEMVersion : IGenericDataObject<IntelliSEMVersion>
    {
        private string version;
        private string storedPath;

        [DataProperty("VersionString", Key = true)]
        public string Version { get => version; set => version = value; }
        [DataProperty("StoragePath")]
        public string StoredPath { get => storedPath; set => storedPath = value; }

        public bool CurrentlyExistsOnDisk
        {
            get
            {
                return System.IO.File.Exists(storedPath);
            }
        }

        private bool wasLoaded;
        public bool WasLoaded { get => wasLoaded; set => wasLoaded = value; }

        public IntelliSEMVersion() { }
        public IntelliSEMVersion(string version, string storedPath)
        {
            this.version = version;
            this.storedPath = storedPath;
        }
    }
}