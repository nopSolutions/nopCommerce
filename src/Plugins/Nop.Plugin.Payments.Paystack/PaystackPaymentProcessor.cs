using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Paystack.Components;
using Nop.Plugin.Payments.Paystack.Models;
using Nop.Plugin.Payments.Paystack.Services;
using Nop.Plugin.Payments.Paystack.Validators;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;

namespace Nop.Plugin.Payments.Paystack;

/// <summary>
/// Paystack payment method plugin
/// </summary>
public class PaystackPaymentProcessor : BasePlugin, IPaymentMethod
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILocalizationService _localizationService;
    private readonly IOrderTotalCalculationService _orderTotalCalculationService;
    private readonly ISettingService _settingService;
    private readonly IWebHelper _webHelper;
    private readonly PaystackPaymentClient _paymentClient;
    private readonly IPaystackTransactionService _transactionService;
    private readonly IStoreContext _storeContext;
    private readonly IAddressService _addressService;
    private readonly ICustomerService _customerService;
    private readonly IWorkContext _workContext;

    public PaystackPaymentProcessor(
        IHttpContextAccessor httpContextAccessor,
        ILocalizationService localizationService,
        IOrderTotalCalculationService orderTotalCalculationService,
        ISettingService settingService,
        IWebHelper webHelper,
        PaystackPaymentClient paymentClient,
        IPaystackTransactionService transactionService,
        IStoreContext storeContext,
        IAddressService addressService,
        ICustomerService customerService,
        IWorkContext workContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _localizationService = localizationService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _settingService = settingService;
        _webHelper = webHelper;
        _paymentClient = paymentClient;
        _transactionService = transactionService;
        _storeContext = storeContext;
        _addressService = addressService;
        _customerService = customerService;
        _workContext = workContext;
    }

    public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
        if (processPaymentRequest.CustomValues.TryGetValue("PaymentCompleted", out var paymentValue) &&
            string.Equals(paymentValue?.Value?.ToString(), "true", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Paid });

        return Task.FromResult(new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Pending });
    }

    public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
    {
        var order = postProcessPaymentRequest.Order;
        if (order == null)
            return;

        var storeId = order.StoreId;
        var settings = await _settingService.LoadSettingAsync<PaystackPaymentSettings>(storeId);
        if (string.IsNullOrWhiteSpace(settings.SecretKey))
            return;

        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
        var email = customer?.Email ?? billingAddress?.Email;
        if (string.IsNullOrWhiteSpace(email))
            return;

        var orderTotal = order.OrderTotal;
        var callbackUrl = _webHelper.GetStoreLocation().TrimEnd('/') + "/paystack/callback";
        var initResult = await _paymentClient.InitializeTransactionAsync(settings, email, orderTotal, null, callbackUrl);

        if (initResult?.Status != true || initResult.Data == null || string.IsNullOrWhiteSpace(initResult.Data.AuthorizationUrl))
            return;

        var reference = initResult.Data.Reference;
        await _transactionService.CreateTransactionAsync(reference, email, orderTotal, order.CustomerCurrencyCode ?? "NGN", order.Id);

        _httpContextAccessor.HttpContext?.Response.Redirect(initResult.Data.AuthorizationUrl, false);
    }

    public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart) => Task.FromResult(false);

    public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        => Task.FromResult(new CapturePaymentResult { Errors = new[] { "Capture not supported" } });

    public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        => Task.FromResult(new RefundPaymentResult { Errors = new[] { "Refund not supported" } });

    public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        => Task.FromResult(new VoidPaymentResult { Errors = new[] { "Void not supported" } });

    public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        => Task.FromResult(new CancelRecurringPaymentResult());

    public Task<bool> CanRePostProcessPaymentAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        return Task.FromResult(false);
    }

    public async Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
    {
        var email = await ResolveEmailFromFormOrCustomerAsync(form);
        var validator = new PaymentInfoValidator(_localizationService);
        var model = new PaymentInfoModel { Email = email };
        var result = await validator.ValidateAsync(model);
        return result.IsValid ? [] : result.Errors.Select(e => e.ErrorMessage).ToList();
    }

    public async Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
    {
        var resolvedEmail = await ResolveEmailFromFormOrCustomerAsync(form);

        return new ProcessPaymentRequest
        {
            CustomValues =
            [
                new CustomValue("Email", resolvedEmail),
                new CustomValue("PaymentCompleted", form["PaymentCompleted"].ToString())
            ]
        };
    }

    private async Task<string> ResolveEmailFromFormOrCustomerAsync(IFormCollection form)
    {
        var email = (form["Email"].ToString() ?? "").Trim();
        if (!string.IsNullOrEmpty(email))
            return email;
        var billingEmail = (form["BillingNewAddress.Email"].ToString() ?? "").Trim();
        if (!string.IsNullOrEmpty(billingEmail))
            return billingEmail;
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (customer == null)
            return "";
        email = (customer.Email ?? "").Trim();
        if (!string.IsNullOrEmpty(email))
            return email;
        var billingAddress = await _customerService.GetCustomerBillingAddressAsync(customer);
        return (billingAddress?.Email ?? "").Trim();
    }

    public override string GetConfigurationPageUrl()
        => $"{_webHelper.GetStoreLocation()}Admin/PaymentPaystack/Configure";

    public Type GetPublicViewComponent() => typeof(PaymentPaystackViewComponent);

    public override async Task InstallAsync()
    {
        var settings = new PaystackPaymentSettings
        {
            SecretKey = "",
            PublicKey = "",
            CallbackUrl = "",
            WebhookSecret = "",
            AdditionalFee = 0,
            AdditionalFeePercentage = false
        };
        await _settingService.SaveSettingAsync(settings);

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Payments.Paystack.Instructions"] = "Configure your Paystack API keys and callback URL. Use your Secret Key and Public Key from the Paystack Dashboard.",
            ["Plugins.Payments.Paystack.PaymentMethodDescription"] = "You will be redirected to Paystack to complete the payment.",
            ["Plugins.Payments.Paystack.Fields.SecretKey"] = "Secret Key",
            ["Plugins.Payments.Paystack.Fields.SecretKey.Hint"] = "Your Paystack Secret Key (starts with sk_test_ or sk_live_). Do NOT use the Public Key (pk_).",
            ["Plugins.Payments.Paystack.Fields.PublicKey"] = "Public Key",
            ["Plugins.Payments.Paystack.Fields.CallbackUrl"] = "Callback URL",
            ["Plugins.Payments.Paystack.Fields.CallbackUrl.Hint"] = "URL where the customer is redirected after payment (e.g. {0}/paystack/callback)",
            ["Plugins.Payments.Paystack.Fields.WebhookSecret"] = "Webhook Secret",
            ["Plugins.Payments.Paystack.Fields.WebhookSecret.Hint"] = "Optional. Leave empty to use Secret Key for webhook signature verification.",
            ["Plugins.Payments.Paystack.Fields.AdditionalFee"] = "Additional Fee",
            ["Plugins.Payments.Paystack.Fields.AdditionalFeePercentage"] = "Additional Fee (percentage)",
            ["Plugins.Payments.Paystack.Fields.Email"] = "Email",
            ["Plugins.Payments.Paystack.Fields.Email.Required"] = "Email is required.",
            ["Plugins.Payments.Paystack.PaymentNote"] = "You will be redirected to Paystack to complete the payment securely."
        });

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await _settingService.DeleteSettingAsync<PaystackPaymentSettings>();
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Payments.Paystack");
        await base.UninstallAsync();
    }

    public async Task<string> GetPaymentMethodDescriptionAsync()
        => await _localizationService.GetResourceAsync("Plugins.Payments.Paystack.PaymentMethodDescription");

    public async Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var settings = await _settingService.LoadSettingAsync<PaystackPaymentSettings>(store.Id);
        if (settings.AdditionalFee <= 0)
            return 0;

        if (!settings.AdditionalFeePercentage)
            return settings.AdditionalFee;

        var (subTotal, _, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart);
        return subTotal.HasValue ? (subTotal.Value * settings.AdditionalFee / 100m) : 0;
    }

    public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        => Task.FromResult(new ProcessPaymentResult { Errors = new[] { "Recurring payments not supported" } });

    public bool SupportCapture => false;
    public bool SupportPartiallyRefund => false;
    public bool SupportRefund => false;
    public bool SupportVoid => false;
    public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.Manual;
    public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;
    public bool SkipPaymentInfo => false;
}
