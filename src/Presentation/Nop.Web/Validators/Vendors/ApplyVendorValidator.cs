using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Vendors;

namespace Nop.Web.Validators.Vendors
{
    public partial class ApplyVendorValidator : BaseNopValidator<ApplyVendorModel>
    {
        public ApplyVendorValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResourceAsync("Vendors.ApplyAccount.Name.Required").Result);

            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResourceAsync("Vendors.ApplyAccount.Email.Required").Result);
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResourceAsync("Common.WrongEmail").Result);
        }
    }
}