using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace EllaSoftware.Plugin.Misc.CronTasks.Models
{
    public record CronTasksSettingsModel : BaseNopModel
    {
        public CronTasksSettingsModel()
        {
            CronTaskSearchModel = new CronTaskSearchModel();
        }

        public int ActiveStoreScopeConfiguration { get; init; }

        public CronTaskSearchModel CronTaskSearchModel { get; init; }
    }
}
