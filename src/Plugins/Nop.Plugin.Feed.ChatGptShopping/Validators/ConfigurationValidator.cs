using FluentValidation;
using Nop.Plugin.Feed.ChatGptShopping.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Feed.ChatGptShopping.Validators;

/// <summary>
/// Represents configuration model validator
/// </summary>
public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
{
    #region Ctor

    public ConfigurationValidator(ILocalizationService localizationService)
    {
        RuleFor(model => model.AutoSyncPeriod)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Feed.ChatGptShopping.Configuration.AutoSyncPeriod.Invalid"))
            .When(model => model.AutoSyncEnabled);
    }

    #endregion
}
