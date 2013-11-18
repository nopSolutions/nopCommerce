using FluentValidation;
using Nop.Admin.Models.Templates;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Templates
{
    public class CategoryTemplateValidator : AbstractValidator<CategoryTemplateModel>
    {
        public CategoryTemplateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.System.Templates.Category.Name.Required"));
            RuleFor(x => x.ViewPath).NotNull().WithMessage(localizationService.GetResource("Admin.System.Templates.Category.ViewPath.Required"));
        }
    }
}