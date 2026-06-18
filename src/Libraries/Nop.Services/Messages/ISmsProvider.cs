using Nop.Services.Plugins;

namespace Nop.Services.Messages;

/// <summary>
/// Provides an interface for SMS providers
/// </summary>
public partial interface ISmsProvider : IPlugin
{
    /// <summary>
    /// Sends an SMS message to the specified phone number
    /// </summary>
    /// <param name="phone">The destination phone number for the SMS message</param>
    /// <param name="text">The text content of the SMS message</param>
    /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the SMS message
    /// was sent successfully; otherwise, <see langword="false"/>.</returns>
    Task<bool> SendSmsAsync(string phone, string text);
}