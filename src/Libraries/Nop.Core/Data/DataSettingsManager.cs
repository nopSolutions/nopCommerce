using Newtonsoft.Json;
using Nop.Core.Infrastructure;
using System;
using System.IO;

namespace Nop.Core.Data
{
    /// <summary>
    /// Manager of data settings (connection string)
    /// </summary>
    public partial class DataSettingsManager
    {
        #region Const
        private const string ObsoleteDataSettingsFilePath = "~/App_Data/Settings.txt";
        private const string DataSettingsFilePath_ = "~/App_Data/dataSettings.json";
        #endregion Const

        #region Properties
        /// <summary>
        /// Gets the path to file that contains data settings
        /// </summary>
        public static string DataSettingsFilePath => DataSettingsFilePath_;
        #endregion Properties

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

            filePath = filePath ?? CommonHelper.MapPath(DataSettingsFilePath);

            //check whether file exists
            if (!File.Exists(filePath))
            {
                //if not, try to parse the file that was used in previous nopCommerce versions
                filePath = CommonHelper.MapPath(ObsoleteDataSettingsFilePath);
                if (!File.Exists(filePath))
                    return new DataSettings();

                //get data settings from the old txt file
                var dataSettings = GetSettingsFromLegacyTextFile(filePath);

                //save data settings to the new file
                SaveSettings(dataSettings);

                //and delete the old one
                File.Delete(filePath);

                var usableDataSettings = dataSettings.GetClone();
                usableDataSettings.Decrypt();
                Singleton<DataSettings>.Instance = usableDataSettings;
                return Singleton<DataSettings>.Instance;
            }

            var text = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(text))
                return new DataSettings();

            //get data settings from the JSON file
            var candidateDataSettings = JsonConvert.DeserializeObject<DataSettings>(text);
            candidateDataSettings.Decrypt();
            Singleton<DataSettings>.Instance = candidateDataSettings;

            //Handles resaving the file if the encryption setting has been changed by the user via editing the file
            ResaveSettingsWhenEncryptionStateMismatch();

            return Singleton<DataSettings>.Instance;
        }

        /// <summary>
        /// Save settings to a file
        /// </summary>
        /// <param name="settings">Data settings</param>
        public virtual void SaveSettings(DataSettings settings)
        {
            Singleton<DataSettings>.Instance = settings ?? throw new ArgumentNullException(nameof(settings));
            var clonedSettings = settings.GetClone();
            if (settings.EncryptStringSettings)
            {
                clonedSettings.Encrypt();
            }
            else
            {
                clonedSettings.Decrypt();
            }
            string text = JsonConvert.SerializeObject(clonedSettings, Formatting.Indented);
            SaveFile(text); //encrypt settings file after saving.
        }

        /// <summary>
        /// Attempts to load settings from old-style settings.txt file
        /// </summary>
        /// <param name="filePath">Path to settings.txt</param>
        /// <returns>Returns a data settings object</returns>
        protected DataSettings GetSettingsFromLegacyTextFile(string filePath)
        {
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
                            dataSettings.DataProvider = value;
                            continue;
                        case "DataConnectionString":
                            dataSettings.DataConnectionString = value;
                            continue;
                        case "EncryptStringSettings":
                            dataSettings.EncryptStringSettings = Convert.ToBoolean(value);
                            continue;
                        default:
                            dataSettings.RawDataSettings.Add(key, value);
                            continue;
                    }
                }
            }
            return dataSettings;
        }

        /// <summary>
        /// Saves the value of the text param to the settings file.
        /// </summary>
        /// <param name="text">value of all text to be saved to the settings file.</param>
        protected void SaveFile(string text)
        {
            var filePath = CommonHelper.MapPath(DataSettingsFilePath);

            //create file if not exists
            if (!File.Exists(filePath))
            {
                //we use 'using' to close the file after it's created
                using (File.Create(filePath)) { }
            }
            File.WriteAllText(filePath, text);
        }

        /// <summary>
        /// Will call SaveSettings() as needed when the EncryptStringSettings method does not match the state of the current datasettings object.  Used when the user manually edits the value of EncryptStringSettings in the settings file.
        /// </summary>
        private void ResaveSettingsWhenEncryptionStateMismatch()
        {
            if (Singleton<DataSettings>.Instance.EncryptStringSettings && Singleton<DataSettings>.Instance.HasAnyDecryptedDataSettings())  //If it's not encrypted and should be, then encrypt it.
            {
                SaveSettings(Singleton<DataSettings>.Instance);
            }
            else if (!Singleton<DataSettings>.Instance.EncryptStringSettings && !Singleton<DataSettings>.Instance.HasAnyEncryptedDataSettings()) //If it's encrypted and it should not be, then decrypt it.
            {
                SaveSettings(Singleton<DataSettings>.Instance);
            }
        }
        #endregion Methods
    }
}