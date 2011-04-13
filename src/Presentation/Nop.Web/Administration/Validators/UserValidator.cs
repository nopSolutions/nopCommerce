using FluentValidation;
using Nop.Admin.Models;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class UserValidator : AbstractValidator<UserModel>
    {
        public UserValidator(ILocalizationService localizationService)
        {
            //UNDONE add requred validation
        }
    }
}