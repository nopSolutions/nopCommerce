using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;

namespace Nop.Web.Validators.Customer;

public partial class LoginValidator : BaseNopValidator<LoginModel>
{
    public LoginValidator(ILocalizationService localizationService, CustomerSettings customerSettings, OtpSettings otpSettings)
    {
        if (!customerSettings.UsernamesEnabled)
        {
            //login by email
            RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Login.Fields.Email.Required"))
                .When(x => !x.LoginByPhoneEnabled);
            RuleFor(x => x.Email)
                .IsEmailAddress()
                .WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"))
                .When(x => !x.LoginByPhoneEnabled);
        }

        if (otpSettings.LoginByPhoneEnabled)
        {
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Phone.Required"))
                .When(x => x.LoginByPhoneEnabled);
            RuleFor(x => x.Phone)
                .IsPhoneNumber(customerSettings).WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Phone.NotValid"))
                .When(x => x.LoginByPhoneEnabled);
        }
    }
}