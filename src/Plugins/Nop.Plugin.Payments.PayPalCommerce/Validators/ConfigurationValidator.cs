using FluentValidation;
using Nop.Plugin.Payments.PayPalCommerce.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.PayPalCommerce.Validators
{
    /// <summary>
    /// Represents configuration model validator
    /// </summary>
    public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
    {
        #region Ctor

        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.ClientId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Fields.ClientId.Required"))
                .When(model => !model.UseSandbox && model.SetCredentialsManually);

            RuleFor(model => model.SecretKey)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Fields.SecretKey.Required"))
                .When(model => !model.UseSandbox && model.SetCredentialsManually);
        }

        #endregion
    }
}