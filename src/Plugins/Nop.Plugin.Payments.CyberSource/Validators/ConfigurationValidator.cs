using FluentValidation;
using Nop.Plugin.Payments.CyberSource.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.CyberSource.Validators
{
    /// <summary>
    /// Represents configuration model validator
    /// </summary>
    public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
    {
        #region Ctor

        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.MerchantId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.CyberSource.Fields.MerchantId.Required"))
                .When(model => !model.UseSandbox);

            RuleFor(model => model.KeyId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.CyberSource.Fields.KeyId.Required"))
                .When(model => !model.UseSandbox);

            RuleFor(model => model.SecretKey)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.CyberSource.Fields.SecretKey.Required"))
                .When(model => !model.UseSandbox);

            RuleFor(model => model.ConversionDetailReportingFrequency)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.CyberSource.Fields.ConversionDetailReportingFrequency.Invalid"));
        }

        #endregion
    }
}