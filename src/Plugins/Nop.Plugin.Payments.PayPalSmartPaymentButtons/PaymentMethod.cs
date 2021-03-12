using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.PayPalSmartPaymentButtons.Domain;
using Nop.Plugin.Payments.PayPalSmartPaymentButtons.Models;
using Nop.Plugin.Payments.PayPalSmartPaymentButtons.Services;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Stores;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons
{
    /// <summary>
    /// Represents a payment method implementation
    /// </summary>
    public class PaymentMethod : BasePlugin, IPaymentMethod, IWidgetPlugin
    {
        #region Fields

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly CurrencySettings _currencySettings;
        private readonly PayPalSmartPaymentButtonsSettings _settings;
        private readonly ServiceManager _serviceManager;
        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public PaymentMethod(IActionContextAccessor actionContextAccessor,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ISettingService settingService,
            IStoreService storeService,
            IUrlHelperFactory urlHelperFactory,
            CurrencySettings currencySettings,
            PayPalSmartPaymentButtonsSettings settings,
            ServiceManager serviceManager,
            WidgetSettings widgetSettings)
        {
            _actionContextAccessor = actionContextAccessor;
            _currencyService = currencyService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _settingService = settingService;
            _storeService = storeService;
            _urlHelperFactory = urlHelperFactory;
            _currencySettings = currencySettings;
            _settings = settings;
            _serviceManager = serviceManager;
            _widgetSettings = widgetSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the process payment result
        /// </returns>
        public async Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            //try to get an order id from custom values
            var orderIdKey = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalSmartPaymentButtons.OrderId");
            if (!processPaymentRequest.CustomValues.TryGetValue(orderIdKey, out var orderId) || string.IsNullOrEmpty(orderId?.ToString()))
                throw new NopException("Failed to get the PayPal order ID");

            //authorize or capture the order
            var (order, error) = _settings.PaymentType == PaymentType.Capture
                ? await _serviceManager.CaptureAsync(_settings, orderId.ToString())
                : (_settings.PaymentType == PaymentType.Authorize
                ? await _serviceManager.AuthorizeAsync(_settings, orderId.ToString())
                : (default, default));

            if (!string.IsNullOrEmpty(error))
                return new ProcessPaymentResult { Errors = new[] { error } };

            //request succeeded
            var result = new ProcessPaymentResult();

            var purchaseUnit = order.PurchaseUnits.FirstOrDefault(item => item.ReferenceId.Equals(processPaymentRequest.OrderGuid.ToString()));
            var authorization = purchaseUnit.Payments?.Authorizations?.FirstOrDefault();
            if (authorization != null)
            {
                result.AuthorizationTransactionId = authorization.Id;
                result.AuthorizationTransactionResult = authorization.Status;
                result.NewPaymentStatus = PaymentStatus.Authorized;
            }
            var capture = purchaseUnit.Payments?.Captures?.FirstOrDefault();
            if (capture != null)
            {
                result.CaptureTransactionId = capture.Id;
                result.CaptureTransactionResult = capture.Status;
                result.NewPaymentStatus = PaymentStatus.Paid;
            }

            return result;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the capture payment result
        /// </returns>
        public async Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            //capture previously authorized payment
            var (capture, error) = await _serviceManager
                .CaptureAuthorizationAsync(_settings, capturePaymentRequest.Order.AuthorizationTransactionId);

            if (!string.IsNullOrEmpty(error))
                return new CapturePaymentResult { Errors = new[] { error } };

            //request succeeded
            return new CapturePaymentResult
            {
                CaptureTransactionId = capture.Id,
                CaptureTransactionResult = capture.Status,
                NewPaymentStatus = PaymentStatus.Paid
            };
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public async Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            //void previously authorized payment
            var (_, error) = await _serviceManager.VoidAsync(_settings, voidPaymentRequest.Order.AuthorizationTransactionId);

            if (!string.IsNullOrEmpty(error))
                return new VoidPaymentResult { Errors = new[] { error } };

            //request succeeded
            return new VoidPaymentResult { NewPaymentStatus = PaymentStatus.Voided };
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public async Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            //refund previously captured payment
            var amount = refundPaymentRequest.AmountToRefund != refundPaymentRequest.Order.OrderTotal
                ? (decimal?)refundPaymentRequest.AmountToRefund
                : null;

            //get the primary store currency
            var currency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
            if (currency == null)
                throw new NopException("Primary store currency cannot be loaded");

            var (refund, error) = await _serviceManager.RefundAsync(
                _settings, refundPaymentRequest.Order.CaptureTransactionId, currency.CurrencyCode, amount);

            if (!string.IsNullOrEmpty(error))
                return new RefundPaymentResult { Errors = new[] { error } };

            //request succeeded
            var refundIds = await _genericAttributeService.GetAttributeAsync<List<string>>(refundPaymentRequest.Order, Defaults.RefundIdAttributeName)
                ?? new List<string>();
            if (!refundIds.Contains(refund.Id))
                refundIds.Add(refund.Id);
            await _genericAttributeService.SaveAttributeAsync(refundPaymentRequest.Order, Defaults.RefundIdAttributeName, refundIds);
            return new RefundPaymentResult
            {
                NewPaymentStatus = refundPaymentRequest.IsPartialRefund ? PaymentStatus.PartiallyRefunded : PaymentStatus.Refunded
            };
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the process payment result
        /// </returns>
        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return Task.FromResult(new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - hide; false - display.
        /// </returns>
        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the additional handling fee
        /// </returns>
        public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(decimal.Zero);
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of validating errors
        /// </returns>
        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var errors = new List<string>();

            //try to get errors from the form parameters
            if (form.TryGetValue(nameof(PaymentInfoModel.Errors), out var errorValue) && !StringValues.IsNullOrEmpty(errorValue))
                errors.Add(errorValue.ToString());

            return Task.FromResult<IList<string>>(errors);
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the payment info holder
        /// </returns>
        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            //already set
            return Task.FromResult(_actionContextAccessor.ActionContext.HttpContext.Session.Get<ProcessPaymentRequest>(Defaults.PaymentRequestSessionKey));
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(Defaults.ConfigurationRouteName);
        }

        /// <summary>
        /// Gets a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        public string GetPublicViewComponentName()
        {
            return Defaults.PAYMENT_INFO_VIEW_COMPONENT_NAME;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the widget zones
        /// </returns>
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
            {
                PublicWidgetZones.CheckoutPaymentInfoTop,
                PublicWidgetZones.OpcContentBefore,
                PublicWidgetZones.ProductDetailsTop,
                PublicWidgetZones.ProductDetailsAddInfo,
                PublicWidgetZones.OrderSummaryContentBefore,
                PublicWidgetZones.OrderSummaryContentAfter,
                PublicWidgetZones.HeaderLinksBefore,
                PublicWidgetZones.Footer
            });
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            if (widgetZone == null)
                throw new ArgumentNullException(nameof(widgetZone));

            if (widgetZone.Equals(PublicWidgetZones.CheckoutPaymentInfoTop) ||
                widgetZone.Equals(PublicWidgetZones.OpcContentBefore) ||
                widgetZone.Equals(PublicWidgetZones.ProductDetailsTop) ||
                widgetZone.Equals(PublicWidgetZones.OrderSummaryContentBefore))
            {
                return Defaults.SCRIPT_VIEW_COMPONENT_NAME;
            }

            if (widgetZone.Equals(PublicWidgetZones.ProductDetailsAddInfo) || widgetZone.Equals(PublicWidgetZones.OrderSummaryContentAfter))
                return Defaults.BUTTONS_VIEW_COMPONENT_NAME;

            if (widgetZone.Equals(PublicWidgetZones.HeaderLinksBefore) || widgetZone.Equals(PublicWidgetZones.Footer))
                return Defaults.LOGO_VIEW_COMPONENT_NAME;

            return string.Empty;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new PayPalSmartPaymentButtonsSettings
            {
                UseSandbox = true,
                PaymentType = PaymentType.Capture,
                LogoInHeaderLinks = @"<!-- PayPal Logo --><li><a href=""https://www.paypal.com/webapps/mpp/paypal-popup"" title=""How PayPal Works"" onclick=""javascript:window.open('https://www.paypal.com/webapps/mpp/paypal-popup','WIPaypal','toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, width=1060, height=700'); return false;""><img style=""padding-top:10px;"" src=""https://www.paypalobjects.com/webstatic/mktg/logo/bdg_now_accepting_pp_2line_w.png"" border=""0"" alt=""Now accepting PayPal""></a></li><!-- PayPal Logo -->",
                LogoInFooter = @"<!-- PayPal Logo --><div><a href=""https://www.paypal.com/webapps/mpp/paypal-popup"" title=""How PayPal Works"" onclick=""javascript:window.open('https://www.paypal.com/webapps/mpp/paypal-popup','WIPaypal','toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, width=1060, height=700'); return false;""><img src=""https://www.paypalobjects.com/webstatic/mktg/logo/AM_mc_vs_dc_ae.jpg"" border=""0"" alt=""PayPal Acceptance Mark""></a></div><!-- PayPal Logo -->",
                StyleLayout = "vertical",
                StyleColor = "blue",
                StyleShape = "rect",
                StyleLabel = "paypal"
            });

            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(Defaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(Defaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

            //locales
            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Enums.Nop.Plugin.Payments.PayPalSmartPaymentButtons.Domain.PaymentType.Authorize"] = "Authorize",
                ["Enums.Nop.Plugin.Payments.PayPalSmartPaymentButtons.Domain.PaymentType.Capture"] = "Capture",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Credentials.Valid"] = "The specified credentials are valid",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Credentials.Invalid"] = "The specified credentials are invalid (see details in the <a href=\"{0}\" target=\"_blank\">log</a>)",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.DisplayButtonsOnProductDetails"] = "Display buttons on product details",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.DisplayButtonsOnProductDetails.Hint"] = "Determine whether to display PayPal buttons on product details pages, clicking on them matches the behavior of the default 'Add to cart' button.",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.DisplayButtonsOnShoppingCart"] = "Display buttons on shopping cart",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.DisplayButtonsOnShoppingCart.Hint"] = "Determine whether to display PayPal buttons on the shopping cart page instead of the default checkout button.",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.DisplayLogoInFooter"] = "Display logo in footer",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.DisplayLogoInFooter.Hint"] = "Determine whether to display PayPal logo in the footer. These logos and banners are a great way to let your buyers know that you choose PayPal to securely process their payments.",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.DisplayLogoInHeaderLinks"] = "Display logo in header links",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.DisplayLogoInHeaderLinks.Hint"] = "Determine whether to display PayPal logo in header links. These logos and banners are a great way to let your buyers know that you choose PayPal to securely process their payments.",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.ClientId"] = "Client ID",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.ClientId.Hint"] = "Enter your PayPal REST client ID. This identifies your PayPal account and determines where transactions are paid. While you're testing in sandbox, you can use 'sb' as a shortcut.",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.ClientId.Required"] = "Client ID is required",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.LogoInFooter"] = "Logo source code",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.LogoInFooter.Hint"] = "Enter source code of the logo. Find more logos and banners on PayPal Logo Center. You can also modify the code to fit correctly into your theme and site style.",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.LogoInHeaderLinks"] = "Logo source code",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.LogoInHeaderLinks.Hint"] = "Enter source code of the logo. Find more logos and banners on PayPal Logo Center. You can also modify the code to fit correctly into your theme and site style.",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.PaymentType"] = "Payment type",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.PaymentType.Hint"] = "Choose a payment type to either capture payment immediately or authorize a payment for an order after order creation.",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.SecretKey"] = "Secret",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.SecretKey.Hint"] = "Enter secret for your app.",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.SecretKey.Required"] = "Secret is required",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.UseSandbox"] = "Use sandbox",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Fields.UseSandbox.Hint"] = "Determine whether to use the sandbox environment for testing purposes.",
                ["Plugins.Payments.PayPalSmartPaymentButtons.OrderId"] = "PayPal order ID",
                ["Plugins.Payments.PayPalSmartPaymentButtons.Prominently"] = "PayPal Prominently",
                ["Plugins.Payments.PayPalSmartPaymentButtons.PaymentMethodDescription"] = "PayPal Checkout with using methods like Venmo, PayPal Credit, credit card payments",
                ["Plugins.Payments.PayPalSmartPaymentButtons.RoundingWarning"] = "It looks like you have <a href=\"{0}\" target=\"_blank\">RoundPricesDuringCalculation</a> setting disabled. Keep in mind that this can lead to a discrepancy of the order total amount, as PayPal rounds to two decimals only.",
                ["Plugins.Payments.PayPalSmartPaymentButtons.WebhookWarning"] = "Webhook was not created, so some functions may not work correctly (see details in the <a href=\"{0}\" target=\"_blank\">log</a>)"
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //webhooks
            foreach (var store in await _storeService.GetAllStoresAsync())
            {
                var settings = await _settingService.LoadSettingAsync<PayPalSmartPaymentButtonsSettings>(store.Id);
                if (!string.IsNullOrEmpty(settings.WebhookId))
                    await _serviceManager.DeleteWebhookAsync(settings);
            }

            //settings
            if (_widgetSettings.ActiveWidgetSystemNames.Contains(Defaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Remove(Defaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }
            await _settingService.DeleteSettingAsync<PayPalSmartPaymentButtonsSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Enums.Nop.Plugin.Payments.PayPalSmartPaymentButtons");
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Payments.PayPalSmartPaymentButtons");

            await base.UninstallAsync();
        }

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            return await _localizationService.GetResourceAsync("Plugins.Payments.PayPalSmartPaymentButtons.PaymentMethodDescription");
        }

        #endregion

        #region Properies

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture => true;

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid => true;

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund => true;

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund => true;

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo => false;
        
        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => true;

        #endregion
    }
}