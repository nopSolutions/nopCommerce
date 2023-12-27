using FluentValidation;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Messages;

public partial class EmailAccountValidator : BaseNopValidator<EmailAccountModel>
{
    public EmailAccountValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Email).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"));

        RuleFor(x => x.DisplayName).NotEmpty();
        RuleFor(x => x.MaxNumberOfEmails).NotEmpty().GreaterThan(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts.Fields.MaxNumberOfEmails.ShouldBeGreaterThanZero"));

        SetDatabaseValidationRules<EmailAccount>();
    }
}