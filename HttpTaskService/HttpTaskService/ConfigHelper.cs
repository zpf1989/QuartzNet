using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpTaskService
{
    public class ConfigHelper
    {
        public static string GetAppSetting(string key, string defaultValue)
        {
            if (string.IsNullOrEmpty(key))
            {
                return defaultValue;
            }
            var appSettings = ConfigurationManager.AppSettings;
            if (!ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                return defaultValue;
            }

            return appSettings[key];
        }
    }
}
