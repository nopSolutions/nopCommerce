using FluentValidation;
using Nop.Admin.Models.Templates;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Templates
{
    public partial class ProductTemplateValidator : BaseNopValidator<ProductTemplateModel>
    {
        public ProductTemplateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.System.Templates.Product.Name.Required"));
            RuleFor(x => x.ViewPath).NotEmpty().WithMessage(localizationService.GetResource("Admin.System.Templates.Product.ViewPath.Required"));
        }
    }
}