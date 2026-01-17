using FluentValidation;
using Nop.Core.Domain.Vendors;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Vendors;

public partial class VendorValidator : BaseNopValidator<VendorModel>
{
    public VendorValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Admin.Vendors.Fields.Name.Required");

        RuleFor(x => x.Email).NotEmpty().WithMessage("Admin.Vendors.Fields.Email.Required");
        RuleFor(x => x.Email)
            .IsEmailAddress()
            .WithMessage("Admin.Common.WrongEmail");
        RuleFor(x => x.PageSizeOptions).Must(ValidatorUtilities.PageSizeOptionsValidator).WithMessage("Admin.Vendors.Fields.PageSizeOptions.ShouldHaveUniqueItems");
        RuleFor(x => x.PageSize).Must((x, context) =>
        {
            if (!x.AllowCustomersToSelectPageSize && x.PageSize <= 0)
                return false;

            return true;
        }).WithMessage("Admin.Vendors.Fields.PageSize.Positive");
        RuleFor(x => x.SeName).Length(0, NopSeoDefaults.SearchEngineNameLength)
            .WithMessage(string.Format("Admin.SEO.SeName.MaxLengthValidation", NopSeoDefaults.SearchEngineNameLength));

        RuleFor(x => x.PriceFrom)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Admin.Vendors.Fields.PriceFrom.GreaterThanOrEqualZero")
            .When(x => x.PriceRangeFiltering && x.ManuallyPriceRange);

        RuleFor(x => x.PriceTo)
            .GreaterThan(x => x.PriceFrom > decimal.Zero ? x.PriceFrom : decimal.Zero)
            .WithMessage(x => string.Format(localizationService.GetResourceAsync("Admin.Vendors.Fields.PriceTo.GreaterThanZeroOrPriceFrom").Result, x.PriceFrom > decimal.Zero ? x.PriceFrom : decimal.Zero))
            .When(x => x.PriceRangeFiltering && x.ManuallyPriceRange);

        SetDatabaseValidationRules<Vendor>();
    }
}