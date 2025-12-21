using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Momo.Components;
using Nop.Plugin.Payments.Momo.Models;
using Nop.Plugin.Payments.Momo.Services;
using Nop.Plugin.Payments.Momo.Validators;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;

namespace Nop.Plugin.Payments.Momo;

/// <summary>
/// Manual payment processor
/// </summary>
public class MomoPaymentProcessor : BasePlugin, IPaymentMethod
{
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderTotalCalculationService _orderTotalCalculationService;
    protected readonly ISettingService _settingService;
    protected readonly IWebHelper _webHelper;
    protected readonly MomoPaymentSettings _momoPaymentSettings;
    protected readonly MomoPaymentClient _momoPaymentClient;

    #endregion


    #region Ctor

    public MomoPaymentProcessor(
        ILocalizationService localizationService,
        IOrderTotalCalculationService orderTotalCalculationService,
        ISettingService settingService,
        IWebHelper webHelper,
        MomoPaymentSettings momoPaymentSettings,
        MomoPaymentClient momoPaymentClient)
    {
        _localizationService = localizationService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _settingService = settingService;
        _webHelper = webHelper;
        _momoPaymentSettings = momoPaymentSettings;
        _momoPaymentClient = momoPaymentClient;
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
    public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
        if (processPaymentRequest.CustomValues.TryGetValue("PaymentCompleted", out var paymentValue) &&
            paymentValue.Value == "true")
        {
            return Task.FromResult(new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Paid, });
        }
        return Task.FromResult(new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Pending, });
    }

    /// <summary>
    /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
    /// </summary>
    /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
    {
        //nothing
        return Task.CompletedTask;
    }

    /// <summary>
    /// Returns a value indicating whether payment method should be hidden during checkout
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - hide; false - display.
    /// </returns>
    public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
    {
        //you can put any logic here
        //for example, hide this payment method if all products in the cart are downloadable
        //or hide this payment method if current customer is from certain country
        return Task.FromResult(false);
    }

    /// <summary>
    /// Captures payment
    /// </summary>
    /// <param name="capturePaymentRequest">Capture payment request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the capture payment result
    /// </returns>
    public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
    {
        return Task.FromResult(new CapturePaymentResult { Errors = new[] { "Capture method not supported" } });
    }

    /// <summary>
    /// Refunds a payment
    /// </summary>
    /// <param name="refundPaymentRequest">Request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
    {
        return Task.FromResult(new RefundPaymentResult { Errors = new[] { "Refund method not supported" } });
    }

    /// <summary>
    /// Voids a payment
    /// </summary>
    /// <param name="voidPaymentRequest">Request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
    {
        return Task.FromResult(new VoidPaymentResult { Errors = new[] { "Void method not supported" } });
    }


    /// <summary>
    /// Cancels a recurring payment
    /// </summary>
    /// <param name="cancelPaymentRequest">Request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(
        CancelRecurringPaymentRequest cancelPaymentRequest)
    {
        //always success
        return Task.FromResult(new CancelRecurringPaymentResult());
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
        ArgumentNullException.ThrowIfNull(order);

        //it's not a redirection payment method. So we always return false
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
        var warnings = new List<string>();

        //validate
        var validator = new PaymentInfoValidator(_localizationService);
        var model = new PaymentInfoModel
        {
            PhoneNumber = form["PhoneNumber"],
        };
        var validationResult = validator.Validate(model);
        if (!validationResult.IsValid)
            warnings.AddRange(validationResult.Errors.Select(error => error.ErrorMessage));

        return Task.FromResult<IList<string>>(warnings);
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
        return Task.FromResult(new ProcessPaymentRequest
        {
            CustomValues =
            [
                new CustomValue("PhoneNumber", form["PhoneNumber"]),
                new CustomValue("PaymentCompleted", form["PaymentCompleted"])
            ]
        });
    }

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/PaymentMomo/Configure";
    }

    /// <summary>
    /// Gets a type of a view component for displaying plugin in public store ("payment info" checkout step)
    /// </summary>
    /// <returns>View component type</returns>
    public Type GetPublicViewComponent()
    {
        return typeof(PaymentMomoViewComponent);
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        //settings
        var settings = new MomoPaymentSettings
        {
            ApiKey = "",
            ApiUser = "",
            SubscriptionKey = "4c91dae7a6f1474387a23a1f3d448eb7",
            Environment = "sandbox",
            Currency = "GHS", // Ghanaian Cedi is the default currency for MTN MoMo in Ghana
            CallbackUrl = "https://localhost:44300/Plugins/PaymentMomo/Return"
        };
        await _settingService.SaveSettingAsync(settings);

        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Payments.Momo.Instructions"] = "Configure your MTN MoMo API credentials to enable mobile money payments.",
            ["Plugins.Payments.Momo.PaymentMethodDescription"] = "Pay with MTN MoMo",
            
            // Field Labels and Hints
            ["Plugins.Payments.Momo.Fields.SubscriptionKey"] = "Subscription Key",
            ["Plugins.Payments.Momo.Fields.SubscriptionKey.Hint"] = "Enter your MTN MoMo API subscription key",
            
            ["Plugins.Payments.Momo.Fields.ApiUser"] = "API User ID",
            ["Plugins.Payments.Momo.Fields.ApiUser.Hint"] = "Your MTN MoMo API User ID",
            
            ["Plugins.Payments.Momo.Fields.ApiKey"] = "API Key",
            ["Plugins.Payments.Momo.Fields.ApiKey.Hint"] = "Your MTN MoMo API Key",
            
            ["Plugins.Payments.Momo.Fields.CallbackUrl"] = "Callback URL",
            ["Plugins.Payments.Momo.Fields.CallbackUrl.Hint"] = "The URL that MTN MoMo will call after payment processing",

            // API Setup Section
            ["Plugins.Payments.Momo.ApiSetup"] = "API User Configuration",
            ["Plugins.Payments.Momo.CreateApiUser"] = "Create API User",
            ["Plugins.Payments.Momo.GenerateApiKey"] = "Generate API Key",

            // Messages
            ["Plugins.Payments.Momo.ApiUserCreated"] = "API User created successfully",
            ["Plugins.Payments.Momo.ApiKeyGenerated"] = "API Key generated successfully",
            ["Plugins.Payments.Momo.Error.ApiUserCreation"] = "Error creating API User",
            ["Plugins.Payments.Momo.Error.ApiKeyGeneration"] = "Error generating API Key"
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        //settings
        await _settingService.DeleteSettingAsync<MomoPaymentSettings>();

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Payments.Momo");

        await base.UninstallAsync();
    }

    /// <summary>
    /// Gets a payment method description that will be displayed on checkout pages in the public store
    /// </summary>
    /// <remarks>
    /// return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
    /// for example, for a redirection payment method, description may be like this: "You will be redirected to PayPal site to complete the payment"
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<string> GetPaymentMethodDescriptionAsync()
    {
        return await _localizationService.GetResourceAsync("Plugins.Payments.Momo.PaymentMethodDescription");
    }

    /// <summary>
    /// Gets an additional handling fee of the payment method
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the additional handling fee
    /// </returns>
    public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
    {
        // No additional handling fee for MTN MoMo payments
        return Task.FromResult(0m);
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
        // MTN MoMo doesn't support recurring payments
        return Task.FromResult(new ProcessPaymentResult 
        { 
            Errors = new[] { "Recurring payments not supported" } 
        });
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether capture is supported
    /// </summary>
    public bool SupportCapture => false;

    /// <summary>
    /// Gets a value indicating whether partial refund is supported
    /// </summary>
    public bool SupportPartiallyRefund => false;

    /// <summary>
    /// Gets a value indicating whether refund is supported
    /// </summary>
    public bool SupportRefund => false;

    /// <summary>
    /// Gets a value indicating whether void is supported
    /// </summary>
    public bool SupportVoid => false;

    /// <summary>
    /// Gets a recurring payment type of payment method
    /// </summary>
    public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.Manual;

    /// <summary>
    /// Gets a payment method type
    /// </summary>
    public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

    /// <summary>
    /// Gets a value indicating whether we should display a payment information page for this plugin
    /// </summary>
    public bool SkipPaymentInfo => false;

    #endregion
}