using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Plugins;

namespace Nop.Plugin.Shipping.CarrierTracking;

public class CarrierTrackingPlugin : BasePlugin, IMiscPlugin
{
    private readonly ISettingService _settingService;

    public CarrierTrackingPlugin(ISettingService settingService)
    {
        _settingService = settingService;
    }

    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new CarrierTrackingSettings());
        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await _settingService.DeleteSettingAsync<CarrierTrackingSettings>();
        await base.UninstallAsync();
    }
}
