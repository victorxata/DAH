using System.Configuration;

namespace TechTracker.Common.Utils.Configuration
{
    public class AppConfigManager : IAppConfigManager
    {
        public string this[string key] => GetSetting(key);

        public virtual string GetSetting(string keyName)
        {
            var value = ConfigurationManager.AppSettings[keyName];
            if (value == null)
                throw new ConfigurationErrorsException($"Expecting an App.Config setting for {keyName}");

            return value;
        }

        public string GetConnectionString(string keyName)
        {
            var entry = ConfigurationManager.ConnectionStrings[keyName];
            if (entry == null)
                throw new ConfigurationErrorsException($"Cannot find a Web.Config or App.Config connection string with the key {keyName}.");

            return entry.ConnectionString;
        }
    }

    public interface IAppConfigManager
    {
        string this[string key] { get; }
        string GetSetting(string keyName);
        string GetConnectionString(string keyName);
    }
}