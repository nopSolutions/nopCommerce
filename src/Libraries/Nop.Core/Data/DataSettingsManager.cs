using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace Nop.Core.Data
{
    /// <summary>
    /// Manager of data settings (connection string)
    /// </summary>
    public partial class DataSettingsManager
    {
        protected const char Separator = ':';
        protected const string DefaultFilename = "Settings.txt";
        protected static DataSettings DataSettings;

        public string FilePath
        {
            get
            {
                var fileName = ConfigurationManager.AppSettings["NopCommerce.SettingsFile"] ?? DefaultFilename;
                return Path.Combine(CommonHelper.MapPath("~/App_Data/"), fileName);
            }
        }

        /// <summary>
        /// Parse settings
        /// </summary>
        /// <param name="text">Text of settings file</param>
        /// <returns>Parsed data settings</returns>
        protected virtual DataSettings ParseSettings(string text)
        {
            var shellSettings = new DataSettings();
            if (string.IsNullOrEmpty(text))
                return shellSettings;

            //Old way of file reading. This leads to unexpected behavior when a user's FTP program transfers these files as ASCII (\r\n becomes \n).
            //var settings = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var settings = new List<string>();
            using (var reader = new StringReader(text))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                    settings.Add(str);
            }

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

        /// <summary>
        /// Convert data settings to string representation
        /// </summary>
        /// <param name="settings">Settings</param>
        /// <returns>Text</returns>
        protected virtual string ComposeSettings(DataSettings settings)
        {
            if (settings == null)
                return "";

            return $"DataProvider: {settings.DataProvider}{Environment.NewLine}DataConnectionString: {settings.DataConnectionString}{Environment.NewLine}";
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <returns></returns>
        public virtual DataSettings LoadSettings()
        {
            if (DataSettings != null)
                return DataSettings;
            
            if (!File.Exists(FilePath))
                return new DataSettings();

            var text = File.ReadAllText(FilePath);
            DataSettings = ParseSettings(text);
            return DataSettings;
        }

        /// <summary>
        /// Save settings to a file
        /// </summary>
        /// <param name="settings"></param>
        public virtual void SaveSettings(DataSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            
            if (!File.Exists(FilePath))
            {
                using (File.Create(FilePath))
                {
                    //we use 'using' to close the file after it's created
                }
            }
            
            var text = ComposeSettings(settings);
            File.WriteAllText(FilePath, text);

            DataSettings = null;
        }
    }
}
