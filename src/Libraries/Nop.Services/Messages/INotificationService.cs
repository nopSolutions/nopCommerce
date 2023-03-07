namespace Nop.Services.Messages
{
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
        void Notification(NotifyType type, string message, bool encode = true);

        /// <summary>
        /// Display success notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        void SuccessNotification(string message, bool encode = true);

        /// <summary>
        /// Display warning notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        void WarningNotification(string message, bool encode = true);

        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        void ErrorNotification(string message, bool encode = true);

        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="logException">A value indicating whether exception should be logged</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ErrorNotificationAsync(Exception exception, bool logException = true);
    }
}
