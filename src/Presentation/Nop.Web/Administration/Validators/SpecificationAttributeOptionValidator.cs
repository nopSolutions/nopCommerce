using FluentValidation;
using Nop.Admin.Models;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class SpecificationAttributeOptionValidator : AbstractValidator<SpecificationAttributeOptionModel>
    {
        public SpecificationAttributeOptionValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.Name.Required"));
        }
    }
}