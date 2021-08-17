using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

namespace Nop.Data
{
    /// <summary>
    /// Represents the data settings manager
    /// </summary>
    public partial class DataSettingsManager
    {
        #region Fields

        private static bool? _databaseIsInstalled;

        #endregion

        #region Utils

        /// <summary>
        /// Gets data settings from the old txt file
        /// </summary>
        /// <param name="data">Old txt file data</param>
        /// <returns>Data settings</returns>
        protected static DataConfig LoadDataSettingsFromOldTxtFile(string data)
        {
            var dataSettings = new DataConfig();
            using var reader = new StringReader(data);
            string settingsLine;
            while ((settingsLine = reader.ReadLine()) != null)
            {
                var separatorIndex = settingsLine.IndexOf(':');
                if (separatorIndex == -1)
                    continue;

                var key = settingsLine[0..separatorIndex].Trim();
                var value = settingsLine[(separatorIndex + 1)..].Trim();

                switch (key)
                {
                    case "DataProvider":
                        dataSettings.DataProvider = Enum.TryParse(value, true, out DataProviderType providerType) ? providerType : DataProviderType.Unknown;
                        continue;
                    case "DataConnectionString":
                        dataSettings.ConnectionString = value;
                        continue;
                    case "SQLCommandTimeout":
                        //If parsing isn't successful, we set a negative timeout, that means the current provider will usе a default value
                        dataSettings.SQLCommandTimeout = int.TryParse(value, out var timeout) ? timeout : -1;
                        continue;
                    default:
                        break;
                }
            }

            return dataSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load data settings
        /// </summary>
        /// <param name="reloadSettings">Whether to reload data, if they already loaded</param>
        /// <param name="fileProvider">File provider</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the data settings
        /// </returns>
        public static async Task<DataConfig> LoadSettingsAsync(bool reloadSettings = false, INopFileProvider fileProvider = null)
        {
            if (!reloadSettings && Singleton<DataConfig>.Instance != null)
                return Singleton<DataConfig>.Instance;

            fileProvider ??= CommonHelper.DefaultFileProvider;
            var filePath_json = fileProvider.MapPath(NopDataSettingsDefaults.FilePath);
            var filePath_txt = fileProvider.MapPath(NopDataSettingsDefaults.ObsoleteFilePath);

            var filePath = filePath_json;

            if (fileProvider.FileExists(filePath_json) || fileProvider.FileExists(filePath_txt))
            {
                var dataSettingsObj = new DataConfig();
                //check whether file exists (JSON)
                if (fileProvider.FileExists(filePath_json))
                {
                    var text = await fileProvider.ReadAllTextAsync(filePath_json, Encoding.UTF8);
                    if (string.IsNullOrEmpty(text))
                        return dataSettingsObj;

                    //get data settings from the JSON file
                    var objDefinition = new { DataConnectionString = "", DataProvider = DataProviderType.SqlServer, SQLCommandTimeout = "" };
                    var jsonDataSettings = JsonConvert.DeserializeAnonymousType(text, objDefinition);
                    dataSettingsObj = new DataConfig
                    {
                        ConnectionString = jsonDataSettings.DataConnectionString,
                        DataProvider = jsonDataSettings.DataProvider,
                        SQLCommandTimeout = int.TryParse(jsonDataSettings.SQLCommandTimeout, out var result) ? result : null
                    };
                }
                else
                {
                    if (fileProvider.FileExists(filePath_txt))
                    {
                        dataSettingsObj = LoadDataSettingsFromOldTxtFile(fileProvider.ReadAllText(filePath_txt, Encoding.UTF8));
                        filePath = filePath_txt;
                    }
                }
                //save data settings to the new file
                await SaveSettingsAsync(dataSettingsObj, fileProvider);

                //and delete the old one
                fileProvider.DeleteFile(filePath);

                Singleton<DataConfig>.Instance = dataSettingsObj;
                return Singleton<DataConfig>.Instance;
            }

            return Singleton<AppSettings>.Instance.DataConfig;
        }

        /// <summary>
        /// Load data settings
        /// </summary>
        /// <param name="reloadSettings">Whether to reload data, if they already loaded</param>
        /// <param name="fileProvider">File provider</param>
        /// <returns>Data settings</returns>
        public static DataConfig LoadSettings(bool reloadSettings = false, INopFileProvider fileProvider = null)
        {
            if (!reloadSettings && Singleton<DataConfig>.Instance != null)
                return Singleton<DataConfig>.Instance;

            fileProvider ??= CommonHelper.DefaultFileProvider;
            var filePath_json = fileProvider.MapPath(NopDataSettingsDefaults.FilePath);
            var filePath_txt = fileProvider.MapPath(NopDataSettingsDefaults.ObsoleteFilePath);

            var filePath = filePath_json;

            if (fileProvider.FileExists(filePath_json) || fileProvider.FileExists(filePath_txt))
            {
                var dataSettingsObj = new DataConfig();
                //check whether file exists (JSON)
                if (fileProvider.FileExists(filePath_json))
                {
                    var text = fileProvider.ReadAllText(filePath_json, Encoding.UTF8);
                    if (string.IsNullOrEmpty(text))
                        return dataSettingsObj;

                    //get data settings from the JSON file
                    var objDefinition = new { DataConnectionString = "", DataProvider = DataProviderType.SqlServer, SQLCommandTimeout = "" };
                    var jsonDataSettings = JsonConvert.DeserializeAnonymousType(text, objDefinition);
                    dataSettingsObj = new DataConfig
                    {
                        ConnectionString = jsonDataSettings.DataConnectionString,
                        DataProvider = jsonDataSettings.DataProvider,
                        SQLCommandTimeout = int.TryParse(jsonDataSettings.SQLCommandTimeout, out var result) ? result : null
                    };
                }
                else
                {
                    if (fileProvider.FileExists(filePath_txt))
                    {
                        dataSettingsObj = LoadDataSettingsFromOldTxtFile(fileProvider.ReadAllText(filePath_txt, Encoding.UTF8));
                        filePath = filePath_txt;
                    }
                }
                //save data settings to the new file
                SaveSettings(dataSettingsObj, fileProvider);

                //and delete the old one
                fileProvider.DeleteFile(filePath);

                Singleton<DataConfig>.Instance = dataSettingsObj;
                return Singleton<DataConfig>.Instance;
            }

            return Singleton<AppSettings>.Instance.DataConfig;
        }

        /// <summary>
        /// Save data settings to the file
        /// </summary>
        /// <param name="settings">Data settings</param>
        /// <param name="fileProvider">File provider</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task SaveSettingsAsync(DataConfig settings, INopFileProvider fileProvider = null)
        {
            Singleton<DataConfig>.Instance = settings ?? throw new ArgumentNullException(nameof(settings));

            fileProvider ??= CommonHelper.DefaultFileProvider;
            var filePath = fileProvider.MapPath(NopDataSettingsDefaults.AppSettingsFilePath);

            var appSettings = Singleton<AppSettings>.Instance;
            appSettings.DataConfig = new DataConfig
            {
                ConnectionString = settings.ConnectionString,
                DataProvider = settings.DataProvider,
                SQLCommandTimeout = settings.SQLCommandTimeout
            };

            //create file if not exists
            if (!fileProvider.FileExists(filePath))
                fileProvider.CreateFile(filePath);

            //check additional configuration parameters
            var additionalData = JsonConvert.DeserializeObject<AppSettings>(await fileProvider.ReadAllTextAsync(filePath, Encoding.UTF8))?.AdditionalData;
            appSettings.AdditionalData = additionalData;

            //save data app settings to the file
            var text = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
            fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
        }

        /// <summary>
        /// Save data settings to the file
        /// </summary>
        /// <param name="settings">Data settings</param>
        /// <param name="fileProvider">File provider</param>
        public static void SaveSettings(DataConfig settings, INopFileProvider fileProvider = null)
        {
            Singleton<DataConfig>.Instance = settings ?? throw new ArgumentNullException(nameof(settings));

            fileProvider ??= CommonHelper.DefaultFileProvider;
            var filePath = fileProvider.MapPath(NopDataSettingsDefaults.AppSettingsFilePath);

            var appSettings = Singleton<AppSettings>.Instance;
            appSettings.DataConfig = new DataConfig
            {
                ConnectionString = settings.ConnectionString,
                DataProvider = settings.DataProvider,
                SQLCommandTimeout = settings.SQLCommandTimeout
            };

            //create file if not exists
            if (!fileProvider.FileExists(filePath))
                fileProvider.CreateFile(filePath);

            //check additional configuration parameters
            var additionalData = JsonConvert.DeserializeObject<AppSettings>(fileProvider.ReadAllText(filePath, Encoding.UTF8))?.AdditionalData;
            appSettings.AdditionalData = additionalData;

            //save data app settings to the file
            var text = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
            fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
        }

        /// <summary>
        /// Reset "database is installed" cached information
        /// </summary>
        public static void ResetCache()
        {
            _databaseIsInstalled = null;
        }

        /// <summary>
        /// Gets a value indicating whether database is already installed
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task<bool> IsDatabaseInstalledAsync()
        {
            _databaseIsInstalled ??= !string.IsNullOrEmpty((await LoadSettingsAsync(reloadSettings: true))?.ConnectionString);

            return _databaseIsInstalled.Value;
        }

        /// <summary>
        /// Gets a value indicating whether database is already installed
        /// </summary>
        public static bool IsDatabaseInstalled()
        {
            _databaseIsInstalled ??= !string.IsNullOrEmpty(LoadSettings(reloadSettings: true)?.ConnectionString);

            return _databaseIsInstalled.Value;
        }

        /// <summary>
        /// Gets the command execution timeout.
        /// </summary>
        /// <value>
        /// Number of seconds. Negative timeout value means that a default timeout will be used. 0 timeout value corresponds to infinite timeout.
        /// </value>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task<int> GetSqlCommandTimeoutAsync()
        {
            return (await LoadSettingsAsync())?.SQLCommandTimeout ?? -1;
        }

        /// <summary>
        /// Gets the command execution timeout.
        /// </summary>
        /// <value>
        /// Number of seconds. Negative timeout value means that a default timeout will be used. 0 timeout value corresponds to infinite timeout.
        /// </value>
        public static int GetSqlCommandTimeout()
        {
            return LoadSettings()?.SQLCommandTimeout ?? -1;
        }

        #endregion
    }
}