using FluentValidation;
using Nop.Admin.Models;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class CategoryValidator : AbstractValidator<CategoryModel>
    {
        public CategoryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.Catalog.Categories.Fields.Name.Required"));
        }
    }
}