using Nop.Services.Messages;
using Nop.Services.Plugins;

namespace Nop.Plugin.Sms.Twilio;

public class TwilioMethod : BasePlugin, ISmsProvider
{
    public Task<bool> SendSmsAsync(string phone, string text)
    {
        return Task.FromResult(true);
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        await base.UninstallAsync();
    }    
}
