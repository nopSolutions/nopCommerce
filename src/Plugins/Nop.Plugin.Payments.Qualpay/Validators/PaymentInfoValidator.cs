using System;
using FluentValidation;
using Nop.Plugin.Payments.Qualpay.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.Qualpay.Validators
{
    /// <summary>
    /// Represents payment info model validator
    /// </summary>
    public class PaymentInfoValidator : BaseNopValidator<PaymentInfoModel>
    {
        #region Ctor

        public PaymentInfoValidator(ILocalizationService localizationService)
        {
            //set validation rules
            RuleFor(model => model.CardholderName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Payment.CardholderName.Required"))
                .When(model => string.IsNullOrEmpty(model.BillingCardId) || model.BillingCardId.Equals(Guid.Empty.ToString()));

            RuleFor(model => model.CardNumber)
                .IsCreditCard()
                .WithMessage(localizationService.GetResource("Payment.CardNumber.Wrong"))
                .When(model => string.IsNullOrEmpty(model.BillingCardId) || model.BillingCardId.Equals(Guid.Empty.ToString()));

            RuleFor(model => model.CardCode)
                .Matches(@"^[0-9]{3,4}$")
                .WithMessage(localizationService.GetResource("Payment.CardCode.Wrong"))
                .When(model => string.IsNullOrEmpty(model.BillingCardId) || model.BillingCardId.Equals(Guid.Empty.ToString()));

            RuleFor(model => model.ExpireMonth)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Payment.ExpireMonth.Required"))
                .When(model => string.IsNullOrEmpty(model.BillingCardId) || model.BillingCardId.Equals(Guid.Empty.ToString()));

            RuleFor(model => model.ExpireYear)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Payment.ExpireYear.Required"))
                .When(model => string.IsNullOrEmpty(model.BillingCardId) || model.BillingCardId.Equals(Guid.Empty.ToString()));
        }

        #endregion
    }
}