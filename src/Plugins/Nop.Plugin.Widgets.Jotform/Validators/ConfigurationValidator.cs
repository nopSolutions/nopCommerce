using FluentValidation;
using Nop.Plugin.Widgets.Jotform.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Widgets.Jotform.Validators;

/// <summary>
/// Represents configuration model validator
/// </summary>
public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
{
    #region Ctor

    public ConfigurationValidator(ILocalizationService localizationService)
    {
        RuleFor(model => model.EmbedCode)
            .NotEmpty()
            .When(model => model.Enabled)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.Jotform.EmbedCode.Required"));
    }

    #endregion
}