using FluentValidation;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Catalog
{
    public partial class ProductValidator : BaseNopValidator<ProductModel>
    {
        public ProductValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.Name.Required").Result);
            
            RuleFor(x => x.SeName)
                .Length(0, NopSeoDefaults.SearchEngineNameLength)
                .WithMessage(string.Format(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation").Result, NopSeoDefaults.SearchEngineNameLength));
            
            RuleFor(x => x.RentalPriceLength)
                .GreaterThan(0)
                .WithMessage(localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.RentalPriceLength.ShouldBeGreaterThanZero").Result)
                .When(x => x.IsRental);

            SetDatabaseValidationRules<Product>(dataProvider);
        }
    }
}