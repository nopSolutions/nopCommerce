using FluentValidation;
using Nop.Services.Localization;
using Nop.Plugin.Payments.Manual.Models;

namespace Nop.Plugin.Payments.Manual.Validators
{
    public class PaymentInfoValidator : AbstractValidator<PaymentInfoModel>
    {
        public PaymentInfoValidator(ILocalizationService localizationService)
        {
            //TODO implement and add custom CC validator and use it here
            //useful links:
            //http://fluentvalidation.codeplex.com/wikipage?title=Custom&referringTitle=Documentation&ANCHOR#CustomValidator
            //http://benjii.me/2010/11/credit-card-validator-attribute-for-asp-net-mvc-3/
            RuleFor(x => x.CardholderName).NotEmpty().WithMessage(localizationService.GetResource("Payment.CardholderName.Required"));
            RuleFor(x => x.CardNumber).NotEmpty().WithMessage(localizationService.GetResource("Payment.CardNumber.Required"));
            RuleFor(x => x.CardCode).NotEmpty().WithMessage(localizationService.GetResource("Payment.CardCode.Required"));
            //TODO ensure CardCode is a 3-4 digit integer
        }}
}