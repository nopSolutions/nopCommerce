using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;

namespace Nop.Plugin.Payments.Bitcoin;

/// <summary>
/// Bitcoin payment processor
/// </summary>
public class BitcoinPaymentProcessor : BasePlugin, IPaymentMethod
{
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderTotalCalculationService _orderTotalCalculationService;
    protected readonly ISettingService _settingService;
    protected readonly IWebHelper _webHelper;
    protected readonly BitcoinPaymentSettings _bitcoinPaymentSettings;

    #endregion

    #region Ctor

    public BitcoinPaymentProcessor(ILocalizationService localizationService,
        IOrderTotalCalculationService orderTotalCalculationService,
        ISettingService settingService,
        IWebHelper webHelper,
        BitcoinPaymentSettings bitcoinPaymentSettings)
    {
        _localizationService = localizationService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _settingService = settingService;
        _webHelper = webHelper;
        _bitcoinPaymentSettings = bitcoinPaymentSettings;
    }

    public bool SupportCapture => throw new NotImplementedException();

    public bool SupportPartiallyRefund => throw new NotImplementedException();

    public bool SupportRefund => throw new NotImplementedException();

    public bool SupportVoid => throw new NotImplementedException();

    public RecurringPaymentType RecurringPaymentType => throw new NotImplementedException();

    public PaymentMethodType PaymentMethodType => throw new NotImplementedException();

    public bool SkipPaymentInfo => throw new NotImplementedException();

    public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CanRePostProcessPaymentAsync(Order order)
    {
        throw new NotImplementedException();
    }

    public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
    {
        throw new NotImplementedException();
    }

    public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
    {
        throw new NotImplementedException();
    }

    public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetPaymentMethodDescriptionAsync()
    {
        throw new NotImplementedException();
    }

    public Type GetPublicViewComponent()
    {
        throw new NotImplementedException();
    }

    public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
    {
        throw new NotImplementedException();
    }

    public Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
    {
        throw new NotImplementedException();
    }

    public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
        throw new NotImplementedException();
    }

    public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
        throw new NotImplementedException();
    }

    public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
    {
        throw new NotImplementedException();
    }

    public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
    {
        throw new NotImplementedException();
    }

    public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Methods



    #endregion

    #region Properties



    #endregion
}