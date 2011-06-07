using FluentValidation;
using Nop.Admin.Models.Directory;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Directory
{
    public class StateProvinceValidator : AbstractValidator<StateProvinceModel>
    {
        public StateProvinceValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.Countries.States.Fields.Name.Required"));
        }
    }
}