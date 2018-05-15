using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Nop.Core.Infrastructure;

namespace Nop.Core.Data
{
    /// <summary>
    /// Manager of data settings (connection string)
    /// </summary>
    public partial class DataSettingsManager
    {
        #region Const

        private const string OBSOLETE_DATA_SETTINGS_FILE_PATH = "~/App_Data/Settings.txt";
        private const string DATA_SETTINGS_FILE_PATH_ = "~/App_Data/dataSettings.json";

        #endregion

        #region Fields

        protected INopFileProvider _fileProvider;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the path to file that contains data settings
        /// </summary>
        public static string DataSettingsFilePath => DATA_SETTINGS_FILE_PATH_;

        #endregion

        #region Ctor

        public DataSettingsManager(INopFileProvider fileProvider = null)
        {
            this._fileProvider = fileProvider ?? CommonHelper.DefaultFileProvider;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load settings
        /// </summary>
        /// <param name="filePath">File path; pass null to use default settings file path</param>
        /// <param name="reloadSettings">Indicates whether to reload data, if they already loaded</param>
        /// <returns>Data settings</returns>
        public virtual DataSettings LoadSettings(string filePath = null, bool reloadSettings = false)
        {
            if (!reloadSettings && Singleton<DataSettings>.Instance != null)
                return Singleton<DataSettings>.Instance;

            filePath = filePath ?? _fileProvider.MapPath(DataSettingsFilePath);

            //check whether file exists
            if (!_fileProvider.FileExists(filePath))
            {
                //if not, try to parse the file that was used in previous nopCommerce versions
                filePath = _fileProvider.MapPath(OBSOLETE_DATA_SETTINGS_FILE_PATH);
                if (!_fileProvider.FileExists(filePath))
                    return new DataSettings();

                //get data settings from the old txt file
                var dataSettings = new DataSettings();
                using (var reader = new StringReader(_fileProvider.ReadAllText(filePath, Encoding.UTF8)))
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
                                dataSettings.DataProvider = value;
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
                _fileProvider.DeleteFile(filePath);

                Singleton<DataSettings>.Instance = dataSettings;
                return Singleton<DataSettings>.Instance;
            }

            var text = _fileProvider.ReadAllText(filePath, Encoding.UTF8);
            if (string.IsNullOrEmpty(text))
                return new DataSettings();

            //get data settings from the JSON file
            Singleton<DataSettings>.Instance = JsonConvert.DeserializeObject<DataSettings>(text);
            return Singleton<DataSettings>.Instance;
        }

        /// <summary>
        /// Save settings to a file
        /// </summary>
        /// <param name="settings">Data settings</param>
        public virtual void SaveSettings(DataSettings settings)
        {
            Singleton<DataSettings>.Instance = settings ?? throw new ArgumentNullException(nameof(settings));
            
            var filePath = _fileProvider.MapPath(DataSettingsFilePath);

            //create file if not exists
            _fileProvider.CreateFile(filePath);

            //save data settings to the file
            var text = JsonConvert.SerializeObject(Singleton<DataSettings>.Instance, Formatting.Indented);
            _fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
        }

        #endregion
    }
}
