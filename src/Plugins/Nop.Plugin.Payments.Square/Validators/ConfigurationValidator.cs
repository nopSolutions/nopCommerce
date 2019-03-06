using System;
using FluentValidation;
using Nop.Plugin.Payments.Square.Models;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.Square.Validators
{
    /// <summary>
    /// Represents validator of the Square payment plugin configuration model
    /// </summary>
    public partial class ConfigurationModelValidator : BaseNopValidator<ConfigurationModel>
    {
        public ConfigurationModelValidator()
        {
            //rules for sandbox credentials
            RuleFor(model => model.SandboxAccessToken).Must((model, context) =>
            {
                //do not validate for production credentials
                if (!model.UseSandbox)
                    return true;

                //password type input is not populated from the model
                if (model.SandboxAccessToken == null)
                    return true;

                return model.SandboxAccessToken.StartsWith(SquarePaymentDefaults.SandboxCredentialsPrefix, StringComparison.InvariantCultureIgnoreCase);
            }).WithMessage($"Sandbox access token should start with '{SquarePaymentDefaults.SandboxCredentialsPrefix}'");

            RuleFor(model => model.SandboxApplicationId).Must((model, context) =>
            {
                //do not validate for production credentials
                if (!model.UseSandbox)
                    return true;

                return !string.IsNullOrEmpty(model.SandboxApplicationId) &&
                    model.SandboxApplicationId.StartsWith(SquarePaymentDefaults.SandboxCredentialsPrefix, StringComparison.InvariantCultureIgnoreCase);
            }).WithMessage($"Sandbox application ID should start with '{SquarePaymentDefaults.SandboxCredentialsPrefix}'");
        }
    }
}