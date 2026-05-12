using Nop.Services.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Common;

namespace Nop.Plugin.Messaging.RabbitMq;

public class RabbitMqPlugin : BasePlugin, IMiscPlugin
{
    private readonly ISettingService _settingService;

    public RabbitMqPlugin(ISettingService settingService)
    {
        _settingService = settingService;
    }

    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new RabbitMqSettings());
        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await _settingService.DeleteSettingAsync<RabbitMqSettings>();
        await base.UninstallAsync();
    }
}
