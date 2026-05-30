using FluentValidation;
using Nop.Core.Domain.Gdpr;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Settings;

public partial class GdprConsentValidator : BaseNopValidator<GdprConsentModel>
{
    public GdprConsentValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Message).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.Gdpr.Consent.Message.Required"));
        RuleFor(x => x.RequiredMessage)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.Gdpr.Consent.RequiredMessage.Required"))
            .When(x => x.IsRequired);

        SetDatabaseValidationRules<GdprConsent>();
    }
}