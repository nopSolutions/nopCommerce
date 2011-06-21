using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Models.PrivateMessages;

namespace Nop.Web.Validators.PrivateMessages
{
    public class PrivateMessageValidator : AbstractValidator<PrivateMessageModel>
    {
        public PrivateMessageValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Subject).NotEmpty().WithMessage(localizationService.GetResource("PrivateMessages.SubjectCannotBeEmpty"));
            RuleFor(x => x.Message).NotEmpty().WithMessage(localizationService.GetResource("PrivateMessages.MessageCannotBeEmpty"));
        }
    }
}