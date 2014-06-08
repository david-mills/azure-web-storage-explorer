using System.Configuration;
using System.Linq;

namespace Azure.Web.Storage.Configuration
{
    public static class Settings
    {
        public static string StorageConfiguration
        {
            get { return ConfigurationManager.AppSettings["StorageConnectionString"]; }
        }

        public static string[] AvailableRoles
        {
            get { return ConfigurationManager.AppSettings["AvailableRoles"].Split('|').ToArray(); }
        }
    }
}