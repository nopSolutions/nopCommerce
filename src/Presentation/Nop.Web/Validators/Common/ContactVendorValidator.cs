using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Common;

namespace Nop.Web.Validators.Common
{
    public class ContactVendorValidator : BaseNopValidator<ContactVendorModel>
    {
        public ContactVendorValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("ContactVendor.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
            RuleFor(x => x.FullName).NotEmpty().WithMessage(localizationService.GetResource("ContactVendor.FullName.Required"));
            RuleFor(x => x.Enquiry).NotEmpty().WithMessage(localizationService.GetResource("ContactVendor.Enquiry.Required"));
        }}
}