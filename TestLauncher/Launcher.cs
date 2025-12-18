using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestLauncher
{
    public partial class Launcher : Form
    {
        private string loginKey;
        private string latestAvailableVersion;

        public Launcher()
        {
            InitializeComponent();
            loginKey = LoadKey();
            if (string.IsNullOrEmpty(loginKey))
            {
                using (KeyEntry f = new KeyEntry())
                {
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        loginKey = f.LoginKey;
                        SaveKey(loginKey);
                    }
                }
            }

            latestAvailableVersion = GetMostRecentVersion();
            if (string.Compare(GetExtractedExeVersion(), latestAvailableVersion) >= 0)
            {
                MessageBox.Show("You are using the latest version.", "Up to Date", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            MessageBox.Show($"A newer version ({latestAvailableVersion}) is available. Please update.", "Update Available", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DownloadLatestVersionZip(latestAvailableVersion);
            ExtractFiles();
        }

        private string LoadKey()
        {
            string key = ConfigurationManager.AppSettings["LoginKey"];
            return string.IsNullOrEmpty(key) ? string.Empty : key;
        }

        private void SaveKey(string key)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["LoginKey"].Value = key;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private string GetMostRecentVersion()
        {
            // build payload
            string macAddress = "";
            try
            {
                var nic = System.Net.NetworkInformation.NetworkInterface
                    .GetAllNetworkInterfaces()
                    .FirstOrDefault(n => n.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                                         n.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback);
                if (nic != null)
                {
                    macAddress = string.Join(":", nic.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")));
                }
            }
            catch { }

            var payload = new
            {
                loginKey,
                macAddress
            };

            // get most recent version from server
            bool validationSuccess = false;
            string mostRecentVersion = "Unknown";
            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44395/");
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                var content = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");
                try
                {
                    var response = client.PostAsync("IntelliSEMLogin/ValidateKey", content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;
                        dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);
                        MessageBox.Show($"Validation successful!\nVersion: {result.version}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        mostRecentVersion = result.version;
                        validationSuccess = true;
                    }
                    else
                    {
                        MessageBox.Show("Validation failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        validationSuccess = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    validationSuccess = false;
                }
            }

            // return value
            if (validationSuccess)
                return mostRecentVersion;
            MessageBox.Show("Key validation failed. Please check your key and try again.", "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }

        private void DownloadLatestVersionZip(string version)
        {
            string tempZipPath = Path.Combine(Path.GetTempPath(), $"IntelliSEM_{version}.zip");
            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44395/");
                try
                {
                    // Build payload with key and version
                    var payload = new
                    {
                        loginKey,
                        version
                    };
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                    var content = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync("IntelliSEMLogin/DownloadVersionZip", content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var zipBytes = response.Content.ReadAsByteArrayAsync().Result;
                        File.WriteAllBytes(tempZipPath, zipBytes);
                        MessageBox.Show($"Downloaded latest version zip to:\n{tempZipPath}", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to download the latest version zip.", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error downloading zip: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExtractFiles()
        {
            string extractionFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExtractionDestination");
            Directory.CreateDirectory(extractionFolder);

            // Find the most recent IntelliSEM zip in temp folder
            string tempPath = Path.GetTempPath();
            string zipFile = Directory.GetFiles(tempPath, "IntelliSEM_*.zip")
                .OrderByDescending(f => File.GetCreationTime(f))
                .FirstOrDefault();

            if (zipFile == null)
            {
                MessageBox.Show("No IntelliSEM zip file found in temp folder.", "Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Extract zip
                ZipFile.ExtractToDirectory(zipFile, extractionFolder);
                // Remove the zip file after extraction
                File.Delete(zipFile);
                MessageBox.Show($"Extraction complete!\nExtracted to: {extractionFolder}\nZip file deleted.", "Extraction Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error extracting zip: " + ex.Message, "Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetExtractedExeVersion()
        {
            string extractionFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExtractionDestination");
            string exePath = Path.Combine(extractionFolder, "IntelliSEM.exe");
            if (!File.Exists(exePath))
                return null;

            try
            {
                var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(exePath);
                return versionInfo.FileVersion;
            }
            catch
            {
                return null;
            }
        }
    }
}
