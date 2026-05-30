using Nop.Core.Domain.ScheduleTasks;
using Nop.Plugin.Fulfillment.OpenBoxes.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Fulfillment.OpenBoxes;

public class OpenBoxesPlugin : BasePlugin, IMiscPlugin
{
    private readonly ISettingService _settingService;
    private readonly IScheduleTaskService _scheduleTaskService;

    public OpenBoxesPlugin(ISettingService settingService, IScheduleTaskService scheduleTaskService)
    {
        _settingService = settingService;
        _scheduleTaskService = scheduleTaskService;
    }

    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new OpenBoxesSettings());

        if (await _scheduleTaskService.GetTaskByTypeAsync(typeof(OpenBoxesStatusPollerTask).FullName) is null)
        {
            await _scheduleTaskService.InsertTaskAsync(new ScheduleTask
            {
                Name = "VerdeMart: poll OpenBoxes fulfillment order status",
                Seconds = 30,
                Type = typeof(OpenBoxesStatusPollerTask).FullName,
                Enabled = true,
                StopOnError = false
            });
        }

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        var openBoxesTask = await _scheduleTaskService.GetTaskByTypeAsync(typeof(OpenBoxesStatusPollerTask).FullName);
        if (openBoxesTask is not null)
            await _scheduleTaskService.DeleteTaskAsync(openBoxesTask);

        await _settingService.DeleteSettingAsync<OpenBoxesSettings>();
        await base.UninstallAsync();
    }
}
