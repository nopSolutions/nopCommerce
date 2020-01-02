using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.CheckMoneyOrder.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.CheckMoneyOrder.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class PaymentCheckMoneyOrderController : BasePaymentController
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public PaymentCheckMoneyOrderController(ILanguageService languageService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _languageService = languageService;
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var checkMoneyOrderPaymentSettings = _settingService.LoadSetting<CheckMoneyOrderPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                DescriptionText = checkMoneyOrderPaymentSettings.DescriptionText
            };

            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.DescriptionText = _localizationService
                    .GetLocalizedSetting(checkMoneyOrderPaymentSettings, x => x.DescriptionText, languageId, 0, false, false);
            });
            model.AdditionalFee = checkMoneyOrderPaymentSettings.AdditionalFee;
            model.AdditionalFeePercentage = checkMoneyOrderPaymentSettings.AdditionalFeePercentage;
            model.ShippableProductRequired = checkMoneyOrderPaymentSettings.ShippableProductRequired;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.DescriptionText_OverrideForStore = _settingService.SettingExists(checkMoneyOrderPaymentSettings, x => x.DescriptionText, storeScope);
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(checkMoneyOrderPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(checkMoneyOrderPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
                model.ShippableProductRequired_OverrideForStore = _settingService.SettingExists(checkMoneyOrderPaymentSettings, x => x.ShippableProductRequired, storeScope);
            }

            return View("~/Plugins/Payments.CheckMoneyOrder/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var checkMoneyOrderPaymentSettings = _settingService.LoadSetting<CheckMoneyOrderPaymentSettings>(storeScope);

            //save settings
            checkMoneyOrderPaymentSettings.DescriptionText = model.DescriptionText;
            checkMoneyOrderPaymentSettings.AdditionalFee = model.AdditionalFee;
            checkMoneyOrderPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            checkMoneyOrderPaymentSettings.ShippableProductRequired = model.ShippableProductRequired;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(checkMoneyOrderPaymentSettings, x => x.DescriptionText, model.DescriptionText_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(checkMoneyOrderPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(checkMoneyOrderPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(checkMoneyOrderPaymentSettings, x => x.ShippableProductRequired, model.ShippableProductRequired_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            //localization. no multi-store support for localization yet.
            foreach (var localized in model.Locales)
            {
                _localizationService.SaveLocalizedSetting(checkMoneyOrderPaymentSettings,
                    x => x.DescriptionText, localized.LanguageId, localized.DescriptionText);
            }

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}