using Nop.Plugin.Payments.WorldpayHpp.Models.Webhook.Token;

namespace Nop.Plugin.Payments.WorldpayHpp.Handlers;
public interface ITokenCreatedEventHandler
{
    Task HandleAsync(TokenCreatedEventDetails tokenCreatedEventDetails, CancellationToken cancellationToken);
}
