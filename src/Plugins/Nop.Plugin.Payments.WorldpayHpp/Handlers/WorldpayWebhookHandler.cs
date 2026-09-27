using System.Text.Json;
using Nop.Plugin.Payments.WorldpayHpp.Models.Webhook;
using Nop.Plugin.Payments.WorldpayHpp.Models.Webhook.Payment;
using Nop.Plugin.Payments.WorldpayHpp.Models.Webhook.Token;

namespace Nop.Plugin.Payments.WorldpayHpp.Handlers;
public class WorldpayWebhookHandler : IWorldpayWebhookHandler
{
    private readonly IPaymentEventHandler _paymentEventHandler;
    private readonly ITokenCreatedEventHandler _tokenCreatedEventHandler;

    public WorldpayWebhookHandler(IPaymentEventHandler paymentEventHandler, ITokenCreatedEventHandler tokenCreatedEventHandler)
    {
        _paymentEventHandler = paymentEventHandler;
        _tokenCreatedEventHandler = tokenCreatedEventHandler;
    }

    public async Task HandleAsync(WorldpayWebhookEnvelope worldpayWebhookEnvelope, CancellationToken cancellationToken = default)
    {

        var envelope = worldpayWebhookEnvelope;
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        if (envelope.IsTokenEvent)
        {
            var tokenEventDetails = envelope.EventDetails.Deserialize<TokenCreatedEventDetails>(jsonOptions);
            await _tokenCreatedEventHandler.HandleAsync(tokenEventDetails, cancellationToken);
            return;
        }

        var paymentEventDetails = envelope.EventDetails.Deserialize<PaymentEventDetails>(jsonOptions);
        await _paymentEventHandler.HandleAsync(paymentEventDetails, cancellationToken);
    }
}
