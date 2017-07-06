using FluentValidation;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Settings
{
    public partial class ReturnRequestReasonValidator : BaseNopValidator<ReturnRequestReasonModel>
    {
        public ReturnRequestReasonValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestReasons.Name.Required"));
        }
    }
}