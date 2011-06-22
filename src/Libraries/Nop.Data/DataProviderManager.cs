using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using Nop.Core;

namespace Nop.Data
{
    public partial class DataProviderManager
    {
        public const char Separator = ':';
        public const string Filename = "Settings.txt";

        protected virtual DataProviderSettings ParseSettings(string text)
        {
            var shellSettings = new DataProviderSettings();
            if (String.IsNullOrEmpty(text))
                return shellSettings;

            var settings = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var setting in settings)
            {
                var separatorIndex = setting.IndexOf(Separator);
                if (separatorIndex == -1)
                {
                    continue;
                }
                string key = setting.Substring(0, separatorIndex).Trim();
                string value = setting.Substring(separatorIndex + 1).Trim();

                switch (key)
                {
                    case "DataProvider":
                        shellSettings.DataProvider = value;
                        break;
                    case "DataConnectionString":
                        shellSettings.DataConnectionString = value;
                        break;
                }
            }

            return shellSettings;
        }

        protected virtual string ComposeSettings(DataProviderSettings settings)
        {
            if (settings == null)
                return "";

            return string.Format("DataProvider: {0}\r\nDataConnectionString: {1}\r\n",
                                 settings.DataProvider,
                                 settings.DataConnectionString
                );
        }

        public virtual DataProviderSettings LoadSettings()
        {
            string filePath = Path.Combine(HostingEnvironment.MapPath("~/App_Data/"), Filename);
            if (File.Exists(filePath))
            {
                string text = File.ReadAllText(filePath);
                return ParseSettings(text);
            }
            else
                return new DataProviderSettings();
        }

        public virtual void SaveSettings(DataProviderSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            
            string filePath = Path.Combine(HostingEnvironment.MapPath("~/App_Data/"), Filename);
            if (!File.Exists(filePath))
                using (File.Create(filePath))
                {
                    //we use 'using' to close the file after it's created
                }
            
            var text = ComposeSettings(settings);
            File.WriteAllText(filePath, text);
        }

        public virtual IDataProvider LoadDataProvider(string providerName)
        {
            if (String.IsNullOrWhiteSpace(providerName))
                throw new ArgumentNullException("providerName");

            switch (providerName.ToLowerInvariant())
            {
                case "sqlserver":
                    return new SqlServerDataProvider();
                case "sqlce":
                    return new SqlCeDataProvider();
                default:
                    throw new NopException(string.Format("Not supported dataprovider name: {0}", providerName));
            }
        }
    }
}
