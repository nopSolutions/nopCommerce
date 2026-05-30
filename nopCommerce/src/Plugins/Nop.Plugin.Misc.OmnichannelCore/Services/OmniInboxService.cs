using Nop.Data;
using Nop.Plugin.Misc.OmnichannelCore.Domains;
using Nop.Plugin.Misc.OmnichannelCore.Models.Callbacks;

namespace Nop.Plugin.Misc.OmnichannelCore.Services;

/// <summary>
/// Represents inbound integration message idempotency service
/// </summary>
public class OmniInboxService
{
    #region Fields

    private readonly IRepository<OmniInboxMessage> _inboxMessageRepository;

    #endregion

    #region Ctor

    public OmniInboxService(IRepository<OmniInboxMessage> inboxMessageRepository)
    {
        _inboxMessageRepository = inboxMessageRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Tries to register a message before callback processing
    /// </summary>
    /// <param name="context">Inbound message context</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<InboxProcessingResult> TryBeginProcessingAsync(InboxMessageContext context)
    {
        var existingMessage = await _inboxMessageRepository.Table
            .FirstOrDefaultAsync(message => message.MessageId == context.MessageId);

        if (existingMessage != null)
            return new InboxProcessingResult
            {
                IsDuplicate = true,
                InboxMessage = existingMessage
            };

        var inboxMessage = new OmniInboxMessage
        {
            MessageId = context.MessageId,
            OrderGuid = context.OrderGuid,
            EventType = context.EventType,
            CorrelationId = context.CorrelationId,
            Source = context.Source,
            Status = OmniInboxMessageStatus.Received,
            ReceivedOnUtc = DateTime.UtcNow
        };

        await _inboxMessageRepository.InsertAsync(inboxMessage);

        return new InboxProcessingResult
        {
            IsDuplicate = false,
            InboxMessage = inboxMessage
        };
    }

    /// <summary>
    /// Marks an inbox message as processed
    /// </summary>
    /// <param name="inboxMessage">Inbox message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task MarkProcessedAsync(OmniInboxMessage inboxMessage)
    {
        inboxMessage.Status = OmniInboxMessageStatus.Processed;
        inboxMessage.ProcessedOnUtc = DateTime.UtcNow;
        inboxMessage.UpdatedOnUtc = inboxMessage.ProcessedOnUtc;

        await _inboxMessageRepository.UpdateAsync(inboxMessage);
    }

    /// <summary>
    /// Marks an inbox message as failed
    /// </summary>
    /// <param name="inboxMessage">Inbox message</param>
    /// <param name="error">Failure detail</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task MarkFailedAsync(OmniInboxMessage inboxMessage, string error)
    {
        inboxMessage.Status = OmniInboxMessageStatus.Failed;
        inboxMessage.LastError = error;
        inboxMessage.UpdatedOnUtc = DateTime.UtcNow;

        await _inboxMessageRepository.UpdateAsync(inboxMessage);
    }

    #endregion
}
