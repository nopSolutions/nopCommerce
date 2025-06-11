using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Nop.Plugin.Payments.PeachPayments.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.PeachPayments.Validators
{
    public class ConfigurationValidator:BaseNopValidator<ConfigurationModel>
    {
        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.CheckoutChannelSandbox)
               .NotEmpty()
               .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.PeachPayments.Fields.CheckoutChannelSandbox.Required"));
            //.When(model => model.SandBoxModeId.Equals(0));

            //RuleFor(model => model.CheckoutChannelSandbox)
            //  .NotEmpty()
            //  .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.PeachPayments.Fields.CheckoutChannelSandbox.Required"))
            //  .When(model => model.UseSandbox.Equals("Yes"));

            RuleFor(model => model.MerchantIdPrefix)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.PeachPayments.Fields.MerchantIdPrefix.Required"))
                .MaximumLength(10)
                .MinimumLength(8);
        }
    }
}
