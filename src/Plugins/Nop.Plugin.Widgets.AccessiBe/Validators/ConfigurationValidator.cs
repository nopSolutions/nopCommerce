using FluentValidation;
using Nop.Plugin.Widgets.AccessiBe.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Widgets.AccessiBe.Validators
{
    /// <summary>
    /// Represents configuration model validator
    /// </summary>
    public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
    {
        #region Ctor

        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.Script)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.AccessiBe.Fields.Script.Required"))
                .When(model => model.Enabled);
        }

        #endregion
    }
}