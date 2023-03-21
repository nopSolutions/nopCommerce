using FluentValidation;
using Nop.Plugin.Payments.Param.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.Param.Validators
{
    public class PaymentInfoValidator : AbstractValidator<PaymentInfoModel>
    {
        public PaymentInfoValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CardholderName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Payment.CardholderName.Required"));
            RuleFor(x => x.CardNumber).IsCreditCard().WithMessageAwait(localizationService.GetResourceAsync("Payment.CardNumber.Wrong"));
            RuleFor(x => x.CardCode).Matches(@"^[0-9]{3,4}$").WithMessageAwait(localizationService.GetResourceAsync("Payment.CardCode.Wrong"));
        }
    }
}