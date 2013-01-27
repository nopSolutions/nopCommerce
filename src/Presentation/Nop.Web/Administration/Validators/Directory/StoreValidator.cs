using FluentValidation;
using Nop.Admin.Models.Directory;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Directory
{
    public class StoreValidator : AbstractValidator<StoreModel>
    {
        public StoreValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.Stores.Fields.Name.Required"));
        }
    }
}