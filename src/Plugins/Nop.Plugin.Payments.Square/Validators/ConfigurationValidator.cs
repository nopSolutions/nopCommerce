using FluentValidation;
using Nop.Plugin.Payments.Square.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.Square.Validators
{
    /// <summary>
    /// Represents validator of a configuration model
    /// </summary>
    public partial class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
    {
        public ConfigurationValidator(ILocalizationService localizationService)
        {
            //token renewal limit to 45 days max
            RuleFor(x => x.AccessTokenRenewalPeriod).LessThanOrEqualTo(45)
                .WithMessage(localizationService.GetResource("Plugins.Payments.Square.Fields.AccessTokenRenewalPeriod.Max"));
        }
    }
}