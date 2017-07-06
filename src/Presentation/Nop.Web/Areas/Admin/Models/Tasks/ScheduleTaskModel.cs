using FluentValidation.Attributes;
using Nop.Web.Areas.Admin.Validators.Tasks;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Tasks
{
    [Validator(typeof(ScheduleTaskValidator))]
    public partial class ScheduleTaskModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.System.ScheduleTasks.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.System.ScheduleTasks.Seconds")]
        public int Seconds { get; set; }

        [NopResourceDisplayName("Admin.System.ScheduleTasks.Enabled")]
        public bool Enabled { get; set; }

        [NopResourceDisplayName("Admin.System.ScheduleTasks.StopOnError")]
        public bool StopOnError { get; set; }

        [NopResourceDisplayName("Admin.System.ScheduleTasks.LastStart")]
        public string LastStartUtc { get; set; }

        [NopResourceDisplayName("Admin.System.ScheduleTasks.LastEnd")]
        public string LastEndUtc { get; set; }

        [NopResourceDisplayName("Admin.System.ScheduleTasks.LastSuccess")]
        public string LastSuccessUtc { get; set; }
    }
}