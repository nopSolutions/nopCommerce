using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;

namespace Nop.Web.Validators.Customer;

public partial class PasswordRecoveryValidator : BaseNopValidator<PasswordRecoveryModel>
{
    public PasswordRecoveryValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Account.PasswordRecovery.Email.Required");
        RuleFor(x => x.Email)
            .IsEmailAddress()
            .WithMessage("Common.WrongEmail");
    }
}