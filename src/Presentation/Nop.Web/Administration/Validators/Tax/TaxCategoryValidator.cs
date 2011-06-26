using FluentValidation;
using Nop.Admin.Models.Tax;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Tax
{
    public class TaxCategoryValidator : AbstractValidator<TaxCategoryModel>
    {
        public TaxCategoryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.Configuration.Tax.Categories.Fields.Name.Required"));
        }
    }
}