using System;
using System.IO;
using Newtonsoft.Json;
using Nop.Core.Infrastructure;

namespace Nop.Core.Data
{
    /// <summary>
    /// Represents the data settings manager
    /// </summary>
    public partial class DataSettingsManager
    {
        #region Fields

        private static bool? _databaseIsInstalled;

        #endregion

        #region Methods

        /// <summary>
        /// Load data settings
        /// </summary>
        /// <param name="filePath">File path; pass null to use the default settings file</param>
        /// <param name="reloadSettings">Whether to reload data, if they already loaded</param>
        /// <returns>Data settings</returns>
        public static DataSettings LoadSettings(string filePath = null, bool reloadSettings = false)
        {
            if (!reloadSettings && Singleton<DataSettings>.Instance != null)
                return Singleton<DataSettings>.Instance;

            filePath = filePath ?? CommonHelper.MapPath(DataSettingsFilePath);

            //check whether file exists
            if (!File.Exists(filePath))
            {
                //if not, try to parse the file that was used in previous nopCommerce versions
                filePath = CommonHelper.MapPath(ObsoleteDataSettingsFilePath);
                if (!File.Exists(filePath))
                    return new DataSettings();

                //get data settings from the old txt file
                var dataSettings = new DataSettings();
                using (var reader = new StringReader(File.ReadAllText(filePath)))
                {
                    var settingsLine = string.Empty;
                    while ((settingsLine = reader.ReadLine()) != null)
                    {
                        var separatorIndex = settingsLine.IndexOf(':');
                        if (separatorIndex == -1)
                            continue;

                        var key = settingsLine.Substring(0, separatorIndex).Trim();
                        var value = settingsLine.Substring(separatorIndex + 1).Trim();

                        switch (key)
                        {
                            case "DataProvider":
                                dataSettings.DataProvider = Enum.TryParse(value, true, out DataProviderType providerType) ? providerType : DataProviderType.Unknown;
                                continue;
                            case "DataConnectionString":
                                dataSettings.DataConnectionString = value;
                                continue;
                            default:
                                dataSettings.RawDataSettings.Add(key, value);
                                continue;
                        }
                    }
                }

                //save data settings to the new file
                SaveSettings(dataSettings);

                //and delete the old one
                File.Delete(filePath);

                Singleton<DataSettings>.Instance = dataSettings;
                return Singleton<DataSettings>.Instance;
            }

            var text = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(text))
                return new DataSettings();

            //get data settings from the JSON file
            Singleton<DataSettings>.Instance = JsonConvert.DeserializeObject<DataSettings>(text);

            return Singleton<DataSettings>.Instance;
        }

        /// <summary>
        /// Save data settings to the file
        /// </summary>
        /// <param name="settings">Data settings</param>
        public static void SaveSettings(DataSettings settings)
        {
            Singleton<DataSettings>.Instance = settings ?? throw new ArgumentNullException(nameof(settings));

            var filePath = CommonHelper.MapPath(DataSettingsFilePath);

            //create file if not exists
            if (!File.Exists(filePath))
            {
                //we use 'using' to close the file after it's created
                using (File.Create(filePath)) { }
            }

            //save data settings to the file
            var text = JsonConvert.SerializeObject(Singleton<DataSettings>.Instance, Formatting.Indented);
            File.WriteAllText(filePath, text);
        }

        /// <summary>
        /// Reset "database is installed" cached information
        /// </summary>
        public static void ResetCache()
        {
            _databaseIsInstalled = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a path to the file that was used in old nopCommerce versions to contain data settings
        /// </summary>
        protected static string ObsoleteDataSettingsFilePath => "~/App_Data/Settings.txt";

        /// <summary>
        /// Gets a path to the file that contains data settings
        /// </summary>
        public static string DataSettingsFilePath => "~/App_Data/dataSettings.json";

        /// <summary>
        /// Gets a value indicating whether database is already installed
        /// </summary>
        public static bool DatabaseIsInstalled
        {
            get
            {
                if (!_databaseIsInstalled.HasValue)
                    _databaseIsInstalled = !string.IsNullOrEmpty(LoadSettings(reloadSettings: true)?.DataConnectionString);

                return _databaseIsInstalled.Value;
            }
        }

        #endregion
    }
}