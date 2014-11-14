using FluentValidation;
using Nop.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Catalog
{
    public class ProductAttributeValueModelValidator : BaseNopValidator<ProductModel.ProductAttributeValueModel>
    {
        public ProductAttributeValueModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Name.Required"));

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(1)
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Quantity.GreaterThanOrEqualTo1"))
                .When(x => x.AttributeValueTypeId == (int)AttributeValueType.AssociatedToProduct);
        }
    }
}