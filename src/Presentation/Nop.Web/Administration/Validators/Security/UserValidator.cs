using FluentValidation;
using Nop.Admin.Models.Security;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Security
{
    public class UserValidator : AbstractValidator<UserModel>
    {
        public UserValidator(ILocalizationService localizationService)
        {
            //we store 'UsernamesEnabled' and 'AllowUsersToChangeUsernames' as hidden fields; otherwise, they always be false.
            RuleFor(x => x.Username)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Users.Fields.Username.Required"))
                .When(x => x.UsernamesEnabled && x.AllowUsersToChangeUsernames);

            RuleFor(x => x.Email)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Users.Fields.Email.Required"));

            RuleFor(x => x.Email)
                .EmailAddress(); //TODO locale email not valid message
        }
    }
}