using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.AbcExportOrder.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Misc.AbcExportOrder.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class AbcExportOrderController : BasePluginController
    {
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        public AbcExportOrderController(
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

        public async Task<IActionResult> Configure()
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<ExportOrderSettings>(storeScope);

            var model = new ConfigModel
            {
                ActiveStoreScopeConfiguration = storeScope,
                OrderIdPrefix = settings.OrderIdPrefix,
                TablePrefix = settings.TablePrefix,
                FailureAlertEmail = settings.FailureAlertEmail
            };

            if (storeScope > 0)
            {
                model.OrderIdPrefix_OverrideForStore =
                    await _settingService.SettingExistsAsync(settings, x => x.OrderIdPrefix, storeScope);
                model.TablePrefix_OverrideForStore =
                    await _settingService.SettingExistsAsync(settings, x => x.TablePrefix, storeScope);
                model.FailureAlertEmail_OverrideForStore =
                    await _settingService.SettingExistsAsync(settings, x => x.FailureAlertEmail, storeScope);
            }

            return View("~/Plugins/Misc.AbcExportOrder/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigModel model)
        {
            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var googleAnalyticsSettings = await _settingService.LoadSettingAsync<ExportOrderSettings>(storeScope);

            var settings = new ExportOrderSettings()
            {
                OrderIdPrefix = model.OrderIdPrefix,
                TablePrefix = model.TablePrefix,
                FailureAlertEmail = model.FailureAlertEmail
            };

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.OrderIdPrefix, model.OrderIdPrefix_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.TablePrefix, model.TablePrefix_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.FailureAlertEmail, model.FailureAlertEmail_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }
    }
}
