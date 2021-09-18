using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

namespace Nop.Services.Configuration
{
    /// <summary>
    /// Represents the app settings helper
    /// </summary>
    public partial class AppSettingsHelper
    {
        #region Methods

        /// <summary>
        /// Create app settings with the passed configurations and save it to the file
        /// </summary>
        /// <param name="configurations">Configurations to save</param>
        /// <param name="fileProvider">File provider</param>
        /// <returns>App settings</returns>
        public static AppSettings SaveAppSettings(IList<IConfig> configurations, INopFileProvider fileProvider = null)
        {
            if (configurations is null)
                throw new ArgumentNullException(nameof(configurations));

            //create app settings
            var appSettings = new AppSettings(configurations);
            Singleton<AppSettings>.Instance = appSettings;

            //create file if not exists
            fileProvider ??= CommonHelper.DefaultFileProvider;
            var filePath = fileProvider.MapPath(NopConfigurationDefaults.AppSettingsFilePath);
            fileProvider.CreateFile(filePath);

            //get raw configuration parameters
            var configuration = JsonConvert.DeserializeObject<AppSettings>(fileProvider.ReadAllText(filePath, Encoding.UTF8))
                ?.Configuration
                ?? new();
            foreach (var config in configurations)
            {
                configuration[config.Name] = JToken.FromObject(config);
            }
            appSettings.Configuration = configuration;

            //save app settings to the file
            var text = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
            fileProvider.WriteAllText(filePath, text, Encoding.UTF8);

            return appSettings;
        }

        #endregion
    }
}