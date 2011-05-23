using FluentValidation;
using Nop.Admin.Models;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class StateProvinceValidator : AbstractValidator<StateProvinceModel>
    {
        public StateProvinceValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.Location.Countries.States.Fields.Name.Required"));
        }
    }
}