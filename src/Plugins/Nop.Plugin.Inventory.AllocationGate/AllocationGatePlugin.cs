using Nop.Core.Domain.ScheduleTasks;
using Nop.Plugin.Inventory.AllocationGate.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Inventory.AllocationGate;

public class AllocationGatePlugin : BasePlugin, IMiscPlugin
{
    private readonly ISettingService _settingService;
    private readonly IScheduleTaskService _scheduleTaskService;

    public AllocationGatePlugin(ISettingService settingService, IScheduleTaskService scheduleTaskService)
    {
        _settingService = settingService;
        _scheduleTaskService = scheduleTaskService;
    }

    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new AllocationSettings());

        if (await _scheduleTaskService.GetTaskByTypeAsync(typeof(ReleaseExpiredReservationsTask).FullName) is null)
        {
            await _scheduleTaskService.InsertTaskAsync(new ScheduleTask
            {
                Name = "VerdeMart: release expired reservations",
                Seconds = 30,
                Type = typeof(ReleaseExpiredReservationsTask).FullName,
                Enabled = true,
                StopOnError = false
            });
        }

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        var task = await _scheduleTaskService.GetTaskByTypeAsync(typeof(ReleaseExpiredReservationsTask).FullName);
        if (task is not null)
            await _scheduleTaskService.DeleteTaskAsync(task);

        await _settingService.DeleteSettingAsync<AllocationSettings>();
        await base.UninstallAsync();
    }
}
