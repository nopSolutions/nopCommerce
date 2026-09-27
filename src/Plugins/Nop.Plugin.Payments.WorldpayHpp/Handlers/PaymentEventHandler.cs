using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.WorldpayHpp.Constants;
using Nop.Plugin.Payments.WorldpayHpp.Models;
using Nop.Plugin.Payments.WorldpayHpp.Models.Webhook.Payment;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Orders;

namespace Nop.Plugin.Payments.WorldpayHpp.Handlers;
public class PaymentEventHandler : IPaymentEventHandler
{

    
    private readonly IOrderService _orderService;
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly ISettingService _settingService;
    private readonly ILogger _logger;

    public PaymentEventHandler(IOrderService orderService, IOrderProcessingService orderProcessingService, ISettingService settingService, ILogger logger)
    {
        _orderService = orderService;
        _orderProcessingService = orderProcessingService;
        _settingService = settingService;
        _logger = logger;
    }

    public async Task HandleAsync(PaymentEventDetails paymentEventDetails, CancellationToken cancellationToken)
    {

        switch (paymentEventDetails.Type)
        {

            case PaymentEvent.SentForAuthorization:
                // Handle payment sent for authorization event
                // You can log the event or perform any necessary actions here
               await _logger.InformationAsync($"Payment sent for authorization, TransactionReference: {paymentEventDetails.TransactionReference}");
                break;
           
            case PaymentEvent.Authorized:
                // Handle payment authorized event
                var worldpaySettings = await _settingService.LoadSettingAsync<WorldpayHppSettings>();
                var prefix = worldpaySettings.TransactionReferencePrefix ?? string.Empty;
                var orderIdString = string.IsNullOrEmpty(prefix) ? paymentEventDetails.TransactionReference :  paymentEventDetails.TransactionReference.Replace(prefix, string.Empty);
                var orderId = int.Parse(orderIdString);
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order != null && order.PaymentStatus == Nop.Core.Domain.Payments.PaymentStatus.Pending)
                {
                    // Do NOT call MarkOrderAsPaidAsync
                    order.PaymentStatus = PaymentStatus.Paid;
                    // Keep the order open for fulfilment
                     order.OrderStatus = OrderStatus.Processing; 
                    await _orderService.UpdateOrderAsync(order);
                }

                break;


            case PaymentEvent.SentForSettlement:
                // Handle payment sent for authorization event
                // You can log the event or perform any necessary actions here
                await _logger.InformationAsync($"Payment sent for settlement, TransactionReference: {paymentEventDetails.TransactionReference}");
                break;

            case PaymentEvent.Cancelled:
                // Handle payment sent for authorization event
                // You can log the event or perform any necessary actions here
               await _logger.InformationAsync($"Payment cancelled, TransactionReference: {paymentEventDetails.TransactionReference}");
                break;

            case PaymentEvent.Error:
                // Handle payment sent for authorization event
                // You can log the event or perform any necessary actions here
               await _logger.InformationAsync($"An error happened,The payment wasn't completed. Your customer may want to reattempt the payment, TransactionReference: {paymentEventDetails.TransactionReference}");
                break;

            case PaymentEvent.Expired:
                // Handle payment sent for authorization event
                // You can log the event or perform any necessary actions here
              await  _logger.InformationAsync($"Payment Expired. The authorization period ended before a settlement or cancel request was made, TransactionReference: {paymentEventDetails.TransactionReference}");
                break;

            case PaymentEvent.Refused:
                // Handle payment sent for authorization event
                // You can log the event or perform any necessary actions here
             await   _logger.InformationAsync($"Payment Refused. Your payment request has been declined by a third party, TransactionReference: {paymentEventDetails.TransactionReference}");
                break;

            case PaymentEvent.SentForRefund:
                // Handle payment sent for authorization event
                // You can log the event or perform any necessary actions here
              await  _logger.InformationAsync($"Sent for Refund. You've requested funds to be sent back to your customer's account, TransactionReference: {paymentEventDetails.TransactionReference}");
                break;

            case PaymentEvent.RefundFailed:
                // Handle payment sent for authorization event
                // You can log the event or perform any necessary actions here
             await   _logger.InformationAsync($"Refund Failed. The refund couldn't be processed and the funds were returned to your account., TransactionReference: {paymentEventDetails.TransactionReference}");
                break;

            // Add more cases as needed for different payment event types
            default:
                // Handle unknown event type
                break;
        }
    }
}
