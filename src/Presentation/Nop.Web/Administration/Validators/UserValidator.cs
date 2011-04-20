using FluentValidation;
using Nop.Admin.Models;
using Nop.Core.Domain.Security;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class UserValidator : AbstractValidator<UserModel>
    {
        public UserValidator(ILocalizationService localizationService)
        {
            //UNDONE store 'UsernamesEnabled' and 'AllowUsersToChangeUsernames' as hidden fields; otherwise, they always be false.
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