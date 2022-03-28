using EllaSoftware.Plugin.Misc.CronTasks.Services;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Tasks;
using Nop.Web.Areas.Admin.Models.Tasks;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Task = System.Threading.Tasks.Task;

namespace EllaSoftware.Plugin.Misc.CronTasks.EventConsumers
{
    public class ScheduleTaskModelEventConsumer : IConsumer<ModelReceivedEvent<BaseNopModel>>
    {
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ILocalizationService _localizationService;
        private readonly ICronTaskService _cronTaskService;

        public ScheduleTaskModelEventConsumer(
            IScheduleTaskService scheduleTaskService,
            ILocalizationService localizationService,
            ICronTaskService cronTaskService)
        {
            _scheduleTaskService = scheduleTaskService;
            _localizationService = localizationService;
            _cronTaskService = cronTaskService;
        }

        public async Task HandleEventAsync(ModelReceivedEvent<BaseNopModel> eventMessage)
        {
            //whether received model is ScheduleTaskModel
            if (!(eventMessage?.Model is ScheduleTaskModel scheduleTaskModel))
                return;

            var cronTasks = await _cronTaskService.GetCronTasksAsync();
            if (!cronTasks.ContainsKey(scheduleTaskModel.Id))
                return;

            //check task interval if cron task has proper state
            if (scheduleTaskModel.Enabled)
            {
                var error = await _localizationService.GetResourceAsync("EllaSoftware.Plugin.Misc.CronTasks.ScheduleTask.RemoveCronTaskBeforeEnable");

                eventMessage.ModelState.AddModelError(string.Empty, error);
            }
        }
    }
}
