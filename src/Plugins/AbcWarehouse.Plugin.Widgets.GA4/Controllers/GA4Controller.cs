using System.Threading.Tasks;
using AbcWarehouse.Plugin.Widgets.GA4.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Core;

namespace AbcWarehouse.Plugin.Widgets.GA4.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class GA4Controller : BasePluginController
    {
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        public GA4Controller(
            ISettingService settingService,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            INotificationService notificationService)
        {
            _settingService = settingService;
            _storeContext = storeContext;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }

        public async Task<ActionResult> Configure()
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var ga4Settings = await _settingService.LoadSettingAsync<GA4Settings>(storeScope);

            var model = new ConfigModel
            {
                ActiveStoreScopeConfiguration = storeScope,
                GoogleTag = ga4Settings.GoogleTag,
                IsDebugMode = ga4Settings.IsDebugMode,
            };

            if (storeScope > 0)
            {
                model.GoogleTag_OverrideForStore =
                    await _settingService.SettingExistsAsync(ga4Settings, x => x.GoogleTag, storeScope);
                model.IsDebugMode_OverrideForStore =
                    await _settingService.SettingExistsAsync(ga4Settings, x => x.IsDebugMode, storeScope);
            }

            return View("~/Plugins/Widgets.GA4/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<ActionResult> Configure(ConfigModel model)
        {
            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GA4Settings>(storeScope);

            var settings = new GA4Settings()
            {
                GoogleTag = model.GoogleTag,
                IsDebugMode = model.IsDebugMode
            };

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.GoogleTag, model.GoogleTag_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.IsDebugMode, model.IsDebugMode_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }
    }
}
