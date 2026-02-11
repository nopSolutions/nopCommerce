using Nop.Services.Plugins;

namespace Nop.Services.Messages;

/// <summary>
/// Provides an interface for SMS providers
/// </summary>
public partial interface ISmsProvider : IPlugin
{
    /// <summary>
    /// Sends SMS
    /// </summary>
    /// <param name="phone">Phone number</param>
    /// <param name="text">Text</param>
    Task<bool> SendSmsAsync(string phone, string text);
}