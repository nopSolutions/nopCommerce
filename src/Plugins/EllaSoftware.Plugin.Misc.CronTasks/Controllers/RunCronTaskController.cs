using System.Linq;
using System.Threading.Tasks;
using EllaSoftware.Plugin.Misc.CronTasks.Services;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Tasks;
using Quartz;

namespace EllaSoftware.Plugin.Misc.CronTasks.Controllers
{
    //do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
    //they can create guest account(s), etc
    public class RunCronTaskController : Controller
    {
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ICronTaskService _cronTaskService;
        private readonly IScheduler _scheduler;

        public RunCronTaskController(
            IScheduleTaskService scheduleTaskService,
            ICronTaskService cronTaskService,
            IScheduler scheduler)
        {
            _scheduleTaskService = scheduleTaskService;
            _cronTaskService = cronTaskService;
            _scheduler = scheduler;
        }

        [HttpPost]
        public async Task<IActionResult> Index(int id)
        {
            var scheduleTask = await _scheduleTaskService.GetTaskByIdAsync(id);
            if (scheduleTask == null)
                return NoContent();

            await _cronTaskService.ExecuteCronTaskAsync(scheduleTask);

            return NoContent();
        }
    }
}
