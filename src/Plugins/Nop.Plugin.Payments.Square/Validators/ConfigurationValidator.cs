using System;
using FluentValidation;
using Nop.Plugin.Payments.Square.Models;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.Square.Validators
{
    /// <summary>
    /// Represents validator of configuration model
    /// </summary>
    public class ConfigurationModelValidator : BaseNopValidator<ConfigurationModel>
    {
        #region Ctor

        public ConfigurationModelValidator()
        {
            //rules for sandbox credentials
            RuleFor(model => model.SandboxAccessToken)
                .Must((model, name) => model.SandboxAccessToken?.StartsWith(SquarePaymentDefaults.SandboxCredentialsPrefix, StringComparison.InvariantCultureIgnoreCase) ?? false)
                .WithMessage($"Sandbox access token should start with '{SquarePaymentDefaults.SandboxCredentialsPrefix}'")
                .When(model => model.UseSandbox);

            RuleFor(model => model.SandboxApplicationId)
                .Must((model, name) => model.SandboxApplicationId?.StartsWith(SquarePaymentDefaults.SandboxCredentialsPrefix, StringComparison.InvariantCultureIgnoreCase) ?? false)
                .WithMessage($"Sandbox application ID should start with '{SquarePaymentDefaults.SandboxCredentialsPrefix}'")
                .When(model => model.UseSandbox);
        }

        #endregion
    }
}