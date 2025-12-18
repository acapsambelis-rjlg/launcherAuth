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
        private int? customerAccount;

        [DataProperty("Key", Key = true)]
        public string Key { get => key; set => key = value; }
        [DataProperty("CreatedAt")]
        public DateTime CreatedAt { get => createdAt; set => createdAt = value; }
        [DataProperty("Expiration")]
        public DateTime? Expiration { get => expiration; set => expiration = value; }
        public int? InstanceLimit { get => instanceLimit; set => instanceLimit = value; }
        [DataProperty("CustomerAccount")]
        public int? CustomerAccount { get => customerAccount; set => customerAccount = value; }
        public int AssociatedUsersCount
        {
            get
            {
                var users = IntelliSEMUser.LoadAll(key);
                return users.Length;
            }
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
            this.customerAccount = customerId;
            createdAt = DateTime.Now;
            this.instanceLimit = instanceLimit;
            this.expiration = expiration;
        }

        public bool IsValid()
        {
            if (expiration.HasValue && DateTime.Now > expiration.Value)
            {
                return false;
            }
            //if (instanceLimit.HasValue)
            //{
            //    var uniqueMacCount = associatedUsers
            //        .Select(u => u.MacAddress)
            //        .Where(mac => !string.IsNullOrEmpty(mac))
            //        .Distinct(StringComparer.OrdinalIgnoreCase)
            //        .Count();
            //    return uniqueMacCount < instanceLimit.Value;
            //}
            return true;
        }

        public static LoginKey[] LoadAll()
        {
            var loginKeysRaw = Data<LoginKey>.LoadAll();
            //var usersRaw = Data<IntelliSEMUser>.LoadAll();


            return loginKeysRaw;
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