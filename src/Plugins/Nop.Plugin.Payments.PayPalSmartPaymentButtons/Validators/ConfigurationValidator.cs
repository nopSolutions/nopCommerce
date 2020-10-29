using FluentValidation;
using Nop.Plugin.Payments.PayPalSmartPaymentButtons.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons.Validators
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
                .WithMessage(localizationService.GetResourceAsync("Plugins.Payments.PayPalSmartPaymentButtons.Fields.ClientId.Required").Result)
                .When(model => !model.UseSandbox);

            RuleFor(model => model.SecretKey)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Plugins.Payments.PayPalSmartPaymentButtons.Fields.SecretKey.Required").Result)
                .When(model => !model.UseSandbox);
        }

        #endregion
    }
}