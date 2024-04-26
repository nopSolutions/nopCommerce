using FluentValidation;
using Nop.Plugin.Misc.Omnisend.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.Omnisend.Validators;

/// <summary>
/// Represents configuration model validator
/// </summary>
public class ConfigurationModelValidator : BaseNopValidator<ConfigurationModel>
{
    #region Ctor

    public ConfigurationModelValidator(ILocalizationService localizationService)
    {
        RuleFor(model => model.ApiKey)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.Omnisend.Fields.ApiKey.Required"));
    }

    #endregion
}