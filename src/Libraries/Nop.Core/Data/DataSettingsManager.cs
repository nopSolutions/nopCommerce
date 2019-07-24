using System;
using System.IO;
using System.Text;
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
        /// <param name="fileProvider">File provider</param>
        /// <returns>Data settings</returns>
        public static DataSettings LoadSettings(string filePath = null, bool reloadSettings = false, INopFileProvider fileProvider = null)
        {
            if (!reloadSettings && Singleton<DataSettings>.Instance != null)
                return Singleton<DataSettings>.Instance;

            fileProvider = fileProvider ?? CommonHelper.DefaultFileProvider;
            filePath = filePath ?? fileProvider.MapPath(NopDataSettingsDefaults.FilePath);

            //check whether file exists
            if (!fileProvider.FileExists(filePath))
            {
                //if not, try to parse the file that was used in previous nopCommerce versions
                filePath = fileProvider.MapPath(NopDataSettingsDefaults.ObsoleteFilePath);
                if (!fileProvider.FileExists(filePath))
                    return new DataSettings();

                //get data settings from the old txt file
                var dataSettings = new DataSettings();
                using (var reader = new StringReader(fileProvider.ReadAllText(filePath, Encoding.UTF8)))
                {
                    string settingsLine;
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
                SaveSettings(dataSettings, fileProvider);

                //and delete the old one
                fileProvider.DeleteFile(filePath);

                Singleton<DataSettings>.Instance = dataSettings;
                return Singleton<DataSettings>.Instance;
            }

            var text = fileProvider.ReadAllText(filePath, Encoding.UTF8);
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
        /// <param name="fileProvider">File provider</param>
        public static void SaveSettings(DataSettings settings, INopFileProvider fileProvider = null)
        {
            Singleton<DataSettings>.Instance = settings ?? throw new ArgumentNullException(nameof(settings));

            fileProvider = fileProvider ?? CommonHelper.DefaultFileProvider;
            var filePath = fileProvider.MapPath(NopDataSettingsDefaults.FilePath);

            //create file if not exists
            fileProvider.CreateFile(filePath);

            //save data settings to the file
            var text = JsonConvert.SerializeObject(Singleton<DataSettings>.Instance, Formatting.Indented);
            fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
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