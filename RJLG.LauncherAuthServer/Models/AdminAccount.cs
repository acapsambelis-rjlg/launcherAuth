using SNS.Data.DataSerializer;
using SNS.Data.DataSerializer.DataExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace RJLG.LauncherAuthServer.Models
{
    public static class AdminAccounts
    {
        public static bool IsValid(string username, string password)
        {
            AdminAccount account = Data<AdminAccount>.LoadManyFromProc("GetAdminAccount", new string[] { "username" }, new object[] { username }).FirstOrDefault();
            return account != null && VerifyPassword(password, account.PasswordHash);
        }

        public static void StoreAdminAccount(string username, string password)
        {
            AdminAccount account = new AdminAccount()
            {
                Username = username,
                PasswordHash = HashPassword(password)
            };
            account.Save();
        }

        public static void UpdateAdminPassword(string username, string newPassword)
        {
            AdminAccount account = Data<AdminAccount>.LoadManyFromProc("GetAdminAccount", new string[] { "username" }, new object[] { username }).FirstOrDefault();
            if (account != null)
            {
                account.PasswordHash = HashPassword(newPassword);
                account.Update();
            }
        }

        public static string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Hash the password with the salt
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(20);
                // Combine salt and hash
                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);
                // Convert to base64 for storage
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            // Extract bytes
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Hash the input password with the extracted salt
            using (var pbkdf2 = new Rfc2898DeriveBytes(inputPassword, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(20);
                // Compare the results
                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                        return false;
                }
            }
            return true;
        }

        [DataClass("AdminAccounts")]
        internal class AdminAccount : IGenericDataObject<AdminAccount>
        {
            private string username;
            private string passwordHash;
            [DataProperty("Username", Key = true)]
            public string Username { get => username; set => username = value; }
            [DataProperty("Password")]
            public string PasswordHash { get => passwordHash; set => passwordHash = value; }

            private bool wasLoaded = false;
            public bool WasLoaded { get => wasLoaded; set => wasLoaded = value; }
        }
    }
}