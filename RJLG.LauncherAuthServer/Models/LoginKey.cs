using SNS.Data.DataSerializer;
using SNS.Data.DataSerializer.DataExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RJLG.LauncherAuthServer.Models
{
    [DataClass("Keys")]
    public class LoginKey : IIDDataObject<LoginKey>
    {
        public static string GetRandomKey()
        {
            return Guid.NewGuid().ToString("N");
        }

        private int id;
        private string key;
        private DateTime createdAt;
        private DateTime? expiration;
        private int? instanceLimit;
        private int? customerAccountID;
        private List<IntelliSEMUser> associatedUsers = new List<IntelliSEMUser>();

        [DataProperty("Key", Key = true)]
        public string Key { get => key; set => key = value; }
        [DataProperty("CreatedAt")]
        public DateTime CreatedAt { get => createdAt; set => createdAt = value; }
        [DataProperty("Expiration")]
        public DateTime? Expiration { get => expiration; set => expiration = value; }
        [DataProperty("InstanceLimit")]
        public int? InstanceLimit { get => instanceLimit; set => instanceLimit = value; }
        [DataProperty("CustomerAccount")]
        public int? CustomerAccountID { get => customerAccountID; set => customerAccountID = value; }

        public List<IntelliSEMUser> AssociatedUsers
        {
            get { return associatedUsers; }
        }

        public int AssociatedUsersCount
        {
            get { return AssociatedUsers.Count; }
        }

        public int ID { get => id; set => id = value; }
        private bool wasLoaded;
        public bool WasLoaded { get => wasLoaded; set => wasLoaded = value; }

        public LoginKey() { }

        public LoginKey(string key) : this(key, null, null, null) { }

        public LoginKey(string key, int customerId) : this(key, customerId, null, null) { }

        public LoginKey(string key, int? customerId, int? instanceLimit, DateTime? expiration)
        {
            this.key = key;
            this.customerAccountID = customerId;
            createdAt = DateTime.Now;
            this.instanceLimit = instanceLimit;
            this.expiration = expiration;
        }

        public bool IsValid(string fingerprint)
        {
            if (expiration.HasValue && DateTime.Now > expiration.Value)
            {
                return false;
            }
            if (instanceLimit.HasValue)
            {
                var associatedUser = associatedUsers.Where(a => a.HardwareFingerprint.Equals(fingerprint, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (associatedUser != null)
                {
                    return true;
                }
                else
                {
                    return associatedUsers.Count < instanceLimit.Value;  // is there room for another user?
                }
            }

            return true;
        }

        public static List<LoginKey> LoadAll(List<IntelliSEMUser> users)
        {
            var loginKeys = new List<LoginKey>();
            var loginKeysRaw = Data<LoginKey>.LoadAll();
            foreach (var loginKey in loginKeysRaw)
            {
                loginKey.associatedUsers = Data<IntelliSEMUser>.LoadManyFromProc("GetUsersForLoginKeys", new string[] { "LoginKey" }, new object[] { loginKey.ID }).ToList();
                loginKeys.Add(loginKey);
            }
            return loginKeys;
        }

        #region Overrides

        public override bool Equals(object obj)
        {
            return obj is LoginKey key &&
                   id == key.id;
        }

        public override int GetHashCode()
        {
            return 1877310944 + id.GetHashCode();
        }

        public static bool operator ==(LoginKey left, LoginKey right)
        {
            return EqualityComparer<LoginKey>.Default.Equals(left, right);
        }

        public static bool operator !=(LoginKey left, LoginKey right)
        {
            return !(left == right);
        }

        #endregion
    }
}