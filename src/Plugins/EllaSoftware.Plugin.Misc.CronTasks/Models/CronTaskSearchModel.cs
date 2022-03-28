using Nop.Web.Framework.Models;

namespace EllaSoftware.Plugin.Misc.CronTasks.Models
{
    public record CronTaskSearchModel : BaseSearchModel
    {
        public CronTaskSearchModel()
        {
            AddCronTaskModel = new CronTaskModel();
        }

        public CronTaskModel AddCronTaskModel { get; set; }
    }
}
