using Amazon.Pay.API.Types;
using Amazon.Pay.API.WebStore.Charge;
using Amazon.Pay.API.WebStore.ChargePermission;
using Amazon.Pay.API.WebStore.Refund;
using Amazon.Pay.API.WebStore.Types;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.AmazonPay.Domain;
using Nop.Plugin.Payments.AmazonPay.Enums;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;

namespace Nop.Plugin.Payments.AmazonPay.Services;

/// <summary>
/// Represents the service for payment related methods
/// </summary>
public class AmazonPayPaymentService
{
    #region Fields

    private readonly AmazonPayApiService _amazonPayApiService;
    private readonly AmazonPaySettings _amazonPaySettings;
    private readonly IActionContextAccessor _actionContextAccessor;
    private readonly ICurrencyService _currencyService;
    private readonly ICustomerService _customerService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly ILogger _logger;
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly IOrderService _orderService;
    private readonly IPriceCalculationService _priceCalculationService;
    private readonly IStoreContext _storeContext;
    private readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public AmazonPayPaymentService(AmazonPayApiService amazonPayApiService,
        AmazonPaySettings amazonPaySettings,
        IActionContextAccessor actionContextAccessor,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        ILogger logger,
        IOrderProcessingService orderProcessingService,
        IOrderService orderService,
        IPriceCalculationService priceCalculationService,
        IStoreContext storeContext,
        IStoreService storeService)
    {
        _amazonPayApiService = amazonPayApiService;
        _amazonPaySettings = amazonPaySettings;
        _actionContextAccessor = actionContextAccessor;
        _currencyService = currencyService;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _logger = logger;
        _orderProcessingService = orderProcessingService;
        _orderService = orderService;
        _priceCalculationService = priceCalculationService;
        _storeContext = storeContext;
        _storeService = storeService;
    }

    #endregion

    #region Utilities

    private async Task<bool> IsInProgressAsync(Order order, InProgressType inProgressType, bool? flag = null)
    {
        if (order is null)
            return false;

        async Task<bool> getOrSetValue(string key)
        {
            if (flag.HasValue)
            {
                await _genericAttributeService.SaveAttributeAsync(order, key, flag.Value);

                return flag.Value;
            }

            return await _genericAttributeService.GetAttributeAsync<bool>(order, key);
        }

        return inProgressType switch
        {
            InProgressType.Void => await getOrSetValue(AmazonPayDefaults.VoidInProgressAttributeName),
            InProgressType.Capture => await getOrSetValue(AmazonPayDefaults.CaptureInProgressAttributeName),
            InProgressType.Refund => await getOrSetValue(AmazonPayDefaults.RefundInProgressAttributeName),
            _ => false,
        };
    }

    private async Task<Order> GetOrderByChargeAsync(string chargeId)
    {
        // send the request
        var result = await _amazonPayApiService.PerformRequestAsync(client => client.GetCharge(chargeId));

        return await GetOrderByChargeAsync(result);
    }

    private async Task<Order> GetOrderByChargeAsync(ChargeResponse charge)
    {
        if (Guid.TryParse(charge.MerchantMetadata.MerchantReferenceId, out var orderGuid))
            return await _orderService.GetOrderByGuidAsync(orderGuid);

        return null;
    }

    private async Task<Order> GetOrderByChargePermissionAsync(ChargePermissionResponse chargePermission)
    {
        if (Guid.TryParse(chargePermission.MerchantMetadata.MerchantReferenceId, out var orderGuid))
            return await _orderService.GetOrderByGuidAsync(orderGuid);

        return null;
    }

    private async Task RefundOrderAsync(IpnMessage message)
    {
        var result = await _amazonPayApiService.PerformRequestAsync(client => client.GetRefund(message.ObjectId));

        if (!result.StatusDetails.State.Equals("refunded", StringComparison.InvariantCultureIgnoreCase))
            return;

        var order = await GetOrderByChargeAsync(result.ChargeId);
        if (order == null)
            return;

        var refundAmount = result.RefundAmount.Amount;

        if (!await _orderProcessingService.CanPartiallyRefundAsync(order, refundAmount))
            return;

        await _genericAttributeService
            .SaveAttributeAsync(order, AmazonPayDefaults.RefundRequestAttributeName, 0, (await _storeContext.GetCurrentStoreAsync()).Id);

        await IsInProgressAsync(order, InProgressType.Refund, true);
        await _orderProcessingService.PartiallyRefundAsync(order, refundAmount);
    }

