using AbcWarehouse.Plugin.Widgets.Flixmedia.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;
using Nop.Core;

namespace AbcWarehouse.Plugin.Widgets.Flixmedia.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class FlixmediaController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;

        public FlixmediaController(
            ISettingService settingService,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            INotificationService notificationService
        )
        {
            _settingService = settingService;
            _storeContext = storeContext;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }

        public async Task<ActionResult> Configure()
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<FlixmediaSettings>(storeScope);

            var model = new FlixmediaConfigModel
            {
                ActiveStoreScopeConfiguration = storeScope,
                FlixID = settings.FlixID,
                WidgetZone = settings.WidgetZone,
            };

            if (storeScope > 0)
            {
                model.FlixID_OverrideForStore =
                    await _settingService.SettingExistsAsync(settings, x => x.FlixID, storeScope);
                model.WidgetZone_OverrideForStore =
                    await _settingService.SettingExistsAsync(settings, x => x.WidgetZone, storeScope);
            }

            return View("~/Plugins/Widgets.Flixmedia/Views/Configure.cshtml", model);
        }

        [HttpPost]
        
        public async Task<IActionResult> Configure(FlixmediaConfigModel model)
        {
            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = new FlixmediaSettings()
            {
                FlixID = model.FlixID,
                WidgetZone = model.WidgetZone
            };

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.FlixID, model.FlixID_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.WidgetZone, model.WidgetZone_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }
    }
}
