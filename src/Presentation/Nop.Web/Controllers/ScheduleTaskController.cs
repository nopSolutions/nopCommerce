using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.ScheduleTasks;

namespace Nop.Web.Controllers
{
    //do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
    //they can create guest account(s), etc
    [AutoValidateAntiforgeryToken]
    public partial class ScheduleTaskController : Controller
    {
        protected IScheduleTaskService ScheduleTaskService { get; }
        protected IScheduleTaskRunner TaskRunner { get; }

        public ScheduleTaskController(IScheduleTaskService scheduleTaskService,
            IScheduleTaskRunner taskRunner)
        {
            ScheduleTaskService = scheduleTaskService;
            TaskRunner = taskRunner;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> RunTask(string taskType)
        {
            var scheduleTask = await ScheduleTaskService.GetTaskByTypeAsync(taskType);
            if (scheduleTask == null)
                //schedule task cannot be loaded
                return NoContent();

            await TaskRunner.ExecuteAsync(scheduleTask);

            return NoContent();
        }
    }
}