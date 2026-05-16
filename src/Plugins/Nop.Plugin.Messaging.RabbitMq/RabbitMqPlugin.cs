using Nop.Core.Domain.ScheduleTasks;
using Nop.Plugin.Messaging.RabbitMq.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Messaging.RabbitMq;

public class RabbitMqPlugin : BasePlugin, IMiscPlugin
{
    private readonly ISettingService _settingService;
    private readonly IScheduleTaskService _scheduleTaskService;

    public RabbitMqPlugin(ISettingService settingService, IScheduleTaskService scheduleTaskService)
    {
        _settingService = settingService;
        _scheduleTaskService = scheduleTaskService;
    }

    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new RabbitMqSettings());

        if (await _scheduleTaskService.GetTaskByTypeAsync(typeof(OutboxDispatcherTask).FullName) is null)
        {
            await _scheduleTaskService.InsertTaskAsync(new ScheduleTask
            {
                Name = "VerdeMart: outbox dispatcher",
                Seconds = 10,
                Type = typeof(OutboxDispatcherTask).FullName,
                Enabled = true,
                StopOnError = false
            });
        }

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        var task = await _scheduleTaskService.GetTaskByTypeAsync(typeof(OutboxDispatcherTask).FullName);
        if (task is not null)
            await _scheduleTaskService.DeleteTaskAsync(task);

        await _settingService.DeleteSettingAsync<RabbitMqSettings>();
        await base.UninstallAsync();
    }
}
