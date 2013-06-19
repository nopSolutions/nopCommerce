using FluentValidation;
using Nop.Admin.Models.Catalog;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Catalog
{
    public class ProductVariantAttributeValueModelValidator : AbstractValidator<ProductModel.ProductVariantAttributeValueModel>
    {
        public ProductVariantAttributeValueModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.ProductVariantAttributes.Attributes.Values.Fields.Name.Required"));
        }
    }
}