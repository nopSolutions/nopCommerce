namespace Nop.Plugin.Api.Controllers.Admin
{
    using Constants;
    using Core;
    using Domain;
    using MappingExtensions;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Nop.Services.Configuration;
    using Nop.Services.Localization;
    using Nop.Services.Logging;
    using Nop.Services.Messages;
    using Nop.Web.Framework.Controllers;
    using Web.Framework;
    using Web.Framework.Mvc.Filters;

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class ApiAdminController : BasePluginController
    {
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        public ApiAdminController(
            IStoreContext storeContext,
            ISettingService settingService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            INotificationService notificationService)
        {
            _storeContext = storeContext;
            _settingService = settingService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }
        
        [HttpGet]
        public ActionResult Settings()
        {

            var storeScope = _storeContext.ActiveStoreScopeConfiguration;

            var apiSettings = _settingService.LoadSetting<ApiSettings>(storeScope);

            var model = apiSettings.ToModel();

            // Store Settings
            model.ActiveStoreScopeConfiguration = storeScope;

            if (model.EnableApi_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(apiSettings, x => x.EnableApi, storeScope, false);
            if (model.AllowRequestsFromSwagger_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(apiSettings, x => x.AllowRequestsFromSwagger, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            return View(ViewNames.AdminApiSettings, model);
        }

        [HttpPost]
        public ActionResult Settings(ConfigurationModel configurationModel)
        {
            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;

            var settings = configurationModel.ToEntity();

            /* We do not clear cache after each setting update.
            * This behavior can increase performance because cached settings will not be cleared 
            * and loaded from database after each update */

            if (configurationModel.EnableApi_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.EnableApi, storeScope, false);
            if (configurationModel.AllowRequestsFromSwagger_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.AllowRequestsFromSwagger, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            _customerActivityService.InsertActivity("EditApiSettings", "Edit Api Settings");

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return View(ViewNames.AdminApiSettings, configurationModel);
        }
    }
}