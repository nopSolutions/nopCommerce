using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.MercadoPagoPlugin.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Plugins;

namespace Nop.Plugin.Payments.MercadoPagoPlugin;
/// <summary>
/// Rename this file and change to the correct type
/// </summary>
public class CustomPlugin : BasePlugin, IPaymentMethod
{
    private readonly MercadoPagoPaymentSettings _mercadoPagoPaymentSettings;
    private readonly ISettingService _settingService;
    private readonly ILocalizationService _localizationService;

    public CustomPlugin(MercadoPagoPaymentSettings mercadoPagoPaymentSettings,
                        ISettingService settingService,
                        ILocalizationService localizationService)
    {
        _mercadoPagoPaymentSettings = mercadoPagoPaymentSettings;
        _settingService = settingService;
        _localizationService = localizationService;
    }

    public async Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
        var result = new ProcessPaymentResult();
        try
        {
            // Lógica para procesar el pago con Mercado Pago
            var payment = await MercadoPagoService.CreatePaymentAsync(processPaymentRequest);
            result.NewPaymentStatus = PaymentStatus.Authorized;
            result.RedirectUrl = payment.InitPoint;
        }
        catch (Exception ex)
        {
            result.AddError(ex.Message);
        }
        return result;
    }

    public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
    {
        // Lógica para manejar el post-procesamiento del pago
    }

    public async Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
    {
        var result = new VoidPaymentResult();
        try
        {
            await MercadoPagoService.VoidPaymentAsync(voidPaymentRequest);
            result.NewPaymentStatus = PaymentStatus.Voided;
        }
        catch (Exception ex)
        {
            result.AddError(ex.Message);
        }
        return result;
    }

    public async Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
    {
        var result = new RefundPaymentResult();
        try
        {
            await MercadoPagoService.RefundPaymentAsync(refundPaymentRequest);
            result.NewPaymentStatus = PaymentStatus.Refunded;
        }
        catch (Exception ex)
        {
            result.AddError(ex.Message);
        }
        return result;
    }

    public async Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
    {
        var result = new CapturePaymentResult();
        try
        {
            await MercadoPagoService.CapturePaymentAsync(capturePaymentRequest);
            result.NewPaymentStatus = PaymentStatus.Paid;
        }
        catch (Exception ex)
        {
            result.AddError(ex.Message);
        }
        return result;
    }

    public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
    {
        return 0;
    }

    public string GetPaymentMethodDescription()
    {
        return _localizationService.GetResource("Plugins.Payments.MercadoPago.Description");
    }
    public bool SupportCapture => throw new NotImplementedException();

    public bool SupportPartiallyRefund => throw new NotImplementedException();

    public bool SupportRefund => throw new NotImplementedException();

    public bool SupportVoid => throw new NotImplementedException();

    public RecurringPaymentType RecurringPaymentType => throw new NotImplementedException();

    public PaymentMethodType PaymentMethodType => throw new NotImplementedException();

    public bool SkipPaymentInfo => throw new NotImplementedException();

    public override async Task InstallAsync()
    {
        await base.InstallAsync();
    }
    public override async Task UninstallAsync()
    {
        await base.UninstallAsync();
    }
    //public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    //{
    //    throw new NotImplementedException();
    //}

    //public Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
    //{
    //    throw new NotImplementedException();
    //}

    public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
    {
        throw new NotImplementedException();
    }

    public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
    {
        throw new NotImplementedException();
    }

    //public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
    //{
    //    throw new NotImplementedException();
    //}

    //public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
    //{
    //    throw new NotImplementedException();
    //}

    //public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
    //{
    //    throw new NotImplementedException();
    //}

    public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
        throw new NotImplementedException();
    }

    public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CanRePostProcessPaymentAsync(Order order)
    {
        throw new NotImplementedException();
    }

    public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
    {
        throw new NotImplementedException();
    }

    public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
    {
        throw new NotImplementedException();
    }

    public Type GetPublicViewComponent()
    {
        throw new NotImplementedException();
    }

    public Task<string> GetPaymentMethodDescriptionAsync()
    {
        throw new NotImplementedException();
    }

    
}