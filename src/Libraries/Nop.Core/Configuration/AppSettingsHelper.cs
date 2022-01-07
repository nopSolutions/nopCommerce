using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core.Infrastructure;

namespace Nop.Core.Configuration
{
    /// <summary>
    /// Represents the app settings helper
    /// </summary>
    public partial class AppSettingsHelper
    {
        #region Fields

        private static Dictionary<string, int> _configurationOrder;

        #endregion

        #region Methods

        /// <summary>
        /// Create app settings with the passed configurations and save it to the file
        /// </summary>
        /// <param name="configurations">Configurations to save</param>
        /// <param name="fileProvider">File provider</param>
        /// <param name="overwrite">Whether to overwrite appsettings file</param>
        /// <returns>App settings</returns>
        public static AppSettings SaveAppSettings(IList<IConfig> configurations, INopFileProvider fileProvider, bool overwrite = true)
        {
            if (configurations is null)
                throw new ArgumentNullException(nameof(configurations));

            if (_configurationOrder is null)
                _configurationOrder = configurations.ToDictionary(config => config.Name, config => config.GetOrder());

            //create app settings
            var appSettings = Singleton<AppSettings>.Instance ?? new AppSettings();
            appSettings.Update(configurations);
            Singleton<AppSettings>.Instance = appSettings;

            //create file if not exists
            var filePath = fileProvider.MapPath(NopConfigurationDefaults.AppSettingsFilePath);
            var fileExists = fileProvider.FileExists(filePath);
            fileProvider.CreateFile(filePath);

            //get raw configuration parameters
            var configuration = JsonConvert.DeserializeObject<AppSettings>(fileProvider.ReadAllText(filePath, Encoding.UTF8))
                ?.Configuration
                ?? new();
            foreach (var config in configurations)
            {
                configuration[config.Name] = JToken.FromObject(config);
            }

            //sort configurations for display by order (e.g. data configuration with 0 will be the first)
            appSettings.Configuration = configuration
                .SelectMany(outConfig => _configurationOrder.Where(inConfig => inConfig.Key == outConfig.Key).DefaultIfEmpty(),
                    (outConfig, inConfig) => new { OutConfig = outConfig, InConfig = inConfig })
                .OrderBy(config => config.InConfig.Value)
                .Select(config => config.OutConfig)
                .ToDictionary(config => config.Key, config => config.Value);

            //save app settings to the file
            if (!fileExists || overwrite)
            {
                var text = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
                fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
            }

            return appSettings;
        }

        #endregion
    }
}