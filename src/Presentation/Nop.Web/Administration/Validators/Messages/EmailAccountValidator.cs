using FluentValidation;
using Nop.Admin.Models.Messages;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Messages
{
    public class EmailAccountValidator : AbstractValidator<EmailAccountModel>
    {
        public EmailAccountValidator(ILocalizationService localizationService)
        {
            //UNDONE localize
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Email).EmailAddress();
            
            RuleFor(x => x.DisplayName).NotEmpty();
        }
    }
}