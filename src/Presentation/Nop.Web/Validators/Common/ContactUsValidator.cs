using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Common;

namespace Nop.Web.Validators.Common;

public partial class ContactUsValidator : BaseNopValidator<ContactUsModel>
{
    public ContactUsValidator(ILocalizationService localizationService, CommonSettings commonSettings)
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("ContactUs.Email.Required");
        RuleFor(x => x.Email)
            .IsEmailAddress()
            .WithMessage("Common.WrongEmail");
        RuleFor(x => x.FullName).NotEmpty().WithMessage("ContactUs.FullName.Required");
        if (commonSettings.SubjectFieldOnContactUsForm)
        {
            RuleFor(x => x.Subject).NotEmpty().WithMessage("ContactUs.Subject.Required");
        }
        RuleFor(x => x.Enquiry).NotEmpty().WithMessage("ContactUs.Enquiry.Required");
    }
}