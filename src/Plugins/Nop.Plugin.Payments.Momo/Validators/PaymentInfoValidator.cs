using FluentValidation;
using Nop.Plugin.Payments.Momo.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.Momo.Validators;

public class PaymentInfoValidator : BaseNopValidator<PaymentInfoModel>
{
    public PaymentInfoValidator(ILocalizationService localizationService)
    {
        //useful links:
        //http://fluentvalidation.codeplex.com/wikipage?title=Custom&referringTitle=Documentation&ANCHOR#CustomValidator
        //http://benjii.me/2010/11/credit-card-validator-attribute-for-asp-net-mvc-3/

        // RuleFor(x => x.PhoneNumber).IsPhoneNumber().WithMessageAwait(localizationService.GetResourceAsync("Payment.PhoneNumber.Wrong"));
    }
}