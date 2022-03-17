using FluentValidation;
using Nop.Plugin.Shipping.EasyPost.Models.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Shipping.EasyPost.Validators
{
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
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Configuration.Fields.ApiKey.Required"))
                .When(model => !model.UseSandbox);
        }

        #endregion
    }
}