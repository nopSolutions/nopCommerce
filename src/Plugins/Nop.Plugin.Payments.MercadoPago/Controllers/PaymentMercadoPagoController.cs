using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.MercadoPago.Models;
using Nop.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.MercadoPago.Controllers
{
    [Area(AreaNames.Admin)]
    public class PaymentMercadoPagoController : BasePaymentController
    {
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public PaymentMercadoPagoController(INotificationService notificationService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _storeContext = storeContext ?? throw new ArgumentNullException(nameof(storeContext));
        }

        [AuthorizeAdmin]
        public IActionResult Configure(CancellationToken cancellationToken = default)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var mercadoPagoPaymentSettings = _settingService.LoadSetting<MercadoPagoPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                AccessToken = mercadoPagoPaymentSettings.AccessToken,
                AccessTokenSandbox = mercadoPagoPaymentSettings.AccessTokenSandbox,
                UseSandbox = mercadoPagoPaymentSettings.UseSandbox,
                PassPurchasedItems = mercadoPagoPaymentSettings.PassPurchasedItems,
                PaymentMethodDescription = mercadoPagoPaymentSettings.PaymentMethodDescription,
                PublicKey = mercadoPagoPaymentSettings.PublicKey,
                PublicKeySandbox = mercadoPagoPaymentSettings.PublicKeySandbox,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.AccessToken_OverrideForStore = _settingService.SettingExists(mercadoPagoPaymentSettings, x => x.AccessToken, storeScope);
                model.AccessTokenSandbox_OverrideForStore = _settingService.SettingExists(mercadoPagoPaymentSettings, x => x.AccessTokenSandbox, storeScope);
                model.UseSandbox_OverrideForStore = _settingService.SettingExists(mercadoPagoPaymentSettings, x => x.UseSandbox, storeScope);
                model.PassPurchasedItems_OverrideForStore = _settingService.SettingExists(mercadoPagoPaymentSettings, x => x.PassPurchasedItems, storeScope);
                model.PaymentMethodDescription_OverrideForStore = _settingService.SettingExists(mercadoPagoPaymentSettings, x => x.PaymentMethodDescription, storeScope);
                model.PublicKey_OverrideForStore = _settingService.SettingExists(mercadoPagoPaymentSettings, x => x.PublicKey, storeScope);
                model.PublicKeySandbox_OverrideForStore = _settingService.SettingExists(mercadoPagoPaymentSettings, x => x.PublicKeySandbox, storeScope);
            }

            return View("~/Plugins/Payments.MercadoPago/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var mercadoPagoPaymentSettings = _settingService.LoadSetting<MercadoPagoPaymentSettings>(storeScope);

            //save settings
            mercadoPagoPaymentSettings.AccessToken = model.AccessToken;
            mercadoPagoPaymentSettings.AccessTokenSandbox = model.AccessTokenSandbox;
            mercadoPagoPaymentSettings.UseSandbox = model.UseSandbox;
            mercadoPagoPaymentSettings.PassPurchasedItems = model.PassPurchasedItems;
            mercadoPagoPaymentSettings.PaymentMethodDescription = model.PaymentMethodDescription;
            mercadoPagoPaymentSettings.PublicKey = model.PublicKey;
            mercadoPagoPaymentSettings.PublicKeySandbox = model.PublicKeySandbox;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(mercadoPagoPaymentSettings, x => x.AccessToken, model.AccessToken_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mercadoPagoPaymentSettings, x => x.AccessTokenSandbox, model.AccessTokenSandbox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mercadoPagoPaymentSettings, x => x.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mercadoPagoPaymentSettings, x => x.PassPurchasedItems, model.PassPurchasedItems_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mercadoPagoPaymentSettings, x => x.PaymentMethodDescription, model.PaymentMethodDescription_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mercadoPagoPaymentSettings, x => x.PublicKey, model.PublicKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mercadoPagoPaymentSettings, x => x.PublicKeySandbox, model.PublicKeySandbox_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }
    }
}