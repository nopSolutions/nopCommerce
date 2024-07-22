using FluentValidation;
using Nop.Plugin.Payments.AmazonPay.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.AmazonPay.Validators;

/// <summary>
/// Represents configuration model validator
/// </summary>
public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
{
    #region Ctor

    public ConfigurationValidator(ILocalizationService localizationService)
    {
        RuleFor(model => model.PrivateKey)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.AmazonPay.Settings.PrivateKey.Required"))
            .When(model => model.SetCredentialsManually);

        RuleFor(model => model.PublicKeyId)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.AmazonPay.Settings.PublicKeyId.Required"))
            .When(model => model.SetCredentialsManually);

        RuleFor(model => model.MerchantId)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.AmazonPay.Settings.MerchantId.Required"))
            .When(model => model.SetCredentialsManually);

        RuleFor(model => model.StoreId)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.AmazonPay.Settings.StoreId.Required"))
            .When(model => model.SetCredentialsManually);
    }

    #endregion
}