using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Data.Configuration;

namespace Nop.Data;

/// <summary>
/// Represents the data settings manager
/// </summary>
public partial class DataSettingsManager
{
    #region Fields

    /// <summary>
    /// Gets a cached value indicating whether the database is installed. We need this value invariable during installation process
    /// </summary>
    protected static bool? _databaseIsInstalled;

    #endregion

    #region Utilities

    /// <summary>
    /// Gets data settings from the old txt file (Settings.txt)
    /// </summary>
    /// <param name="data">Old txt file data</param>
    /// <returns>Data settings</returns>
    protected static DataConfig LoadDataSettingsFromOldTxtFile(string data)
    {
        if (string.IsNullOrEmpty(data))
            return null;

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
                    //If parsing isn't successful, we set a negative timeout, that means the current provider will use a default value
                    dataSettings.SQLCommandTimeout = int.TryParse(value, out var timeout) ? timeout : -1;
                    continue;
                default:
                    break;
            }
        }

        return dataSettings;
    }

    /// <summary>
    /// Gets data settings from the old json file (dataSettings.json)
    /// </summary>
    /// <param name="data">Old json file data</param>
    /// <returns>Data settings</returns>
    protected static DataConfig LoadDataSettingsFromOldJsonFile(string data)
    {
        if (string.IsNullOrEmpty(data))
            return null;

        var jsonDataSettings = JsonConvert.DeserializeAnonymousType(data,
            new { DataConnectionString = "", DataProvider = DataProviderType.SqlServer, SQLCommandTimeout = "" });
        var dataSettings = new DataConfig
        {
            ConnectionString = jsonDataSettings.DataConnectionString,
            DataProvider = jsonDataSettings.DataProvider,
            SQLCommandTimeout = int.TryParse(jsonDataSettings.SQLCommandTimeout, out var result) ? result : null
        };

        return dataSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Load data settings
    /// </summary>
    /// <param name="fileProvider">File provider</param>
    /// <param name="reload">Force loading settings from disk</param>
    /// <returns>Data settings</returns>
    public static DataConfig LoadSettings(INopFileProvider fileProvider = null, bool reload = false)
    {
        if (!reload && Singleton<DataConfig>.Instance is not null)
            return Singleton<DataConfig>.Instance;

        //backward compatibility
        fileProvider ??= CommonHelper.DefaultFileProvider;
        var filePath_json = fileProvider.MapPath(NopDataSettingsDefaults.FilePath);
        var filePath_txt = fileProvider.MapPath(NopDataSettingsDefaults.ObsoleteFilePath);
        if (fileProvider.FileExists(filePath_json) || fileProvider.FileExists(filePath_txt))
        {
            var dataSettings = fileProvider.FileExists(filePath_json)
                ? LoadDataSettingsFromOldJsonFile(fileProvider.ReadAllText(filePath_json, Encoding.UTF8))
                : LoadDataSettingsFromOldTxtFile(fileProvider.ReadAllText(filePath_txt, Encoding.UTF8))
                  ?? new DataConfig();

            fileProvider.DeleteFile(filePath_json);
            fileProvider.DeleteFile(filePath_txt);

            AppSettingsHelper.SaveAppSettings(new List<IConfig> { dataSettings }, fileProvider);
            Singleton<DataConfig>.Instance = dataSettings;
        }
        else
        {
            Singleton<DataConfig>.Instance = Singleton<AppSettings>.Instance.Get<DataConfig>();
        }

        return Singleton<DataConfig>.Instance;
    }

    /// <summary>
    /// Save data settings
    /// </summary>
    /// <param name="dataSettings">Data settings</param>
    /// <param name="fileProvider">File provider</param>
    public static void SaveSettings(DataConfig dataSettings, INopFileProvider fileProvider)
    {
        AppSettingsHelper.SaveAppSettings(new List<IConfig> { dataSettings }, fileProvider);
        LoadSettings(fileProvider, reload: true);
    }

    /// <summary>
    /// Gets a value indicating whether database is already installed
    /// </summary>
    public static bool IsDatabaseInstalled()
    {
        _databaseIsInstalled ??= !string.IsNullOrEmpty(LoadSettings()?.ConnectionString);

        return _databaseIsInstalled.Value;
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

    /// <summary>
    /// Gets a value that indicates whether to add NoLock hint to SELECT statements (applies only to SQL Server, otherwise returns false)
    /// </summary>
    public static bool UseNoLock()
    {
        var settings = LoadSettings();

        if (settings is null)
            return false;

        return settings.DataProvider == DataProviderType.SqlServer && settings.WithNoLock;
    }

    #endregion
}