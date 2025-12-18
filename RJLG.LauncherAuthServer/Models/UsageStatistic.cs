using SNS.Data.DataSerializer;
using SNS.Data.DataSerializer.DataExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RJLG.LauncherAuthServer.Models
{
    [DataClass("UsageStatistics")]
    public class UsageStatistic : IGenericDataObject<UsageStatistic>
    {
        private string name;
        private int? count;
        private string value;

        [DataProperty("Name", Key = true)]
        public string Name { get => name; set => name = value; }

        [DataProperty("Count")]
        public int? Count { get => count; set { this.count = value; } }
        [DataProperty("Value")]
        public string Value { get => value; set { this.value = value; } }


        private bool wasLoaded;
        public bool WasLoaded { get => wasLoaded; set => wasLoaded = value; }

        public UsageStatistic() { }

        public UsageStatistic(string name, int? count = null, string value = "")
        {
            this.name = name;
            this.count = count;
            this.value = value;
        }

        public static Dictionary<string, UsageStatistic> LoadAll(int versionCount, int customerCount, int keyCount)
        {
            Dictionary<string, UsageStatistic> stats = new Dictionary<string, UsageStatistic>();
            var allStats = Data<UsageStatistic>.LoadAll();
            foreach (var stat in allStats)
            {
                stats[stat.Name] = stat;
            }

            stats["Versions Uploaded"] = new UsageStatistic("Versions Uploaded", versionCount, null);
            stats["Total Customer Accounts"] = new UsageStatistic("Total Customer Accounts", customerCount, null);
            stats["Total Keys"] = new UsageStatistic("Total Keys", keyCount, null);
            stats["Startup Time"] = new UsageStatistic("Startup Time", null, DateTime.Now.ToString());
            return stats;
        }
    }
}