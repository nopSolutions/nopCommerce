using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;
using Nop.Web.Models.Home;

namespace Nop.Web.Validators.Customer
{
    public class PasswordRecoveryValidator : AbstractValidator<PasswordRecoveryModel>
    {
        public PasswordRecoveryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.PasswordRecovery.Email.Required"));
            RuleFor(x => x.Email).EmailAddress(); //TODO localize
        }}
}