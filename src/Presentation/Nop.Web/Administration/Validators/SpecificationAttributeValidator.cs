using FluentValidation;
using Nop.Admin.Models;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class SpecificationAttributeValidator : AbstractValidator<SpecificationAttributeModel>
    {
        public SpecificationAttributeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.Catalog.Attributes.SpecificationAttributes.Fields.Name.Required"));
        }
    }
}