    private async Task ChangePaymentStatusAsync(IpnMessage message)
    {
        var result = await _amazonPayApiService.PerformRequestAsync(client => client.GetCharge(message.ObjectId));

        if (result.StatusDetails.State.Equals("captured", StringComparison.InvariantCultureIgnoreCase))
        {
            var order = await GetOrderByChargeAsync(result);

            if (await IsInProgressAsync(order, InProgressType.Capture))
            {
                await IsInProgressAsync(order, InProgressType.Capture, false);

                return;
            }

            if (order == null)
                return;

            if (string.IsNullOrEmpty(order.AuthorizationTransactionId))
                return;

            if (!await _orderProcessingService.CanCaptureAsync(order))
                return;

            order.CaptureTransactionId = order.AuthorizationTransactionId;
            order.AuthorizationTransactionId = null;

            await IsInProgressAsync(order, InProgressType.Capture, true);
            await _orderProcessingService.CaptureAsync(order);
        }

        if (result.StatusDetails.State.Equals("canceled", StringComparison.InvariantCultureIgnoreCase))
        {
            var order = await GetOrderByChargeAsync(result);

            if (await IsInProgressAsync(order, InProgressType.Void))
            {
                await IsInProgressAsync(order, InProgressType.Void, false);

                return;
            }

            if (order == null)
                return;

            if (string.IsNullOrEmpty(order.AuthorizationTransactionId))
                return;

            if (!await _orderProcessingService.CanVoidAsync(order))
                return;

            order.AuthorizationTransactionId = null;
            await IsInProgressAsync(order, InProgressType.Void, true);
            await _orderProcessingService.VoidAsync(order);
        }
    }

    private async Task ChangeSubscriptionStatusAsync(IpnMessage message)
    {
        var result = await _amazonPayApiService.PerformRequestAsync(client => client.GetChargePermission(message.ObjectId));

        if (result.ChargePermissionType != ChargePermissionType.Recurring)
            return;

        if (!result.StatusDetails.State.Equals("closed", StringComparison.InvariantCultureIgnoreCase))
            return;

        var order = await GetOrderByChargePermissionAsync(result);
        if (order is null)
            return;

        var recurringPayment = (await _orderService.SearchRecurringPaymentsAsync(initialOrderId: order.Id)).FirstOrDefault();
        if (recurringPayment is null)
            return;

        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
        if (!await _orderProcessingService.CanCancelRecurringPaymentAsync(customer, recurringPayment))
            return;

        await _orderProcessingService.CancelRecurringPaymentAsync(recurringPayment);
    }

    private async Task<bool> IsChargePermissionChargeableAsync(string chargePermissionId)
    {
        var result = await _amazonPayApiService.PerformRequestAsync(client => client.GetChargePermission(chargePermissionId));

        return result.StatusDetails.State.Equals("Chargeable", StringComparison.InvariantCultureIgnoreCase);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Process IPN request
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ProcessIPNRequestAsync()
    {
        try
        {
            var request = _actionContextAccessor.ActionContext?.HttpContext.Request;

            if (request == null || (request.ContentLength ?? 0) == 0)
                return;

            using var streamReader = new StreamReader(request.Body);
            var body = await streamReader.ReadToEndAsync();

            if (string.IsNullOrEmpty(body))
                return;

            if (_amazonPaySettings.EnableLogging)
            {
                var logMessage = $"{AmazonPayDefaults.PluginSystemName} IPN request details:{System.Environment.NewLine}{body}";
                await _logger.InsertLogAsync(Core.Domain.Logging.LogLevel.Debug, $"{AmazonPayDefaults.PluginSystemName} IPN request details", logMessage);
            }

            var rawData = JsonConvert.DeserializeAnonymousType(body, new { MessageId = string.Empty, Message = string.Empty });

            if (string.IsNullOrEmpty(rawData?.MessageId))
                return;

            var message = JsonConvert.DeserializeObject<IpnMessage>(rawData.Message);

            if (message == null || string.IsNullOrEmpty(message.ObjectId))
                return;

            switch (message.ObjectType.ToUpper())
            {
                case "REFUND":
                    await RefundOrderAsync(message);
                    break;
                case "CHARGE":
                    await ChangePaymentStatusAsync(message);
                    break;
                case "CHARGE_PERMISSION":
                    await ChangeSubscriptionStatusAsync(message);
                    break;
            }
        }
        catch (Exception exception)
        {
            var logMessage = $"{AmazonPayDefaults.PluginSystemName} error:{System.Environment.NewLine}{exception.Message}";
            await _logger.ErrorAsync(logMessage, exception);
        }
    }

    /// <summary>
    /// Capture a payment
    /// </summary>
    /// <param name="capturePaymentRequest">Capture request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the capture result
    /// </returns>
    public async Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
    {
        var result = new CapturePaymentResult();
        var order = capturePaymentRequest.Order;

        if (await IsInProgressAsync(order, InProgressType.Capture))
        {
            await IsInProgressAsync(order, InProgressType.Capture, false);

            result.CaptureTransactionId = order.CaptureTransactionId;
            result.NewPaymentStatus = PaymentStatus.Paid;

            return result;
        }

        if (string.IsNullOrEmpty(order.AuthorizationTransactionId))
        {
            result.Errors.Add("Transaction already captured");

            return result;
        }

        var (currencyCode, _) = await _amazonPayApiService.GetCurrencyAsync(order.CustomerCurrencyCode);
        var orderTotal = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);

        // prepare the request
        var request = new CaptureChargeRequest(orderTotal, currencyCode);

        await IsInProgressAsync(order, InProgressType.Capture, true);
        await _amazonPayApiService.PerformRequestAsync(client => client.CaptureCharge(order.AuthorizationTransactionId, request, _amazonPayApiService.Headers));

        result.CaptureTransactionId = order.AuthorizationTransactionId;
        result.NewPaymentStatus = PaymentStatus.Paid;
        order.AuthorizationTransactionId = null;

        return result;
    }

