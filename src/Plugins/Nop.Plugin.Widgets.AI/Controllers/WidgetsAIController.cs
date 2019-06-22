using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.AI.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.AI.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AdminAntiForgery]
    public class WidgetsAIController : BasePluginController
    {
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;

        public WidgetsAIController(IPermissionService permissionService, IStoreContext storeContext, ISettingService settingService)
        {
            _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
            _storeContext = storeContext ?? throw new ArgumentNullException(nameof(storeContext));
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
        }

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<AISettings>(storeScope);

            var model = new ConfigurationModel
            {
                Enabled = settings.Enabled,
                OverrideInstrumentationKey = settings.OverrideInstrumentationKey,

                DisableAjaxTracking = settings.DisableAjaxTracking,
                DisableExceptionTracking = settings.DisableExceptionTracking,
                DisableFetchTracking =  settings.DisableFetchTracking,
                EnableDebug = settings.EnableDebug,
                MaxAjaxCallsPerView = settings.MaxAjaxCallsPerView,
                OverridePageViewDuration =  settings.OverridePageViewDuration,

                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.Enabled_OverrideForStore = _settingService.SettingExists(settings, x => x.Enabled, storeScope);
                model.OverrideInstrumentationKey_OverrideForStore = _settingService.SettingExists(settings, x => x.OverrideInstrumentationKey, storeScope);

                model.DisableAjaxTracking_OverrideForStore = _settingService.SettingExists(settings, x => x.DisableAjaxTracking, storeScope);
                model.DisableExceptionTracking_OverrideForStore = _settingService.SettingExists(settings, x => x.DisableExceptionTracking, storeScope);
                model.DisableFetchTracking_OverrideForStore = _settingService.SettingExists(settings, x => x.DisableFetchTracking, storeScope);
                model.EnableDebug_OverrideForStore = _settingService.SettingExists(settings, x => x.EnableDebug, storeScope);
                model.MaxAjaxCallsPerView_OverrideForStore = _settingService.SettingExists(settings, x => x.MaxAjaxCallsPerView, storeScope);
                model.OverridePageViewDuration_OverrideForStore = _settingService.SettingExists(settings, x => x.OverridePageViewDuration, storeScope);
            }

            return View("~/Plugins/Widgets.AI/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<AISettings>(storeScope);

            settings.Enabled = model.Enabled;
            settings.OverrideInstrumentationKey = model.OverrideInstrumentationKey;

            settings.DisableAjaxTracking = model.DisableAjaxTracking;
            settings.DisableExceptionTracking = model.DisableExceptionTracking;
            settings.DisableFetchTracking = model.DisableFetchTracking;
            settings.EnableDebug = model.EnableDebug;
            settings.MaxAjaxCallsPerView = settings.MaxAjaxCallsPerView;
            settings.OverridePageViewDuration = model.OverridePageViewDuration;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(settings, x => x.Enabled, model.Enabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.OverrideInstrumentationKey, model.OverrideInstrumentationKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.DisableAjaxTracking, model.DisableAjaxTracking_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.DisableExceptionTracking, model.DisableExceptionTracking_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.DisableFetchTracking, model.DisableFetchTracking_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.EnableDebug, model.EnableDebug_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.MaxAjaxCallsPerView, model.MaxAjaxCallsPerView_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.OverridePageViewDuration, model.OverridePageViewDuration_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var notificationService = EngineContext.Current.Resolve<INotificationService>();
            notificationService.SuccessNotification(localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }
    }
}
