using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.AmazonPay.Components;
using Nop.Plugin.Payments.AmazonPay.Enums;
using Nop.Plugin.Payments.AmazonPay.Services;
using Nop.Services.Authentication.External;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.AmazonPay;

/// <summary>
/// Represents Amazon Pay plugin
/// </summary>
public class AmazonPayPlugin : BasePlugin, IExternalAuthenticationMethod, IPaymentMethod, IWidgetPlugin
{
    #region Fields

    private readonly AmazonPayApiService _amazonPayApiService;
    private readonly AmazonPayPaymentService _amazonPayPaymentService;
    private readonly AmazonPaySettings _amazonPaySettings;
    private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly IWebHelper _webHelper;
    private readonly PaymentSettings _paymentSettings;
    private readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public AmazonPayPlugin(AmazonPayApiService amazonPayApiService,
        AmazonPayPaymentService amazonPayPaymentService,
        AmazonPaySettings amazonPaySettings,
        ExternalAuthenticationSettings externalAuthenticationSettings,
        IHttpContextAccessor httpContextAccessor,
        ILocalizationService localizationService,
        ISettingService settingService,
        IWebHelper webHelper,
        PaymentSettings paymentSettings,
        WidgetSettings widgetSettings)
    {
        _amazonPayApiService = amazonPayApiService;
        _amazonPayPaymentService = amazonPayPaymentService;
        _amazonPaySettings = amazonPaySettings;
        _externalAuthenticationSettings = externalAuthenticationSettings;
        _httpContextAccessor = httpContextAccessor;
        _localizationService = localizationService;
        _settingService = settingService;
        _webHelper = webHelper;
        _paymentSettings = paymentSettings;
        _widgetSettings = widgetSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        //settings
        await _settingService.SaveSettingAsync(new AmazonPaySettings
        {
            SetCredentialsManually = false,
            PaymentRegion = PaymentRegion.Us,
            PaymentType = PaymentType.Capture,
            UseSandbox = false,
            EnableLogging = false,
            ButtonColor = ButtonColor.Gold,
            ButtonPlacement = new() { ButtonPlacement.Cart },
            LogoInFooter = @"<!-- AmazonPay Logo --><div><a href=""https://pay.amazon.com/what-is-amazon-pay"" title=""What is Amazon Pay?""><img src=""/Plugins/Payments.AmazonPay/Content/acceptance_mark.png"" alt=""What is Amazon Pay?"" style=""margin-top: -15px""></a></div><!-- AmazonPay Logo -->"
        });

        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Enums.Nop.Plugin.Payments.AmazonPay.Enums.ButtonColor.DarkGray"] = "Dark gray",
            ["Enums.Nop.Plugin.Payments.AmazonPay.Enums.ButtonColor.Gold"] = "Gold",
            ["Enums.Nop.Plugin.Payments.AmazonPay.Enums.ButtonColor.LightGray"] = "Light gray",
            ["Enums.Nop.Plugin.Payments.AmazonPay.Enums.ButtonPlacement.Cart"] = "Cart",
            ["Enums.Nop.Plugin.Payments.AmazonPay.Enums.ButtonPlacement.Checkout"] = "Checkout",
            ["Enums.Nop.Plugin.Payments.AmazonPay.Enums.ButtonPlacement.MiniCart"] = "Flyout shopping cart",
            ["Enums.Nop.Plugin.Payments.AmazonPay.Enums.ButtonPlacement.PaymentMethod"] = "Payment method",
            ["Enums.Nop.Plugin.Payments.AmazonPay.Enums.ButtonPlacement.Product"] = "Product",
            ["Enums.Nop.Plugin.Payments.AmazonPay.Enums.PaymentRegion.Eu"] = "Europe",
            ["Enums.Nop.Plugin.Payments.AmazonPay.Enums.PaymentRegion.Jp"] = "Japan",
            ["Enums.Nop.Plugin.Payments.AmazonPay.Enums.PaymentRegion.Uk"] = "United Kingdom",
            ["Enums.Nop.Plugin.Payments.AmazonPay.Enums.PaymentRegion.Us"] = "United States",
            ["Enums.Nop.Plugin.Payments.AmazonPay.Enums.PaymentType.Authorize"] = "Authorize",
            ["Enums.Nop.Plugin.Payments.AmazonPay.Enums.PaymentType.Capture"] = "Capture",

            ["Plugins.Payments.AmazonPay.CantUseAmazonPay"] = "The cart contains product that violates the <a href=\"https://pay.amazon.com/help/6023\" target=\"_blank\">Acceptable Use Policy (AUP)</a> of Amazon Pay",
            ["Plugins.Payments.AmazonPay.CheckoutWithAmazon"] = @"
                    <div class='pay-with-amazon'>
                        <h2>Checkout with Amazon</h2>
                        <ul>
                            <li>No need to register, use your amazon account to log in</li>
                            <li>Skip manually entering shipping address and payment details, simply use the information that is already stored within your Amazon account</li>
                            <li>Fully benefit from Amazon's A-z guaranty</li>
                        </ul>
                    </div>",
            ["Plugins.Payments.AmazonPay.Configure"] = "Configuration",
            ["Plugins.Payments.AmazonPay.Confirm.ChangeButton"] = "Change",
            ["Plugins.Payments.AmazonPay.Confirm.PlaceOrder"] = "Place order",
            ["Plugins.Payments.AmazonPay.Credentials"] = "Credentials",
            ["Plugins.Payments.AmazonPay.Currency.Incorrect.Warning"] = "The <a href=\"{0}\" target=\"_blank\">primary store currency</a> ({1}) cannot be used by AmazonPay in your payment region ({2}).",
            ["Plugins.Payments.AmazonPay.Currency.Warning"] = "The <a href=\"{0}\" target=\"_blank\">primary store currency</a> is not supported by AmazonPay. Check the supported currencies <a href=\"https://developer.amazon.com/docs/amazon-pay-checkout/multi-currency-integration.html#supported-currencies\" target=\"_blank\">here</a>.",
            ["Plugins.Payments.AmazonPay.DoNotUseWithAmazonPay"] = "Do not use with Amazon Pay",
            ["Plugins.Payments.AmazonPay.DoNotUseWithAmazonPay.Hint"] = "Indicates whether Amazon Pay can be used as a payment method.",
            ["Plugins.Payments.AmazonPay.PaymentMethodDescription"] = "Pay by Amazon Pay",

            ["Plugins.Payments.AmazonPay.Settings.ButtonColor"] = "Button color",
            ["Plugins.Payments.AmazonPay.Settings.ButtonColor.Hint"] = "Choose a color of the Amazon Pay button.",
            ["Plugins.Payments.AmazonPay.Settings.ButtonPlacement"] = "Button placement",
            ["Plugins.Payments.AmazonPay.Settings.ButtonPlacement.Hint"] = "Choose a placement of the Amazon Pay button on the store.",
            ["Plugins.Payments.AmazonPay.Settings.EnableLogging"] = "Enable logging",
            ["Plugins.Payments.AmazonPay.Settings.EnableLogging.Hint"] = "Determine whether to enable logging of all requests to Amazon Pay services.",
            ["Plugins.Payments.AmazonPay.Settings.MerchantId"] = "Merchant ID",
            ["Plugins.Payments.AmazonPay.Settings.MerchantId.Hint"] = "Specify the merchant ID.",
            ["Plugins.Payments.AmazonPay.Settings.MerchantId.Required"] = "Merchant ID is required",
            ["Plugins.Payments.AmazonPay.Settings.PaymentRegion"] = "Payment region",
            ["Plugins.Payments.AmazonPay.Settings.PaymentRegion.Hint"] = "Choose a payment region.",
            ["Plugins.Payments.AmazonPay.Settings.PaymentType"] = "Payment type",
            ["Plugins.Payments.AmazonPay.Settings.PaymentType.Hint"] = "Choose a payment type to either capture payment immediately or authorize a payment for an order after order creation.",
            ["Plugins.Payments.AmazonPay.Settings.PrivateKey"] = "Private key",
            ["Plugins.Payments.AmazonPay.Settings.PrivateKey.Hint"] = "Specify the PEM private key to access the Amazon Pay services.",
            ["Plugins.Payments.AmazonPay.Settings.PrivateKey.Required"] = "Private key is required",
            ["Plugins.Payments.AmazonPay.Settings.PublicKeyId"] = "Public key ID",
            ["Plugins.Payments.AmazonPay.Settings.PublicKeyId.Hint"] = "Specify the public key ID to access the Amazon Pay services.",
            ["Plugins.Payments.AmazonPay.Settings.PublicKeyId.Required"] = "Public key ID is required",
            ["Plugins.Payments.AmazonPay.Settings.StoreId"] = "Store ID",
            ["Plugins.Payments.AmazonPay.Settings.StoreId.Hint"] = "Specify the store ID",
            ["Plugins.Payments.AmazonPay.Settings.StoreId.Required"] = "Store ID is required",
            ["Plugins.Payments.AmazonPay.Settings.UseSandbox"] = "Use sandbox",
            ["Plugins.Payments.AmazonPay.Settings.UseSandbox.Hint"] = "Determine whether to use the sandbox environment for testing purposes.",
            ["Plugins.Payments.AmazonPay.Settings.SetCredentialsManually"] = "Specify credentials manually",
            ["Plugins.Payments.AmazonPay.Settings.SetCredentialsManually.Hint"] = "Determine whether to manually set the credentials (for example, there is already an account created, or if you want to use the sandbox mode).",
            ["Plugins.Payments.AmazonPay.Settings.Payload"] = "Payload",
            ["Plugins.Payments.AmazonPay.Settings.Payload.Hint"] = "Specify the plain text credential payload (JSON format) from Seller Central. Use this option if the credentials were not set automatically for some reason.",
            ["Plugins.Payments.AmazonPay.Settings.IpnUrl"] = "IPN URL",
            ["Plugins.Payments.AmazonPay.Settings.IpnUrl.Hint"] = "Set up Instant Payment Notifications (IPN) to receive notifications for events related to Amazon Pay transactions. IPN is used to update your order states and process transactions.",

            ["Plugins.Payments.AmazonPay.Onboarding"] = "Account",
            ["Plugins.Payments.AmazonPay.Onboarding.Button"] = "Create account",
            ["Plugins.Payments.AmazonPay.Onboarding.Error"] = "Error: {0} (see details in the <a href=\"{1}\" target=\"_blank\">log</a>)",
            ["Plugins.Payments.AmazonPay.Onboarding.Region.Warning"] = "Automated key sharing is not applicable in your payment region, please provide your credentials yourself",

            ["Plugins.Payments.AmazonPay.SignIn.AssociateButton"] = "Link accounts",
            ["Plugins.Payments.AmazonPay.SignIn.CreateButton"] = "Create account",
            ["Plugins.Payments.AmazonPay.SignIn.SignOutButton"] = "Sign out",
            ["Plugins.Payments.AmazonPay.SignIn.CreateAccount"] = "You can create a new customer account in this store with your Amazon Pay account, use 'Create account' button below",
            ["Plugins.Payments.AmazonPay.SignIn.LinkAccounts"] = "You can associate your Amazon Pay account with the current customer account, use 'Link accounts' button below",
            ["Plugins.Payments.AmazonPay.SignIn.LinkAccounts.ByEmail"] = "There is already a customer account registered for your email address in this store, you can associate it with your Amazon Pay account, use 'Link accounts' button below"
        });

        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(AmazonPayDefaults.PluginSystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(AmazonPayDefaults.PluginSystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        if (!_externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Contains(AmazonPayDefaults.PluginSystemName))
        {
            _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(AmazonPayDefaults.PluginSystemName);
            await _settingService.SaveSettingAsync(_externalAuthenticationSettings);
        }

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        if (_widgetSettings.ActiveWidgetSystemNames.Contains(AmazonPayDefaults.PluginSystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(AmazonPayDefaults.PluginSystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        if (_paymentSettings.ActivePaymentMethodSystemNames.Contains(AmazonPayDefaults.PluginSystemName))
        {
            _paymentSettings.ActivePaymentMethodSystemNames.Remove(AmazonPayDefaults.PluginSystemName);
            await _settingService.SaveSettingAsync(_paymentSettings);
        }

        await _settingService.DeleteSettingAsync<AmazonPaySettings>();

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Enums.Nop.Plugin.Payments.AmazonPay.Enums.");
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Payments.AmazonPay.");

        await base.UninstallAsync();
    }

    /// <summary>
    /// Process a payment
    /// </summary>
    /// <param name="processPaymentRequest">Payment info required for an order processing</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the process payment result
    /// </returns>
    public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
        return Task.FromResult(new ProcessPaymentResult());
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
    /// Returns a value indicating whether payment method should be hidden during checkout
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the rue - hide; false - display.
    /// </returns>
    public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
    {
        //you can put any logic here
        //for example, hide this payment method if all products in the cart are downloadable
        //or hide this payment method if current customer is from certain country
        return Task.FromResult(false);
    }

    /// <summary>
    /// Gets additional handling fee
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the additional handling fee
    /// </returns>
    public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
    {
        return Task.FromResult(decimal.Zero);
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
        return await _amazonPayPaymentService.CaptureAsync(capturePaymentRequest);
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
        return await _amazonPayPaymentService.RefundAsync(refundPaymentRequest);
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
        return await _amazonPayPaymentService.VoidAsync(voidPaymentRequest);
    }

    /// <summary>
    /// Process recurring payment
    /// </summary>
    /// <param name="processPaymentRequest">Payment info required for an order processing</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the process payment result
    /// </returns>
    public async Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
        return await _amazonPayPaymentService.ProcessRecurringPaymentAsync(processPaymentRequest);
    }

    /// <summary>
    /// Cancels a recurring payment
    /// </summary>
    /// <param name="cancelPaymentRequest">Request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public async Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
    {
        return await _amazonPayPaymentService.CancelRecurringPaymentAsync(cancelPaymentRequest);
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
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/AmazonPay/Configure";
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
        return Task.FromResult<IList<string>>(new List<string>());
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
        return Task.FromResult(new ProcessPaymentRequest());
    }

    /// <summary>
    /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
    /// </summary>
    /// <returns>View component name</returns>
    public Type GetPublicViewComponent()
    {
        return typeof(PaymentInfoViewComponent);
    }

    /// <summary>
    /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
    /// </summary>
    /// <returns>View component name</returns>
    Type IExternalAuthenticationMethod.GetPublicViewComponent()
    {
        if (_amazonPayApiService.IsActiveAndConfiguredAsync().Result)
            return typeof(PaymentInfoViewComponent);

        return null;
    }

    /// <summary>
    /// Gets a payment method description that will be displayed on checkout pages in the public store
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<string> GetPaymentMethodDescriptionAsync()
    {
        return await _localizationService.GetResourceAsync("Plugins.Payments.AmazonPay.PaymentMethodDescription");
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
            PublicWidgetZones.BodyStartHtmlTagAfter,
            PublicWidgetZones.OrderSummaryPaymentMethodInfo,
            PublicWidgetZones.OrderSummaryShippingAddress,
            PublicWidgetZones.ProductDetailsAddInfo,
            PublicWidgetZones.CheckoutProgressBefore,
            PublicWidgetZones.OpcContentBefore,
            PublicWidgetZones.Footer,
            AdminWidgetZones.ProductDetailsBlock,
            AdminWidgetZones.CategoryDetailsBlock
        });
    }

    /// <summary>
    /// Gets a type of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    /// <returns>View component type</returns>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        if (new List<string>
            {
                PublicWidgetZones.OrderSummaryPaymentMethodInfo,
                PublicWidgetZones.OrderSummaryShippingAddress
            }.Contains(widgetZone))
            return typeof(ChangeButtonViewComponent);

        if (new List<string>
            {
                PublicWidgetZones.ProductDetailsAddInfo,
                PublicWidgetZones.CheckoutProgressBefore,
                PublicWidgetZones.OpcContentBefore
            }.Contains(widgetZone)
            || widgetZone.Equals(PublicWidgetZones.BodyStartHtmlTagAfter))
            return typeof(PaymentButtonViewComponent);

        if (widgetZone.Equals(PublicWidgetZones.Footer))
            return typeof(LogoViewComponent);

        if (new List<string>
            {
                AdminWidgetZones.ProductDetailsBlock,
                AdminWidgetZones.CategoryDetailsBlock
            }.Contains(widgetZone))
            return typeof(DoNotUseWithAmazonPayViewComponent);

        return null;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether capture is supported
    /// </summary>
    public bool SupportCapture => true;

    /// <summary>
    /// Gets a value indicating whether partial refund is supported
    /// </summary>
    public bool SupportPartiallyRefund => true;

    /// <summary>
    /// Gets a value indicating whether refund is supported
    /// </summary>
    public bool SupportRefund => true;

    /// <summary>
    /// Gets a value indicating whether void is supported
    /// </summary>
    public bool SupportVoid => true;

    /// <summary>
    /// Gets a recurring payment type of payment method
    /// </summary>
    public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.Manual;

    /// <summary>
    /// Gets a payment method type
    /// </summary>
    public PaymentMethodType PaymentMethodType
    {
        get
        {
            var values = _httpContextAccessor.HttpContext?.Request.RouteValues;

            //checkout
            if (values != null &&
                values.ContainsKey("controller") &&
                (values["controller"]?.Equals("Checkout") ?? false) &&
                _amazonPaySettings.ButtonPlacement.Contains(ButtonPlacement.PaymentMethod))
                return PaymentMethodType.Standard;

            return PaymentMethodType.Button;
        }
    }

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