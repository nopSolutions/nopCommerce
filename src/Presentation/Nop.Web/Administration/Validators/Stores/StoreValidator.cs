using FluentValidation;
using Nop.Admin.Models.Stores;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Stores
{
    public class StoreValidator : AbstractValidator<StoreModel>
    {
        public StoreValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.Stores.Fields.Name.Required"));
            RuleFor(x => x.Url)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.Stores.Fields.Url.Required"));
        }
    }
}