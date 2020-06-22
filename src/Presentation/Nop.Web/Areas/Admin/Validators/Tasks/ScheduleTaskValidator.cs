using FluentValidation;
using Nop.Core.Domain.Tasks;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Tasks;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Tasks
{
    public partial class ScheduleTaskValidator : BaseNopValidator<ScheduleTaskModel>
    {
        public ScheduleTaskValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.System.ScheduleTasks.Name.Required"));
            RuleFor(x => x.Seconds).GreaterThan(0).WithMessage(localizationService.GetResource("Admin.System.ScheduleTasks.Seconds.Positive"));

            SetDatabaseValidationRules<ScheduleTask>(dataProvider);
        }
    }
}