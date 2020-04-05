using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.DateTimeFormat.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.DateTimeFormat.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class WidgetsDateTimeFormatController : BasePluginController
    {
        #region Fields
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        #endregion

        #region Ctor
        public WidgetsDateTimeFormatController(
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
        }
        #endregion

        #region Methods        
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var dateTimeFormatSettings = _settingService.LoadSetting<DateTimeFormatSettings>(storeScope);

            var model = new ConfigurationModel
            {
                FormatString = dateTimeFormatSettings.FormatString,
            };

            if (storeScope > 0)
            {
                model.FormatString_OverrideForStore = _settingService.SettingExists(dateTimeFormatSettings, x => x.FormatString, storeScope);
            }

            return View("~/Plugins/Widgets.DateTimeFormat/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var dateTimeFormatSettings = _settingService.LoadSetting<DateTimeFormatSettings>(storeScope);

            dateTimeFormatSettings.FormatString = model.FormatString;

            _settingService.SaveSettingOverridablePerStore(dateTimeFormatSettings, x => x.FormatString, model.FormatString_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return RedirectToAction("Configure");
        }
        #endregion
    }
}