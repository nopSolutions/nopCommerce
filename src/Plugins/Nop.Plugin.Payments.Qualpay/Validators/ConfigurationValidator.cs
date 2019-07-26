using FluentValidation;
using Nop.Plugin.Payments.Qualpay.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.Qualpay.Validators
{
    /// <summary>
    /// Represents configuration model validator
    /// </summary>
    public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
    {
        #region Ctor

        public ConfigurationValidator(ILocalizationService localizationService)
        {
            //set validation rules
            RuleFor(model => model.MerchantId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Payments.Qualpay.Fields.MerchantId.Required"))
                .When(model => !string.IsNullOrEmpty(model.SecurityKey));

            RuleFor(model => model.SecurityKey)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Payments.Qualpay.Fields.SecurityKey.Required"))
                .When(model => !string.IsNullOrEmpty(model.MerchantId));

            RuleFor(model => model.ProfileId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.Payments.Qualpay.Fields.ProfileId.Required"))
                .When(model => model.UseRecurringBilling);
        }

        #endregion
    }
}