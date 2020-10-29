using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;

namespace Nop.Web.Validators.Customer
{
    public partial class PasswordRecoveryValidator : BaseNopValidator<PasswordRecoveryModel>
    {
        public PasswordRecoveryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.PasswordRecovery.Email.Required").Result);
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResourceAsync("Common.WrongEmail").Result);
        }
    }
}