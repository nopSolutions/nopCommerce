namespace Nop.Services.Messages;

/// <summary>
/// Notification service interface
/// </summary>
public partial interface INotificationService
{
    /// <summary>
    /// Display notification
    /// </summary>
    /// <param name="type">Notification type</param>
    /// <param name="message">Message</param>
    /// <param name="encode">A value indicating whether the message should not be encoded</param>
    /// <param name="timeout">The time on which notification will close automatically; leave 0 if notification shouldn't close automatically</param>
    void Notification(NotifyType type, string message, bool encode = true, int timeout = 0);

    /// <summary>
    /// Display success notification
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="encode">A value indicating whether the message should not be encoded</param>
    /// <param name="timeout">The time on which notification will close automatically; leave 0 if notification shouldn't close automatically</param>
    void SuccessNotification(string message, bool encode = true, int timeout = 0);

    /// <summary>
    /// Display warning notification
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="encode">A value indicating whether the message should not be encoded</param>
    /// <param name="timeout">The time on which notification will close automatically; leave 0 if notification shouldn't close automatically</param>
    void WarningNotification(string message, bool encode = true, int timeout = 0);

    /// <summary>
    /// Display error notification
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="encode">A value indicating whether the message should not be encoded</param>
    /// <param name="timeout">The time on which notification will close automatically; leave 0 if notification shouldn't close automatically</param>
    void ErrorNotification(string message, bool encode = true, int timeout = 0);

    /// <summary>
    /// Display error notification
    /// </summary>
    /// <param name="exception">Exception</param>
    /// <param name="logException">A value indicating whether exception should be logged</param>
    /// <param name="timeout">The time on which notification will close automatically; leave 0 if notification shouldn't close automatically</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task ErrorNotificationAsync(Exception exception, bool logException = true, int timeout = 0);
}