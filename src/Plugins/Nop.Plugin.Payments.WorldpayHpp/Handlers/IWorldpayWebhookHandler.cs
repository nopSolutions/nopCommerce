using Nop.Plugin.Payments.WorldpayHpp.Models.Webhook;

namespace Nop.Plugin.Payments.WorldpayHpp.Handlers;
public interface IWorldpayWebhookHandler
{
    Task HandleAsync(WorldpayWebhookEnvelope worldpayWebhookEnvelope, CancellationToken cancellationToken =  default);
   
}
