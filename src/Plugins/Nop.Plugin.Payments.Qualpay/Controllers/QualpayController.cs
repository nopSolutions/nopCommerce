using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.Payments.Qualpay.Domain;
using Nop.Plugin.Payments.Qualpay.Models;
using Nop.Plugin.Payments.Qualpay.Services;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.Qualpay.Controllers
{
    public class QualpayController : BaseAdminController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly QualpayManager _qualpayManager;

        #endregion

        #region Ctor

        public QualpayController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IStoreContext storeContext,
            ISettingService settingService,
            QualpayManager qualpayManager)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _qualpayManager = qualpayManager;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<QualpaySettings>(storeId);

            //prepare model
            var model = new ConfigurationModel
            {
                MerchantId = settings.MerchantId,
                MerchantEmail = settings.MerchantEmail,
                SecurityKey = settings.SecurityKey,
                ProfileId = settings.ProfileId,
                UseSandbox = settings.UseSandbox,
                UseEmbeddedFields = settings.UseEmbeddedFields,
                UseCustomerVault = settings.UseCustomerVault,
                UseRecurringBilling = settings.UseRecurringBilling,
                PaymentTransactionTypeId = (int)settings.PaymentTransactionType,
                AdditionalFee = settings.AdditionalFee,
                AdditionalFeePercentage = settings.AdditionalFeePercentage,
                ActiveStoreScopeConfiguration = storeId,
                IsConfigured = !string.IsNullOrEmpty(settings.MerchantId) && long.TryParse(settings.MerchantId, out var merchantId)
            };

            if (storeId > 0)
            {
                model.UseSandbox_OverrideForStore = _settingService.SettingExists(settings, x => x.UseSandbox, storeId);
                model.UseEmbeddedFields_OverrideForStore = _settingService.SettingExists(settings, x => x.UseEmbeddedFields, storeId);
                model.UseCustomerVault_OverrideForStore = _settingService.SettingExists(settings, x => x.UseCustomerVault, storeId);
                model.UseRecurringBilling_OverrideForStore = _settingService.SettingExists(settings, x => x.UseRecurringBilling, storeId);
                model.PaymentTransactionTypeId_OverrideForStore = _settingService.SettingExists(settings, x => x.PaymentTransactionType, storeId);
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(settings, x => x.AdditionalFee, storeId);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(settings, x => x.AdditionalFeePercentage, storeId);
            }

            //prepare payment transaction types
            model.PaymentTransactionTypes = TransactionType.Authorization.ToSelectList(false)
                .Select(item => new SelectListItem(item.Text, item.Value)).ToList();

            return View("~/Plugins/Payments.Qualpay/Views/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<QualpaySettings>(storeId);

            //ensure that webhook is already exists and create the new one if does not (required for recurring billing)
            if (model.UseRecurringBilling)
            {
                var webhook = _qualpayManager.CreateWebhook(settings.WebhookId);
                if (webhook?.WebhookId != null)
                {
                    settings.WebhookId = webhook.WebhookId.ToString();
                    settings.WebhookSecretKey = webhook.Secret;
                    _settingService.SaveSetting(settings, x => x.WebhookId, storeId, false);
                    _settingService.SaveSetting(settings, x => x.WebhookSecretKey, storeId, false);
                }
                else
                    _notificationService.WarningNotification(_localizationService.GetResource("Plugins.Payments.Qualpay.Fields.Webhook.Warning"));
            }

            //save settings
            settings.MerchantId = model.MerchantId;
            settings.SecurityKey = model.SecurityKey;
            settings.ProfileId = model.ProfileId;
            settings.UseSandbox = model.UseSandbox;
            settings.UseEmbeddedFields = model.UseEmbeddedFields;
            settings.UseCustomerVault = model.UseCustomerVault;
            settings.UseRecurringBilling = model.UseRecurringBilling;
            settings.PaymentTransactionType = (TransactionType)model.PaymentTransactionTypeId;
            settings.AdditionalFee = model.AdditionalFee;
            settings.AdditionalFeePercentage = model.AdditionalFeePercentage;

            _settingService.SaveSetting(settings, x => x.MerchantId, storeId, false);
            _settingService.SaveSetting(settings, x => x.SecurityKey, storeId, false);
            _settingService.SaveSetting(settings, x => x.ProfileId, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.UseSandbox, model.UseSandbox_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.UseEmbeddedFields, model.UseEmbeddedFields_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.UseCustomerVault, model.UseCustomerVault_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.UseRecurringBilling, model.UseRecurringBilling_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.PaymentTransactionType, model.PaymentTransactionTypeId_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeId, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeId, false);

            _settingService.ClearCache();

            //display notification
            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("subscribe")]
        public IActionResult Subscribe(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings
            var settings = _settingService.LoadSetting<QualpaySettings>();
            if (settings.MerchantEmail == model.MerchantEmail)
                return Configure();

            //try to subscribe/unsubscribe
            var (success, errorMessage) = _qualpayManager.SubscribeForQualpayNews(model.MerchantEmail);
            if (success)
            {
                //save settings and display success notification
                settings.MerchantEmail = model.MerchantEmail;
                _settingService.SaveSetting(settings);

                var message = !string.IsNullOrEmpty(model.MerchantEmail)
                    ? _localizationService.GetResource("Plugins.Payments.Qualpay.Subscribe.Success")
                    : _localizationService.GetResource("Plugins.Payments.Qualpay.Unsubscribe.Success");
                _notificationService.SuccessNotification(message);
            }
            else
            {
                var message = !string.IsNullOrEmpty(errorMessage)
                    ? errorMessage
                    : _localizationService.GetResource("Plugins.Payments.Qualpay.Subscribe.Error");

                _notificationService.ErrorNotification(message);
            }

            return Configure();
        }

        #endregion
    }
}