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
        if (eventMessage.MessageTemplate == null || string.IsNullOrEmpty(eventMessage.MessageTemplate.Name))
            return Task.CompletedTask;

        if (eventMessage.MessageTemplate.Name.Equals(RfqDefaults.CUSTOMER_SENT_NEW_REQUEST_QUOTE, StringComparison.InvariantCultureIgnoreCase)
            || eventMessage.MessageTemplate.Name.Equals(RfqDefaults.ADMIN_SENT_NEW_QUOTE, StringComparison.InvariantCultureIgnoreCase))
            eventMessage.AddTokens("%RequestQuote.Id%", "%RequestQuote.CreatedOn%", "%RequestQuote.URL%");

        if (eventMessage.MessageTemplate.Name.Equals(RfqDefaults.ADMIN_SENT_NEW_QUOTE, StringComparison.InvariantCultureIgnoreCase))
            eventMessage.AddTokens("%Quote.Id%", "%Quote.CreatedOn%", "%Quote.ExpirationOn%", "%Quote.ExpirationOnIsSet%", "%Quote.URL%");

        return Task.CompletedTask;
    }

    #endregion
}
