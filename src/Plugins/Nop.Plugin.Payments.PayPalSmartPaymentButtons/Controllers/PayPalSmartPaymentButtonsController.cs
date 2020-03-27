using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.PayPalSmartPaymentButtons.Domain;
using Nop.Plugin.Payments.PayPalSmartPaymentButtons.Models;
using Nop.Plugin.Payments.PayPalSmartPaymentButtons.Services;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons.Controllers
{
    [Area(AreaNames.Admin)]
    [HttpsRequirement]
    [AutoValidateAntiforgeryToken]
    [ValidateIpAddress]
    [AuthorizeAdmin]
    [ValidateVendor]
    public class PayPalSmartPaymentButtonsController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly ServiceManager _serviceManager;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public PayPalSmartPaymentButtonsController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            ServiceManager serviceManager,
            ShoppingCartSettings shoppingCartSettings)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _serviceManager = serviceManager;
            _shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<PayPalSmartPaymentButtonsSettings>(storeScope);

            //prepare model
            var model = new ConfigurationModel
            {
                ClientId = settings.ClientId,
                SecretKey = settings.SecretKey,
                UseSandbox = settings.UseSandbox,
                PaymentTypeId = (int)settings.PaymentType,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.ClientId_OverrideForStore = _settingService.SettingExists(settings, setting => setting.ClientId, storeScope);
                model.SecretKey_OverrideForStore = _settingService.SettingExists(settings, setting => setting.SecretKey, storeScope);
                model.UseSandbox_OverrideForStore = _settingService.SettingExists(settings, setting => setting.UseSandbox, storeScope);
                model.PaymentTypeId_OverrideForStore = _settingService.SettingExists(settings, setting => setting.PaymentType, storeScope);
                model.ButtonsWidgetZones_OverrideForStore = _settingService.SettingExists(settings, setting => setting.ButtonsWidgetZones, storeScope);
            }

            //prepare available payment types
            model.PaymentTypes = PaymentType.Capture.ToSelectList(false)
                .Select(item => new SelectListItem(item.Text, item.Value))
                .ToList();

            //prepare widget zones
            model.ButtonsWidgetZones = Defaults.AvailableButtonsWidgetZones
                .Select(widgetZone => (settings.ButtonsWidgetZones?.Contains(widgetZone.Value) ?? false) ? widgetZone.Key : 0)
                .Where(id => id > 0).ToList();
            model.AvailableButtonsWidgetZones = Defaults.AvailableButtonsWidgetZones
                .Select(widgetZone => new SelectListItem(widgetZone.Value, widgetZone.Key.ToString(), settings.ButtonsWidgetZones?.Contains(widgetZone.Value) ?? false))
                .ToList();

            //prices and total aren't rounded, so display warning
            if (!_shoppingCartSettings.RoundPricesDuringCalculation)
            {
                var url = Url.Action("AllSettings", "Setting", new { settingName = nameof(ShoppingCartSettings.RoundPricesDuringCalculation) });
                var warning = string.Format(_localizationService.GetResource("Plugins.Payments.PayPalSmartPaymentButtons.RoundingWarning"), url);
                _notificationService.WarningNotification(warning, false);
            }

            return View("~/Plugins/Payments.PayPalSmartPaymentButtons/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return RedirectToAction("Configure");

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<PayPalSmartPaymentButtonsSettings>(storeScope);

            //first delete the unused webhook on a previous client, if changed
            if (!model.ClientId.Equals(settings.ClientId))
                _serviceManager.DeleteWebhook(settings);

            //set new settings values
            settings.ClientId = model.ClientId;
            settings.SecretKey = model.SecretKey;
            settings.UseSandbox = model.UseSandbox;
            settings.PaymentType = (PaymentType)model.PaymentTypeId;
            settings.ButtonsWidgetZones = model.ButtonsWidgetZones.Select(id => Defaults.AvailableButtonsWidgetZones[id]).ToList();

            //ensure that webhook created, display warning in case of fail
            var webhookUrl = Url.RouteUrl(Defaults.WebhookRouteName, null, _webHelper.CurrentRequestProtocol);
            var (webhook, webhookError) = _serviceManager.CreateWebHook(settings, webhookUrl);
            settings.WebhookId = webhook?.Id;
            if (string.IsNullOrEmpty(settings.WebhookId))
            {
                var url = Url.Action("List", "Log");
                var warning = string.Format(_localizationService.GetResource("Plugins.Payments.PayPalSmartPaymentButtons.WebhookWarning"), url);
                _notificationService.WarningNotification(warning, false);
            }

            //save settings
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.WebhookId, true, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.ClientId, model.ClientId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.SecretKey, model.SecretKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.PaymentType, model.PaymentTypeId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.ButtonsWidgetZones, model.ButtonsWidgetZones_OverrideForStore, storeScope, false);
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            //check credentials
            var credentialsError = _serviceManager.CheckCredentials(settings);
            if (!string.IsNullOrEmpty(credentialsError))
                _notificationService.ErrorNotification(credentialsError);
            else
                _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Payments.PayPalSmartPaymentButtons.Credentials.Valid"));

            return RedirectToAction("Configure");
        }

        #endregion
    }
}