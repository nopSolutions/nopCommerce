using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        /// Save app settings to the file
        /// </summary>
        /// <param name="appSettings">App settings</param>
        /// <param name="fileProvider">File provider</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task SaveAppSettingsAsync(AppSettings appSettings, INopFileProvider fileProvider = null)
        {
            Singleton<AppSettings>.Instance = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

            fileProvider ??= CommonHelper.DefaultFileProvider;

            //create file if not exists
            var filePath = fileProvider.MapPath(NopConfigurationDefaults.AppSettingsFilePath);
            fileProvider.CreateFile(filePath);

            //check additional configuration parameters
            var additionalData = JsonConvert.DeserializeObject<AppSettings>(await fileProvider.ReadAllTextAsync(filePath, Encoding.UTF8))?.AdditionalData;
            appSettings.AdditionalData = additionalData;

            //save app settings to the file
            var text = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
            await fileProvider.WriteAllTextAsync(filePath, text, Encoding.UTF8);
        }

        /// <summary>
        /// Save app settings to the file
        /// </summary>
        /// <param name="appSettings">App settings</param>
        /// <param name="fileProvider">File provider</param>
        public static void SaveAppSettings(AppSettings appSettings, INopFileProvider fileProvider = null)
        {
            Singleton<AppSettings>.Instance = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

            fileProvider ??= CommonHelper.DefaultFileProvider;

            //create file if not exists
            var filePath = fileProvider.MapPath(NopConfigurationDefaults.AppSettingsFilePath);
            fileProvider.CreateFile(filePath);

            //check additional configuration parameters
            var additionalData = JsonConvert.DeserializeObject<AppSettings>(fileProvider.ReadAllText(filePath, Encoding.UTF8))?.AdditionalData;
            appSettings.AdditionalData = additionalData;

            //save app settings to the file
            var text = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
            fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
        }

        #endregion
    }
}