    /// <summary>
    /// Void a payment
    /// </summary>
    /// <param name="voidPaymentRequest">Void request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the void result
    /// </returns>
    public async Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
    {
        var result = new VoidPaymentResult();

        var order = voidPaymentRequest.Order;

        if (await IsInProgressAsync(order, InProgressType.Void))
        {
            await IsInProgressAsync(order, InProgressType.Void, false);

            result.NewPaymentStatus = PaymentStatus.Voided;

            return result;
        }

        if (string.IsNullOrEmpty(order.AuthorizationTransactionId))
        {
            result.Errors.Add("Transaction already voided");

            return result;
        }

        // prepare the request
        var request = new CancelChargeRequest(string.Empty);
        await _amazonPayApiService.PerformRequestAsync(client => client.CancelCharge(order.AuthorizationTransactionId, request));

        result.NewPaymentStatus = PaymentStatus.Voided;
        order.AuthorizationTransactionId = null;

        return result;
    }

    /// <summary>
    /// Refund a payment
    /// </summary>
    /// <param name="refundPaymentRequest">Refund request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the refund result
    /// </returns>
    public async Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
    {
        var result = new RefundPaymentResult();
        var order = refundPaymentRequest.Order;

        if (await IsInProgressAsync(order, InProgressType.Refund))
        {
            result.NewPaymentStatus = refundPaymentRequest.IsPartialRefund
                ? PaymentStatus.PartiallyRefunded
                : PaymentStatus.Refunded;

            await IsInProgressAsync(order, InProgressType.Refund, false);

            return result;
        }

        var store = await _storeContext.GetCurrentStoreAsync();
        var refundAmount = await _genericAttributeService
            .GetAttributeAsync<decimal>(order, AmazonPayDefaults.RefundRequestAttributeName, store.Id);

        if (refundAmount != 0)
        {
            result.AddError("Previous refund request not finished yet");

            return result;
        }

        var (currencyCode, _) = await _amazonPayApiService.GetCurrencyAsync(order.CustomerCurrencyCode);
        var amountToRefund = _currencyService.ConvertCurrency(refundPaymentRequest.AmountToRefund, order.CurrencyRate);
        await _genericAttributeService
            .SaveAttributeAsync(order, AmazonPayDefaults.RefundRequestAttributeName, amountToRefund, store.Id);

        // prepare the request
        var request = new CreateRefundRequest(order.CaptureTransactionId, amountToRefund, currencyCode);
        await _amazonPayApiService.PerformRequestAsync(client => client.CreateRefund(request, _amazonPayApiService.Headers), async message =>
        {
            result.AddError(message.Message);

            await _genericAttributeService
                .SaveAttributeAsync<decimal>(order, AmazonPayDefaults.RefundRequestAttributeName, 0, store.Id);
        });

        if (result.Success)
            result.AddError("Refunds are processed asynchronously and will be processed later");

        return result;
    }

