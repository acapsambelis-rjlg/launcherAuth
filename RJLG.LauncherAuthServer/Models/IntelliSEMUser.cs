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

        [DataProperty("MacAddress", Key = true)]
        public string HardwareFingerprint { get; set; }
        [DataProperty("IPAddress")]
        public string IPAddress { get; set; }
        [DataProperty("LastLogin")]
        public DateTime LastLogin { get; set; }
        [DataProperty("LoginKey")]
        public string LoginKeyID { get; set; }


        private bool wasLoaded;
        public bool WasLoaded { get => wasLoaded; set => wasLoaded = value; }

        public IntelliSEMUser() { }

        public IntelliSEMUser(string fingerprint, string ipAddress)
        {
            HardwareFingerprint = fingerprint;
            IPAddress = ipAddress;
            LastLogin = DateTime.Now;
        }

        #region Operators

        public override bool Equals(object obj)
        {
            return Equals(obj as IntelliSEMUser);
        }

        public bool Equals(IntelliSEMUser other)
        {
            return !(other is null) &&
                   HardwareFingerprint == other.HardwareFingerprint;
        }

        public override int GetHashCode()
        {
            int hashCode = -583616143;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(HardwareFingerprint);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(IPAddress);
            hashCode = hashCode * -1521134295 + LastLogin.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(IntelliSEMUser left, IntelliSEMUser right)
        {
            return EqualityComparer<IntelliSEMUser>.Default.Equals(left, right);
        }

        public static bool operator !=(IntelliSEMUser left, IntelliSEMUser right)
        {
            return !(left == right);
        }

        #endregion
    }
}