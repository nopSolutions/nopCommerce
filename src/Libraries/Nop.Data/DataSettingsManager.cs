using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Core;
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
        protected static DataSettings LoadDataSettingsFromOldFile(string data)
        {
            var dataSettings = new DataSettings();
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
                        dataSettings.RawDataSettings.Add(key, value);
                        continue;
                }
            }

            return dataSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load data settings
        /// </summary>
        /// <param name="filePath">File path; pass null to use the default settings file</param>
        /// <param name="reloadSettings">Whether to reload data, if they already loaded</param>
        /// <param name="fileProvider">File provider</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the data settings
        /// </returns>
        public static async Task<DataSettings> LoadSettingsAsync(string filePath = null, bool reloadSettings = false, INopFileProvider fileProvider = null)
        {
            if (!reloadSettings && Singleton<DataSettings>.Instance != null)
                return Singleton<DataSettings>.Instance;

            fileProvider ??= CommonHelper.DefaultFileProvider;
            filePath ??= fileProvider.MapPath(NopDataSettingsDefaults.FilePath);

            //check whether file exists
            if (!fileProvider.FileExists(filePath))
            {
                //if not, try to parse the file that was used in previous nopCommerce versions
                filePath = fileProvider.MapPath(NopDataSettingsDefaults.ObsoleteFilePath);
                if (!fileProvider.FileExists(filePath))
                    return new DataSettings();

                //get data settings from the old txt file
                var dataSettings_old =
                    LoadDataSettingsFromOldFile(await fileProvider.ReadAllTextAsync(filePath, Encoding.UTF8));

                //save data settings to the new file
                await SaveSettingsAsync(dataSettings_old, fileProvider);

                //and delete the old one
                fileProvider.DeleteFile(filePath);

                Singleton<DataSettings>.Instance = dataSettings_old;
                return Singleton<DataSettings>.Instance;
            }

            var text = await fileProvider.ReadAllTextAsync(filePath, Encoding.UTF8);
            if (string.IsNullOrEmpty(text))
                return new DataSettings();

            //get data settings from the JSON file
            var dataSettings = JsonConvert.DeserializeObject<DataSettings>(text);

            var dataConnectionString = Environment.GetEnvironmentVariable(NopDataSettingsDefaults.EnvironmentVariableDataConnectionString);
            var dataProvider = Environment.GetEnvironmentVariable(NopDataSettingsDefaults.EnvironmentVariableDataProvider);
            var sqlCommandTimeout = Environment.GetEnvironmentVariable(NopDataSettingsDefaults.EnvironmentVariableSQLCommandTimeout);

            if (!string.IsNullOrEmpty(dataConnectionString))
                dataSettings.ConnectionString = dataConnectionString;

            if (!string.IsNullOrEmpty(dataProvider))
                dataSettings.DataProvider = JsonConvert.DeserializeObject<DataProviderType>(dataProvider);

            if (!string.IsNullOrEmpty(sqlCommandTimeout) && int.TryParse(sqlCommandTimeout, out var sqlTimeOut))
                dataSettings.SQLCommandTimeout = sqlTimeOut;

            Singleton<DataSettings>.Instance = dataSettings;

            return Singleton<DataSettings>.Instance;
        }

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

            fileProvider ??= CommonHelper.DefaultFileProvider;
            filePath ??= fileProvider.MapPath(NopDataSettingsDefaults.FilePath);

            //check whether file exists
            if (!fileProvider.FileExists(filePath))
            {
                //if not, try to parse the file that was used in previous nopCommerce versions
                filePath = fileProvider.MapPath(NopDataSettingsDefaults.ObsoleteFilePath);
                if (!fileProvider.FileExists(filePath))
                    return new DataSettings();

                //get data settings from the old txt file
                var dataSettings_old = LoadDataSettingsFromOldFile(fileProvider.ReadAllText(filePath, Encoding.UTF8));

                //save data settings to the new file
                SaveSettings(dataSettings_old, fileProvider);

                //and delete the old one
                fileProvider.DeleteFile(filePath);

                Singleton<DataSettings>.Instance = dataSettings_old;
                return Singleton<DataSettings>.Instance;
            }

            var text = fileProvider.ReadAllText(filePath, Encoding.UTF8);
            if (string.IsNullOrEmpty(text))
                return new DataSettings();

            //get data settings from the JSON file
            var dataSettings = JsonConvert.DeserializeObject<DataSettings>(text);

            var dataConnectionString = Environment.GetEnvironmentVariable(NopDataSettingsDefaults.EnvironmentVariableDataConnectionString);
            var dataProvider = Environment.GetEnvironmentVariable(NopDataSettingsDefaults.EnvironmentVariableDataProvider);
            var sqlCommandTimeout = Environment.GetEnvironmentVariable(NopDataSettingsDefaults.EnvironmentVariableSQLCommandTimeout);

            if (!string.IsNullOrEmpty(dataConnectionString))
                dataSettings.ConnectionString = dataConnectionString;

            if (!string.IsNullOrEmpty(dataProvider))
                dataSettings.DataProvider = JsonConvert.DeserializeObject<DataProviderType>(dataProvider);

            if (!string.IsNullOrEmpty(sqlCommandTimeout) && int.TryParse(sqlCommandTimeout, out var sqlTimeOut))
                dataSettings.SQLCommandTimeout = sqlTimeOut;

            Singleton<DataSettings>.Instance = dataSettings;

            return Singleton<DataSettings>.Instance;
        }

        /// <summary>
        /// Save data settings to the file
        /// </summary>
        /// <param name="settings">Data settings</param>
        /// <param name="fileProvider">File provider</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task SaveSettingsAsync(DataSettings settings, INopFileProvider fileProvider = null)
        {
            Singleton<DataSettings>.Instance = settings ?? throw new ArgumentNullException(nameof(settings));

            fileProvider ??= CommonHelper.DefaultFileProvider;
            var filePath = fileProvider.MapPath(NopDataSettingsDefaults.FilePath);

            //create file if not exists
            fileProvider.CreateFile(filePath);

            //save data settings to the file
            var text = JsonConvert.SerializeObject(Singleton<DataSettings>.Instance, Formatting.Indented);
            await fileProvider.WriteAllTextAsync(filePath, text, Encoding.UTF8);
        }

        /// <summary>
        /// Save data settings to the file
        /// </summary>
        /// <param name="settings">Data settings</param>
        /// <param name="fileProvider">File provider</param>
        public static void SaveSettings(DataSettings settings, INopFileProvider fileProvider = null)
        {
            Singleton<DataSettings>.Instance = settings ?? throw new ArgumentNullException(nameof(settings));

            fileProvider ??= CommonHelper.DefaultFileProvider;
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
            return (LoadSettings())?.SQLCommandTimeout ?? -1;
        }

        #endregion
    }
}