    /// <summary>
    /// Process recurring payment
    /// </summary>
    /// <param name="processPaymentRequest">Payment info required for an order processing</param>
    /// <param name="retryIfSoftDeclined">Indicates whether payment should be attempted again when it is soft declined</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the process payment result
    /// </returns>
    public async Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest, bool retryIfSoftDeclined = true)
    {
        if (processPaymentRequest.InitialOrder == null)
            return new ProcessPaymentResult();

        var store = await _storeService.GetStoreByIdAsync(processPaymentRequest.InitialOrder.StoreId);
        var processResult = new ProcessPaymentResult();
        var chargePermissionId = processPaymentRequest.InitialOrder.SubscriptionTransactionId;

        var (currencyCode, currency) = await _amazonPayApiService.GetCurrencyAsync(processPaymentRequest.InitialOrder.CustomerCurrencyCode);
        var orderTotal = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(processPaymentRequest.OrderTotal, currency);
        orderTotal = await _priceCalculationService.RoundPriceAsync(orderTotal, currency);

        //prepare the request
        var request = new CreateChargeRequest(chargePermissionId, orderTotal, currencyCode)
        {
            CaptureNow = _amazonPaySettings.PaymentType == PaymentType.Capture,
            CanHandlePendingAuthorization = false,
            PlatformId = AmazonPayDefaults.SpId
        };
        request.MerchantMetadata.CustomInformation = AmazonPayDefaults.IntegrationName;
        request.MerchantMetadata.MerchantStoreName = store.Name;
        request.MerchantMetadata.MerchantReferenceId = processPaymentRequest.OrderGuid.ToString();

        var response = await _amazonPayApiService.PerformRequestAsync(client => client.CreateCharge(request, _amazonPayApiService.Headers),
            async message =>
            {
                processResult.Errors.Add(message.Message ?? message.ReasonCode);
                processResult.RecurringPaymentFailed = true;

                if (message.ReasonCode is not null)
                {
                    if ((message.ReasonCode.Equals("SoftDeclined", StringComparison.InvariantCultureIgnoreCase) ||
                        message.ReasonCode.Equals("ProcessingFailure", StringComparison.InvariantCultureIgnoreCase)) &&
                        retryIfSoftDeclined &&
                        await IsChargePermissionChargeableAsync(chargePermissionId))
                    {
                        processResult.RecurringPaymentFailed = false;
                        processResult = await ProcessRecurringPaymentAsync(processPaymentRequest, false);
                    }

                    if (message.ReasonCode.Equals("HardDeclined", StringComparison.InvariantCultureIgnoreCase))
                        processResult.Errors.Add($@"Buyer may update their payment instrument using the following link: https://payments.amazon.com/jr/your-account/ba/{chargePermissionId}");
                }
            });

        if (!response.Success || !processResult.Success)
            return processResult;

        processResult.AuthorizationTransactionId = response.ChargeId;
        processResult.AuthorizationTransactionResult = "Authorization was either successfully completed or successfully initiated";
        processResult.SubscriptionTransactionId = chargePermissionId;
        processResult.NewPaymentStatus = PaymentStatus.Authorized;

        return processResult;
    }

    /// <summary>
    /// Cancel a recurring payment
    /// </summary>
    /// <param name="cancelPaymentRequest">Cancel request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cancel result
    /// </returns>
    public async Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
    {
        var result = new CancelRecurringPaymentResult();

        //prepare the request
        var chargePermissionId = cancelPaymentRequest.Order.SubscriptionTransactionId;
        var request = new CloseChargePermissionRequest("No more charges required");

        await _amazonPayApiService.PerformRequestAsync(
            client => client.CloseChargePermission(chargePermissionId, request),
            message =>
            {
                result.Errors.Add(message.Message);
                return Task.CompletedTask;
            });

        return result;
    }

    #endregion

    #region Nested class

    /// <summary>
    /// Represents in-progress type enumeration
    /// </summary>
    private enum InProgressType
    {
        /// <summary>
        /// Void in-progress
        /// </summary>
        Void,

        /// <summary>
        /// Capture in-progress
        /// </summary>
        Capture,

        /// <summary>
        /// Refund in-progress
        /// </summary>
        Refund
    }

    #endregion
}