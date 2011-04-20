using FluentValidation;
using Nop.Admin.Models;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class QueuedEmailValidator : AbstractValidator<QueuedEmailModel>
    {
        public QueuedEmailValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.From).NotNull().WithMessage(localizationService.GetResource("Admin.System.QueuedEmails.Fields.From.Required"));
            RuleFor(x => x.To).NotNull().WithMessage(localizationService.GetResource("Admin.System.QueuedEmails.Fields.To.Required"));
        }
    }
}