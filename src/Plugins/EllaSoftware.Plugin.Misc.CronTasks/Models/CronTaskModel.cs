using System.Collections.Generic;
using EllaSoftware.Plugin.Misc.CronTasks.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace EllaSoftware.Plugin.Misc.CronTasks.Models
{
    public record CronTaskModel : BaseNopModel
    {
        public CronTaskModel()
        {
            AvailableScheduleTasks = new List<SelectListItem>();
        }

        [NopResourceDisplayName("EllaSoftware.Plugin.Misc.CronTasks.CronTask.ScheduleTaskId")]
        public int ScheduleTaskId { get; init; }
        public IList<SelectListItem> AvailableScheduleTasks { get; init; }
        public ScheduleTaskModel ScheduleTaskModel { get; init; }

        [NopResourceDisplayName("EllaSoftware.Plugin.Misc.CronTasks.CronTask.CronExpression")]
        public string CronExpression { get; init; }

        [NopResourceDisplayName("EllaSoftware.Plugin.Misc.CronTasks.CronTask.CronNextOccurrence")]
        public string CronNextOccurrence { get; init; }

        public CronTaskExecutionStatus ExecutionStatus { get; init; }
        public string ExecutionStatusString => ExecutionStatus.ToString();
    }
}
