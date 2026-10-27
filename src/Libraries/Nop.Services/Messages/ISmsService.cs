namespace Nop.Services.Messages;

public partial interface ISmsService
{
    /// <summary>
    /// Sends an SMS message with the specified text content asynchronously
    /// </summary>
    /// <param name="phoneNumber">The phone number</param>
    /// <param name="text">The text content of the SMS message to send</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the boolean result of sending SMS message
    /// </returns>
    Task<bool> SendSmsAsync(string phoneNumber, string text);
}
