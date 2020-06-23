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
                DisplayButtonsOnShoppingCart = settings.DisplayButtonsOnShoppingCart,
                DisplayButtonsOnProductDetails = settings.DisplayButtonsOnProductDetails,
                DisplayLogoInHeaderLinks = settings.DisplayLogoInHeaderLinks,
                LogoInHeaderLinks = settings.LogoInHeaderLinks,
                DisplayLogoInFooter = settings.DisplayLogoInFooter,
                LogoInFooter = settings.LogoInFooter,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.ClientId_OverrideForStore = _settingService.SettingExists(settings, setting => setting.ClientId, storeScope);
                model.SecretKey_OverrideForStore = _settingService.SettingExists(settings, setting => setting.SecretKey, storeScope);
                model.UseSandbox_OverrideForStore = _settingService.SettingExists(settings, setting => setting.UseSandbox, storeScope);
                model.PaymentTypeId_OverrideForStore = _settingService.SettingExists(settings, setting => setting.PaymentType, storeScope);
                model.DisplayButtonsOnShoppingCart_OverrideForStore = _settingService.SettingExists(settings, setting => setting.DisplayButtonsOnShoppingCart, storeScope);
                model.DisplayButtonsOnProductDetails_OverrideForStore = _settingService.SettingExists(settings, setting => setting.DisplayButtonsOnProductDetails, storeScope);
                model.DisplayLogoInHeaderLinks_OverrideForStore = _settingService.SettingExists(settings, setting => setting.DisplayLogoInHeaderLinks, storeScope);
                model.LogoInHeaderLinks_OverrideForStore = _settingService.SettingExists(settings, setting => setting.LogoInHeaderLinks, storeScope);
                model.DisplayLogoInFooter_OverrideForStore = _settingService.SettingExists(settings, setting => setting.DisplayLogoInFooter, storeScope);
                model.LogoInFooter_OverrideForStore = _settingService.SettingExists(settings, setting => setting.LogoInFooter, storeScope);
            }

            //prepare available payment types
            model.PaymentTypes = PaymentType.Capture.ToSelectList(false)
                .Select(item => new SelectListItem(item.Text, item.Value))
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
                return Configure();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<PayPalSmartPaymentButtonsSettings>(storeScope);

            //first delete the unused webhook on a previous client, if changed
            if ((!model.ClientId?.Equals(settings.ClientId) ?? true) && !string.IsNullOrEmpty(settings.ClientId) && !string.IsNullOrEmpty(settings.SecretKey))
                _serviceManager.DeleteWebhook(settings);

            //set new settings values
            settings.ClientId = model.ClientId;
            settings.SecretKey = model.SecretKey;
            settings.UseSandbox = model.UseSandbox;
            settings.PaymentType = (PaymentType)model.PaymentTypeId;
            settings.DisplayButtonsOnShoppingCart = model.DisplayButtonsOnShoppingCart;
            settings.DisplayButtonsOnProductDetails = model.DisplayButtonsOnProductDetails;
            settings.DisplayLogoInHeaderLinks = model.DisplayLogoInHeaderLinks;
            settings.LogoInHeaderLinks = model.LogoInHeaderLinks;
            settings.DisplayLogoInFooter = model.DisplayLogoInFooter;
            settings.LogoInFooter = model.LogoInFooter;

            //ensure that webhook created, display warning in case of fail
            if (!string.IsNullOrEmpty(settings.ClientId) && !string.IsNullOrEmpty(settings.SecretKey))
            {
                var webhookUrl = Url.RouteUrl(Defaults.WebhookRouteName, null, _webHelper.CurrentRequestProtocol);
                var (webhook, webhookError) = _serviceManager.CreateWebHook(settings, webhookUrl);
                settings.WebhookId = webhook?.Id;
                if (string.IsNullOrEmpty(settings.WebhookId))
                {
                    var url = Url.Action("List", "Log");
                    var warning = string.Format(_localizationService.GetResource("Plugins.Payments.PayPalSmartPaymentButtons.WebhookWarning"), url);
                    _notificationService.WarningNotification(warning, false);
                }
            }

            //save settings
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.WebhookId, true, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.ClientId, model.ClientId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.SecretKey, model.SecretKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.PaymentType, model.PaymentTypeId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.DisplayButtonsOnShoppingCart, model.DisplayButtonsOnShoppingCart_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.DisplayButtonsOnProductDetails, model.DisplayButtonsOnProductDetails_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.DisplayLogoInHeaderLinks, model.DisplayLogoInHeaderLinks_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.LogoInHeaderLinks, model.LogoInHeaderLinks_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.DisplayLogoInFooter, model.DisplayLogoInFooter_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, setting => setting.LogoInFooter, model.LogoInFooter_OverrideForStore, storeScope, false);
            _settingService.ClearCache();

            //ensure credentials are valid
            if (!string.IsNullOrEmpty(settings.ClientId) && !string.IsNullOrEmpty(settings.SecretKey))
            {
                var (_, errorMessage) = _serviceManager.GetAccessToken(settings);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    var url = Url.Action("List", "Log");
                    var error = string.Format(_localizationService.GetResource("Plugins.Payments.PayPalSmartPaymentButtons.Credentials.Invalid"), url);
                    _notificationService.ErrorNotification(error, false);
                }
                else
                    _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Payments.PayPalSmartPaymentButtons.Credentials.Valid"));
            }

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}