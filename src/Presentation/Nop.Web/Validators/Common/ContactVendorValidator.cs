using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Common;

namespace Nop.Web.Validators.Common;

public partial class ContactVendorValidator : BaseNopValidator<ContactVendorModel>
{
    public ContactVendorValidator(ILocalizationService localizationService, CommonSettings commonSettings)
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("ContactVendor.Email.Required");
        RuleFor(x => x.Email)
            .IsEmailAddress()
            .WithMessage("Common.WrongEmail");
        RuleFor(x => x.FullName).NotEmpty().WithMessage("ContactVendor.FullName.Required");
        if (commonSettings.SubjectFieldOnContactUsForm)
        {
            RuleFor(x => x.Subject).NotEmpty().WithMessage("ContactVendor.Subject.Required");
        }
        RuleFor(x => x.Enquiry).NotEmpty().WithMessage("ContactVendor.Enquiry.Required");
    }
}