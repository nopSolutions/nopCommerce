using FluentValidation;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Vendors
{
    public partial class VendorValidator : BaseNopValidator<VendorModel>
    {
        public VendorValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResourceAsync("Admin.Vendors.Fields.Name.Required").Result);

            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResourceAsync("Admin.Vendors.Fields.Email.Required").Result);
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResourceAsync("Admin.Common.WrongEmail").Result);
            RuleFor(x => x.PageSizeOptions).Must(ValidatorUtilities.PageSizeOptionsValidator).WithMessage(localizationService.GetResourceAsync("Admin.Vendors.Fields.PageSizeOptions.ShouldHaveUniqueItems").Result);
            RuleFor(x => x.PageSize).Must((x, context) =>
            {
                if (!x.AllowCustomersToSelectPageSize && x.PageSize <= 0)
                    return false;

                return true;
            }).WithMessage(localizationService.GetResourceAsync("Admin.Vendors.Fields.PageSize.Positive").Result);
            RuleFor(x => x.SeName).Length(0, NopSeoDefaults.SearchEngineNameLength)
                .WithMessage(string.Format(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation").Result, NopSeoDefaults.SearchEngineNameLength));

            SetDatabaseValidationRules<Vendor>(dataProvider);
        }
    }
}