using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Settings
{
    public partial class CatalogSettingsValidator : BaseNopValidator<CatalogSettingsModel>
    {
        public CatalogSettingsValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SearchPagePriceFrom)
                .GreaterThanOrEqualTo(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.Catalog.SearchPagePriceFrom.GreaterThanOrEqualZero"))
                .When(x => x.SearchPagePriceRangeFiltering && x.SearchPageManuallyPriceRange);

            RuleFor(x => x.SearchPagePriceTo)
                .GreaterThan(x => x.SearchPagePriceFrom > decimal.Zero ? x.SearchPagePriceFrom : decimal.Zero)
                .WithMessage(x => string.Format(localizationService.GetResourceAsync("Admin.Configuration.Settings.Catalog.SearchPagePriceTo.GreaterThanZeroOrPriceFrom").Result, x.SearchPagePriceFrom > decimal.Zero ? x.SearchPagePriceFrom : decimal.Zero))
                .When(x => x.SearchPagePriceRangeFiltering && x.SearchPageManuallyPriceRange);

            RuleFor(x => x.ProductsByTagPriceFrom)
                .GreaterThanOrEqualTo(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.Catalog.ProductsByTagPriceFrom.GreaterThanOrEqualZero"))
                .When(x => x.ProductsByTagPriceRangeFiltering && x.ProductsByTagManuallyPriceRange);

            RuleFor(x => x.ProductsByTagPriceTo)
                .GreaterThan(x => x.ProductsByTagPriceFrom > decimal.Zero ? x.ProductsByTagPriceFrom : decimal.Zero)
                .WithMessage(x => string.Format(localizationService.GetResourceAsync("Admin.Configuration.Settings.Catalog.ProductsByTagPriceTo.GreaterThanZeroOrPriceFrom").Result, x.ProductsByTagPriceFrom > decimal.Zero ? x.ProductsByTagPriceFrom : decimal.Zero))
                .When(x => x.ProductsByTagPriceRangeFiltering && x.ProductsByTagManuallyPriceRange);
        }
    }
}