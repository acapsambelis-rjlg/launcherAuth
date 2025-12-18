using SNS.Data.DataSerializer;
using SNS.Data.DataSerializer.DataExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RJLG.LauncherAuthServer.Models
{
    [DataClass("IntelliSEMUser")]
    public class IntelliSEMUser : IGenericDataObject<IntelliSEMUser>
    {
        public static IntelliSEMUser LoadOne(string macAddress)
        {
            IntelliSEMUser user = Data<IntelliSEMUser>.Load(u => u.MacAddress == macAddress);
            return user;
        }
        public static IntelliSEMUser[] LoadAll(string key = "")
        {
            IntelliSEMUser[] users = Data<IntelliSEMUser>.LoadMany(u => u.LoginKey == key);
            return users;
        }

        [DataProperty("MacAddress", Key = true)]
        public string MacAddress { get; set; }
        [DataProperty("IPAddress")]
        public string IPAddress { get; set; }
        [DataProperty("LastLogin")]
        public DateTime LastLogin { get; set; }
        [DataProperty("LoginKey")]
        public string LoginKey { get; set; }

        private bool wasLoaded;
        public bool WasLoaded { get => wasLoaded; set => wasLoaded = value; }

        public IntelliSEMUser() { }

        public IntelliSEMUser(string macAddress, string ipAddress, LoginKey key)
        {
            MacAddress = macAddress;
            IPAddress = ipAddress;
            LastLogin = DateTime.Now;
            LoginKey = key.Key;
        }
    }
}