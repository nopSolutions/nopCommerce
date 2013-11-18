using FluentValidation;
using Nop.Admin.Models.Templates;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Templates
{
    public class ProductTemplateValidator : AbstractValidator<ProductTemplateModel>
    {
        public ProductTemplateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.System.Templates.Product.Name.Required"));
            RuleFor(x => x.ViewPath).NotNull().WithMessage(localizationService.GetResource("Admin.System.Templates.Product.ViewPath.Required"));
        }
    }
}