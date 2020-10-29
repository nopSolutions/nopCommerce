using FluentValidation;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Messages
{
    public partial class QueuedEmailValidator : BaseNopValidator<QueuedEmailModel>
    {
        public QueuedEmailValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.From).NotEmpty().WithMessage(localizationService.GetResourceAsync("Admin.System.QueuedEmails.Fields.From.Required").Result);
            RuleFor(x => x.To).NotEmpty().WithMessage(localizationService.GetResourceAsync("Admin.System.QueuedEmails.Fields.To.Required").Result);

            RuleFor(x => x.SentTries).NotNull().WithMessage(localizationService.GetResourceAsync("Admin.System.QueuedEmails.Fields.SentTries.Required").Result)
                                    .InclusiveBetween(0, 99999).WithMessage(localizationService.GetResourceAsync("Admin.System.QueuedEmails.Fields.SentTries.Range").Result);

            SetDatabaseValidationRules<QueuedEmail>(dataProvider);

        }
    }
}