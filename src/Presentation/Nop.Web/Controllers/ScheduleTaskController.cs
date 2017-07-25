using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Tasks;

namespace Nop.Web.Controllers
{
    //do not inherit it from BasePublicController. otherwise a lot of extra acion filters will be called
    //they can create guest account(s), etc
    public partial class ScheduleTaskController : Controller
    {
        private readonly IScheduleTaskService _scheduleTaskService;

        public ScheduleTaskController(IScheduleTaskService scheduleTaskService)
        {
            this._scheduleTaskService = scheduleTaskService;
        }

        [HttpPost]
        public virtual IActionResult RunTask(string taskType)
        {
            var scheduleTask = _scheduleTaskService.GetTaskByType(taskType);
            if (scheduleTask == null)
                //schedule task cannot be loaded
                return NoContent();
            
            if(scheduleTask.LastEndUtc.HasValue && (DateTime.UtcNow-scheduleTask.LastEndUtc).Value.TotalSeconds < scheduleTask.Seconds)
                //too early
                return NoContent();

            var task = new Task(scheduleTask);
            task.Execute();

            return NoContent();
        }
    }
}