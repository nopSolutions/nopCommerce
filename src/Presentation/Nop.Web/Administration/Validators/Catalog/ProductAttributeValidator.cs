using FluentValidation;
using Nop.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Catalog
{
    public partial class ProductAttributeValidator : BaseNopValidator<ProductAttributeModel>
    {
        public ProductAttributeValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Attributes.ProductAttributes.Fields.Name.Required"));
            SetStringPropertiesMaxLength<ProductAttribute>(dbContext);
        }
    }
}