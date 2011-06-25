using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using Nop.Core;

namespace Nop.Core.Data
{
    public partial class SettingsManager
    {
        public const char Separator = ':';
        public const string Filename = "Settings.txt";

        protected virtual Settings ParseSettings(string text)
        {
            var shellSettings = new Settings();
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
                    default:
                        shellSettings.RawDataSettings.Add(key,value);
                        break;
                }
            }

            return shellSettings;
        }

        protected virtual string ComposeSettings(Settings settings)
        {
            if (settings == null)
                return "";

            return string.Format("DataProvider: {0}\r\nDataConnectionString: {1}\r\n",
                                 settings.DataProvider,
                                 settings.DataConnectionString
                );
        }

        public virtual Settings LoadSettings()
        {
            string filePath = Path.Combine(HostingEnvironment.MapPath("~/App_Data/"), Filename);
            if (File.Exists(filePath))
            {
                string text = File.ReadAllText(filePath);
                return ParseSettings(text);
            }
            else
                return new Settings();
        }

        public virtual void SaveSettings(Settings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            
            string filePath = Path.Combine(HostingEnvironment.MapPath("~/App_Data/"), Filename);
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
