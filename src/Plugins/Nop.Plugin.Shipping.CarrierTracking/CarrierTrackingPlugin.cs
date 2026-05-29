using Nop.Core.Domain.ScheduleTasks;
using Nop.Plugin.Shipping.CarrierTracking.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Shipping.CarrierTracking;

public class CarrierTrackingPlugin : BasePlugin, IMiscPlugin
{
    private readonly ISettingService _settingService;
    private readonly IScheduleTaskService _scheduleTaskService;

    public CarrierTrackingPlugin(ISettingService settingService, IScheduleTaskService scheduleTaskService)
    {
        _settingService = settingService;
        _scheduleTaskService = scheduleTaskService;
    }

    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new CarrierTrackingSettings());

        if (await _scheduleTaskService.GetTaskByTypeAsync(typeof(CarrierStatusPollerTask).FullName) is null)
        {
            await _scheduleTaskService.InsertTaskAsync(new ScheduleTask
            {
                Name = "VerdeMart: carrier status poller",
                Seconds = 120,
                Type = typeof(CarrierStatusPollerTask).FullName,
                Enabled = true,
                StopOnError = false
            });
        }

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        var task = await _scheduleTaskService.GetTaskByTypeAsync(typeof(CarrierStatusPollerTask).FullName);
        if (task is not null)
            await _scheduleTaskService.DeleteTaskAsync(task);

        await _settingService.DeleteSettingAsync<CarrierTrackingSettings>();
        await base.UninstallAsync();
    }
}
