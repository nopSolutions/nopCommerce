using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using AbcWarehouse.Plugin.Widgets.UniFi.Models;
using Nop.Core;

namespace AbcWarehouse.Plugin.Widgets.UniFi.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class UniFiController : BasePluginController
    {
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        public UniFiController(
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
            var settings = await _settingService.LoadSettingAsync<UniFiSettings>(storeScope);

            var model = new ConfigModel
            {
                ActiveStoreScopeConfiguration = storeScope,
                IsEnabled = settings.IsEnabled,
                PartnerId = settings.PartnerId,
                UseIntegration = settings.UseIntegration
            };

            if (storeScope > 0)
            {
                model.IsEnabled_OverrideForStore =
                    await _settingService.SettingExistsAsync(settings, x => x.IsEnabled, storeScope);
                model.PartnerId_OverrideForStore =
                    await _settingService.SettingExistsAsync(settings, x => x.PartnerId, storeScope);
                model.UseIntegration_OverrideForStore =
                    await _settingService.SettingExistsAsync(settings, x => x.UseIntegration, storeScope);
            }

            return View("~/Plugins/Widgets.UniFi/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<ActionResult> Configure(ConfigModel model)
        {
            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = new UniFiSettings()
            {
                IsEnabled = model.IsEnabled,
                PartnerId = model.PartnerId,
                UseIntegration = model.UseIntegration
            };

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.IsEnabled, model.IsEnabled_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.PartnerId, model.PartnerId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.UseIntegration, model.UseIntegration_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }
    }
}
