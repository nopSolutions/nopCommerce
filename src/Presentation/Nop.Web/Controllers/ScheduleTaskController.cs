using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Tasks;
using Task = Nop.Services.Tasks.Task;

namespace Nop.Web.Controllers
{
    //do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
    //they can create guest account(s), etc
    public partial class ScheduleTaskController : Controller
    {
        private readonly IScheduleTaskService _scheduleTaskService;

        public ScheduleTaskController(IScheduleTaskService scheduleTaskService)
        {
            _scheduleTaskService = scheduleTaskService;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> RunTask(string taskType)
        {
            var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(taskType);
            if (scheduleTask == null)
                //schedule task cannot be loaded
                return NoContent();

            var task = new Task(scheduleTask);
            await task.ExecuteAsync();

            return NoContent();
        }
    }
}