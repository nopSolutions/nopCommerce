using Nop.Core.Domain.Messages;

namespace Nop.Services.Reminders;

/// <summary>
/// Reminder service interface
/// </summary>
public partial interface IReminderService
{
    #region Method

    /// <summary>
    /// Updates the message template parameters of the reminder
    /// </summary>
    /// <param name="templateName">Name of the message template</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="enabled">Value indicating that the template is active</param>
    /// <param name="delayBeforeSend">Delay before sending message</param>
    /// <param name="delayPeriod">Period of message delay</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateFollowUpAsync(string templateName, int storeId, bool enabled, int delayBeforeSend, MessageDelayPeriod delayPeriod);

    #endregion
}