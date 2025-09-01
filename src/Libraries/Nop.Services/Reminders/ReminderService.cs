using Nop.Core.Domain.Messages;
using Nop.Services.Messages;

namespace Nop.Services.Reminders;

/// <summary>
/// Reminder service
/// </summary>
public partial class ReminderService : IReminderService
{
    #region Fields

    protected readonly IMessageTemplateService _messageTemplateService;

    #endregion

    #region Ctor

    public ReminderService(IMessageTemplateService messageTemplateService)
    {
        _messageTemplateService = messageTemplateService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Updates the message template parameters of the reminder
    /// </summary>
    /// <param name="templateName">Name of the message template</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="enabled">Value indicating that the template is active</param>
    /// <param name="delayBeforeSend">Delay before sending message</param>
    /// <param name="delayPeriod">Period of message delay</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateFollowUpAsync(string templateName, int storeId, bool enabled, int delayBeforeSend, MessageDelayPeriod delayPeriod)
    {
        ArgumentException.ThrowIfNullOrEmpty(templateName);

        if (enabled)
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(delayBeforeSend);

        //we update only the first message template
        var messageTemplates = await _messageTemplateService.GetMessageTemplatesByNameAsync(templateName, storeId);
        var followUp = messageTemplates.FirstOrDefault();
        if (followUp is null)
            return;

        followUp.IsActive = enabled;

        if (followUp.IsActive)
        {
            followUp.DelayBeforeSend = delayBeforeSend;
            followUp.DelayPeriod = delayPeriod;
        }

        await _messageTemplateService.UpdateMessageTemplateAsync(followUp);
    }

    #endregion
}