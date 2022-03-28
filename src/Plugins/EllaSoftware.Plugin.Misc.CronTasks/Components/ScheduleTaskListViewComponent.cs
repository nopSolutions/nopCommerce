using System.Linq;
using System.Threading.Tasks;
using EllaSoftware.Plugin.Misc.CronTasks.Services;
using Microsoft.AspNetCore.Mvc;

namespace EllaSoftware.Plugin.Misc.CronTasks.Components
{
    [ViewComponent(Name = CronTasksDefaults.ScheduleTaskListViewComponentName)]
    public class ScheduleTaskListViewComponent : ViewComponent
    {
        private readonly ICronTaskService _cronTaskService;

        public ScheduleTaskListViewComponent(
            ICronTaskService cronTaskService)
        {
            _cronTaskService = cronTaskService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cronTaskScheduleTaskIds = (await _cronTaskService.GetCronTasksAsync()).Select(t => t.Key).ToArray();

            return View("~/Plugins/EllaSoftware.CronTasks/Views/Shared/Components/ScheduleTaskList.cshtml", cronTaskScheduleTaskIds);
        }
    }
}
