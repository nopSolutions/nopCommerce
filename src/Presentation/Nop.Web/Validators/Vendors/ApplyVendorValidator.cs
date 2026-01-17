using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Vendors;

namespace Nop.Web.Validators.Vendors;

public partial class ApplyVendorValidator : BaseNopValidator<ApplyVendorModel>
{
    public ApplyVendorValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Vendors.ApplyAccount.Name.Required");

        RuleFor(x => x.Email).NotEmpty().WithMessage("Vendors.ApplyAccount.Email.Required");
        RuleFor(x => x.Email)
            .IsEmailAddress()
            .WithMessage("Common.WrongEmail");
    }
}