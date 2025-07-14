using Nop.Core.Domain.Messages;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.RFQ.Services;

public class RfqMessageTokenEventConsumer : IConsumer<AdditionalTokensAddedEvent>
{
    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public Task HandleEventAsync(AdditionalTokensAddedEvent eventMessage)
    {
        eventMessage.AddTokens("%RequestQuote.Id%", "%RequestQuote.CreatedOn%", "%RequestQuote.URL%", "%Quote.Id%", "%Quote.CreatedOn%", "%Quote.ExpirationOn%", "%Quote.ExpirationOnIsSet%", "%Quote.URL%");

        return Task.CompletedTask;
    }

    #endregion
}
