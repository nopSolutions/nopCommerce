using Nop.Plugin.Payments.WorldpayHpp.Models.Webhook.Token;
using Nop.Services.Logging;

namespace Nop.Plugin.Payments.WorldpayHpp.Handlers;
public class TokenCreatedEventHandler : ITokenCreatedEventHandler
{

    private readonly ILogger _logger;


    public TokenCreatedEventHandler(ILogger logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(TokenCreatedEventDetails tokenCreatedEventDetails, CancellationToken cancellationToken)
    {

        await _logger.InformationAsync ($"TokenCreated for TransactionReference: {tokenCreatedEventDetails.TransactionReference}");

    }
}
