using System;
using FluentValidation;
using Nop.Plugin.Payments.Square.Models;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.Square.Validators
{
    public partial class ConfigurationModelValidator : BaseNopValidator<ConfigurationModel>
    {
        public ConfigurationModelValidator()
        {
            RuleFor(x => x.AccessToken).Must((x, context) =>
            {
                if (!x.UseSandbox)
                    return true;

                return !string.IsNullOrEmpty(x.AccessToken) && x.AccessToken.StartsWith("sandbox-", StringComparison.InvariantCultureIgnoreCase);
            }).WithMessage("Sandbox access token should start with 'sandbox-'");
        }
    }
}