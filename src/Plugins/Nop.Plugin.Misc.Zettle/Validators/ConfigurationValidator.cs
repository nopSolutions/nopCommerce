using FluentValidation;
using Nop.Plugin.Misc.Zettle.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.Zettle.Validators;

/// <summary>
/// Represents configuration model validator
/// </summary>
public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
{
    #region Ctor

    public ConfigurationValidator(ILocalizationService localizationService)
    {
        RuleFor(model => model.ApiKey)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.Zettle.Configuration.Fields.ApiKey.Required"))
            .When(model => !string.IsNullOrEmpty(model.ClientId));

        RuleFor(model => model.AutoSyncPeriod)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.Zettle.Configuration.Fields.AutoSyncPeriod.Invalid"))
            .When(model => model.AutoSyncEnabled);
    }

    #endregion
}