using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;

namespace Nop.Plugin.POS.Kaching
{
    public class KachingPlugin : BasePlugin, IMiscPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public KachingPlugin(IWebHelper webHelper, ISettingService settingService, ILocalizationService localizationService)
        {            
            this._webHelper = webHelper;
            this._settingService = settingService;
            this._localizationService = localizationService;
        }


        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        /// 
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/POSKaching/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new POSKachingSettings());

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Nop.Plugin.POS.Kaching.POSKaChingActive"] = "Active",
                ["Nop.Plugin.POS.Kaching.KaChingHost"] = "Kaching host",
                ["Nop.Plugin.POS.Kaching.POSKaChingId"] = "Kaching Id",
                ["Nop.Plugin.POS.Kaching.POSKaChingAccountToken"] = "Kaching Account Token",
                ["Nop.Plugin.POS.Kaching.POSKaChingAPIToken"] = "Kaching API Token",
                ["Nop.Plugin.POS.Kaching.POSKaChingImportQueueName"] = "Kaching ImportQueue name",
                ["Nop.Plugin.POS.Kaching.POSKaChingReconciliationMailAddresses"] = "Kaching reconciliation mail adresses",
            });

            await _localizationService.AddOrUpdateLocaleResourceAsync("Nop.Plugin.POS.Kaching.UpdateStockMessage", "Stock updated through Kaching {0}");

            //// Add danish values
            string dkCode = "da-DK";
            await _localizationService.AddOrUpdateLocaleResourceAsync("Nop.Plugin.POS.Kaching.UpdateStockMessage", "Lagerbeholdning ændret via Kaching {0}", dkCode);

            await base.InstallAsync();
        }
    }
}