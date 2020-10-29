using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Vendors;

namespace Nop.Web.Validators.Vendors
{
    public partial class VendorInfoValidator : BaseNopValidator<VendorInfoModel>
    {
        public VendorInfoValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.VendorInfo.Name.Required").Result);

            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.VendorInfo.Email.Required").Result);
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResourceAsync("Common.WrongEmail").Result);
        }
    }
}