using RJLG.LauncherAuthServer.Controllers;
using SNS.Data.DataSerializer;
using SNS.Data.DataSerializer.DataExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RJLG.LauncherAuthServer.Models
{
    [DataClass("CustomerAccounts")]
    public class CustomerAccount : IIDDataObject<CustomerAccount>
    {
        public static readonly int DEFAULT_MAX_KEYS = 5;

        private int id;
        private string customerName;
        private List<LoginKey> loginKeys;
        private int keysAllowed;
        private int uniqueMacCount;
        private string currentVersion;
        private string contactName;
        private string contactEmail;
        private string timezone;
        private string notes;

        [DataProperty("ID", Key = true)]
        public int ID { get => id; set => id = value; }

        [DataProperty("Name")]
        public string CustomerName { get => customerName; set => customerName = value; }

        public List<LoginKey> LoginKeys { get => loginKeys; set => loginKeys = value; }

        [DataProperty("KeysAllowed")]
        public int KeysAllowed { get => keysAllowed; set => keysAllowed = value; }
        
        [DataProperty("UniqueMacCount")]
        private int UniqueMacCount { get => uniqueMacCount; set => uniqueMacCount = value; }

        [DataProperty("CurrentVersion")]
        public string CurrentVersion { get => currentVersion; set => currentVersion = value; }

        [DataProperty("ContactName")]
        public string ContactName { get => contactName; set => contactName = value; }

        [DataProperty("ContactEmail")]
        public string ContactEmail { get => contactEmail; set => contactEmail = value; }

        [DataProperty("Timezone")]
        public string Timezone { get => timezone; set => timezone = value; }

        [DataProperty("Notes")]
        public string Notes { get => notes; set => notes = value; }


        private bool wasLoaded = false;
        public bool WasLoaded { get => wasLoaded; set => wasLoaded = value; }

        public CustomerAccount() { }

        public CustomerAccount(string customerName, IntelliSEMVersion currentVersion, string contactName, string contactEmail, string timezone, int? keysAllowed = null)
        {
            CustomerName = customerName;
            KeysAllowed = keysAllowed != null ? keysAllowed.Value : DEFAULT_MAX_KEYS;
            CurrentVersion = currentVersion.Version;
            ContactName = contactName;
            ContactEmail = contactEmail;
            LoginKeys = new List<LoginKey>();
            Timezone = timezone;
        }

        public static List<CustomerAccount> LoadAll()
        {
            List<CustomerAccount> accounts = new List<CustomerAccount>();
            var accountsRaw = Data<CustomerAccount>.LoadAll();
            foreach (var account in accountsRaw)
            {
                account.loginKeys = Data<LoginKey>.LoadManyFromProc("GetLoginKeysForCustomer", new string[] { "CustomerId" }, new object[] { account.ID }).ToList();
                accounts.Add(account);
            }
            return accounts;
        }

        #region Operators

        public override string ToString()
        {
            return CustomerName;
        }

        #endregion
    }
}