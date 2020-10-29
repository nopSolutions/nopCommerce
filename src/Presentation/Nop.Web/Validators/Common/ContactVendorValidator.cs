using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Common;

namespace Nop.Web.Validators.Common
{
    public partial class ContactVendorValidator : BaseNopValidator<ContactVendorModel>
    {
        public ContactVendorValidator(ILocalizationService localizationService, CommonSettings commonSettings)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResourceAsync("ContactVendor.Email.Required").Result);
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResourceAsync("Common.WrongEmail").Result);
            RuleFor(x => x.FullName).NotEmpty().WithMessage(localizationService.GetResourceAsync("ContactVendor.FullName.Required").Result);
            if (commonSettings.SubjectFieldOnContactUsForm)
            {
                RuleFor(x => x.Subject).NotEmpty().WithMessage(localizationService.GetResourceAsync("ContactVendor.Subject.Required").Result);
            }
            RuleFor(x => x.Enquiry).NotEmpty().WithMessage(localizationService.GetResourceAsync("ContactVendor.Enquiry.Required").Result);
        }
    }
}