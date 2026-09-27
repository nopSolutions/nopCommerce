using Nop.Plugin.Payments.WorldpayHpp.Models.Webhook.Payment;

namespace Nop.Plugin.Payments.WorldpayHpp.Handlers;
public interface IPaymentEventHandler
{
    Task HandleAsync(PaymentEventDetails paymentEventDetails, CancellationToken cancellationToken);
}
