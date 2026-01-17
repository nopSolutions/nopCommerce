using FluentValidation;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Messages;

public partial class QueuedEmailValidator : BaseNopValidator<QueuedEmailModel>
{
    public QueuedEmailValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.From).NotEmpty().WithMessage("Admin.System.QueuedEmails.Fields.From.Required");
        RuleFor(x => x.To).NotEmpty().WithMessage("Admin.System.QueuedEmails.Fields.To.Required");

        RuleFor(x => x.SentTries).NotNull().WithMessage("Admin.System.QueuedEmails.Fields.SentTries.Required")
            .InclusiveBetween(0, 99999).WithMessage("Admin.System.QueuedEmails.Fields.SentTries.Range");

        SetDatabaseValidationRules<QueuedEmail>();

    }
}