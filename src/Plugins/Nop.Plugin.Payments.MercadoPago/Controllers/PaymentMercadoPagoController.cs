using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
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
    public class PaymentMercadoPagoController : BasePaymentController
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        #endregion


        #region Ctor

        public PaymentMercadoPagoController(IGenericAttributeService genericAttributeService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ILogger logger,
            INotificationService notificationService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext,
            ShoppingCartSettings shoppingCartSettings)
        {
            _genericAttributeService = genericAttributeService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _logger = logger;
            _notificationService = notificationService;
            _settingService = settingService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
            _shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Utilities

        ///// <summary>
        ///// Create webhook that receive events for the subscribed event types
        ///// </summary>
        ///// <returns>Webhook id</returns>
        //protected string CreateWebHook()
        //{
        //    //var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
        //    //var payPalDirectPaymentSettings = _settingService.LoadSetting<PayPalDirectPaymentSettings>(storeScope);

        //    return string.Empty;
        //}

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var mercadoPagoPaymentSettings = _settingService.LoadSetting<MercadoPagoPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                ClientId = mercadoPagoPaymentSettings.ClientId,
                ClientSecret = mercadoPagoPaymentSettings.ClientSecret,
                UseSandbox = mercadoPagoPaymentSettings.UseSandbox,
                WebhookId = mercadoPagoPaymentSettings.WebhookId,
                PassPurchasedItems = mercadoPagoPaymentSettings.PassPurchasedItems,
                TransactModeId = (int)mercadoPagoPaymentSettings.TransactMode,
                AdditionalFee = mercadoPagoPaymentSettings.AdditionalFee,
                AdditionalFeePercentage = mercadoPagoPaymentSettings.AdditionalFeePercentage,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.ClientId_OverrideForStore = _settingService.SettingExists(mercadoPagoPaymentSettings, x => x.ClientId, storeScope);
                model.ClientSecret_OverrideForStore = _settingService.SettingExists(mercadoPagoPaymentSettings, x => x.ClientSecret, storeScope);
                model.UseSandbox_OverrideForStore = _settingService.SettingExists(mercadoPagoPaymentSettings, x => x.UseSandbox, storeScope);
                model.WebhookId_OverrideForStore = _settingService.SettingExists(mercadoPagoPaymentSettings, x => x.WebhookId, storeScope);
                model.PassPurchasedItems_OverrideForStore = _settingService.SettingExists(mercadoPagoPaymentSettings, x => x.PassPurchasedItems, storeScope);
                model.TransactModeId_OverrideForStore = _settingService.SettingExists(mercadoPagoPaymentSettings, x => x.TransactMode, storeScope);
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(mercadoPagoPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(mercadoPagoPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
            }

            return View("~/Plugins/Payments.MercadoPago/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
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
            mercadoPagoPaymentSettings.ClientId = model.ClientId;
            mercadoPagoPaymentSettings.ClientSecret = model.ClientSecret;
            mercadoPagoPaymentSettings.UseSandbox = model.UseSandbox;
            mercadoPagoPaymentSettings.WebhookId = model.WebhookId;
            mercadoPagoPaymentSettings.PassPurchasedItems = model.PassPurchasedItems;
            mercadoPagoPaymentSettings.TransactMode = (TransactMode)model.TransactModeId;
            mercadoPagoPaymentSettings.AdditionalFee = model.AdditionalFee;
            mercadoPagoPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(mercadoPagoPaymentSettings, x => x.ClientId, model.ClientId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mercadoPagoPaymentSettings, x => x.ClientSecret, model.ClientSecret_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mercadoPagoPaymentSettings, x => x.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mercadoPagoPaymentSettings, x => x.WebhookId, model.WebhookId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mercadoPagoPaymentSettings, x => x.PassPurchasedItems, model.PassPurchasedItems_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mercadoPagoPaymentSettings, x => x.TransactMode, model.TransactModeId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mercadoPagoPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mercadoPagoPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        //[HttpPost, ActionName("Configure")]
        //[FormValueRequired("createwebhook")]
        //[AuthorizeAdmin]
        //[AdminAntiForgery]
        //[Area(AreaNames.Admin)]
        //public IActionResult GetWebhookId(ConfigurationModel model)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
        //        return AccessDeniedView();

        //    //var payPalDirectPaymentSettings = _settingService.LoadSetting<PayPalDirectPaymentSettings>();
        //    //payPalDirectPaymentSettings.WebhookId = CreateWebHook();
        //    //_settingService.SaveSetting(payPalDirectPaymentSettings);
        //    //
        //    //if (string.IsNullOrEmpty(payPalDirectPaymentSettings.WebhookId))
        //    //    ErrorNotification(_localizationService.GetResource("Plugins.Payments.PayPalDirect.WebhookError"));

        //    return Configure();
        //}

        //[HttpPost]
        //public IActionResult WebhookEventsHandler()
        //{
        //    var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
        //    //var payPalDirectPaymentSettings = _settingService.LoadSetting<PayPalDirectPaymentSettings>(storeScope);

        //    try
        //    {
        //        var requestBody = string.Empty;
        //        using (var stream = new StreamReader(this.Request.Body, Encoding.UTF8))
        //        {
        //            requestBody = stream.ReadToEnd();
        //        }


        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok();
        //    }
        //}

        #endregion
    }
}