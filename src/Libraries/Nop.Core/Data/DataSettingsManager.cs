using System;
using System.IO;
using System.Web.Hosting;

namespace Nop.Core.Data
{
    public partial class DataSettingsManager
    {
        protected const char separator = ':';
        protected const string filename = "Settings.txt";

        protected virtual DataSettings ParseSettings(string text)
        {
            var shellSettings = new DataSettings();
            if (String.IsNullOrEmpty(text))
                return shellSettings;

            var settings = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var setting in settings)
            {
                var separatorIndex = setting.IndexOf(separator);
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
                    default:
                        shellSettings.RawDataSettings.Add(key,value);
                        break;
                }
            }

            return shellSettings;
        }

        protected virtual string ComposeSettings(DataSettings settings)
        {
            if (settings == null)
                return "";

            return string.Format("DataProvider: {0}\r\nDataConnectionString: {1}\r\n",
                                 settings.DataProvider,
                                 settings.DataConnectionString
                );
        }

        public virtual DataSettings LoadSettings()
        {
            string filePath = Path.Combine(HostingEnvironment.MapPath("~/App_Data/"), filename);
            if (File.Exists(filePath))
            {
                string text = File.ReadAllText(filePath);
                return ParseSettings(text);
            }
            else
                return new DataSettings();
        }

        public virtual void SaveSettings(DataSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            
            string filePath = Path.Combine(HostingEnvironment.MapPath("~/App_Data/"), filename);
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath))
                {
                    //we use 'using' to close the file after it's created
                }
            }
            
            var text = ComposeSettings(settings);
            File.WriteAllText(filePath, text);
        }
    }
}
