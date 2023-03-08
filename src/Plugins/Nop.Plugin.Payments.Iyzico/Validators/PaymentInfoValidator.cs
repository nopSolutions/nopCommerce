using System;
using FluentValidation;
using Nop.Plugin.Payments.Iyzico.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.Iyzico.Validators
{
    public partial class PaymentInfoValidator : BaseNopValidator<PaymentInfoModel>
    {
        public PaymentInfoValidator(ILocalizationService localizationService)
        {
            this.RuleFor(x => x.CardholderName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Payment.CardholderName.Required"));
            this.RuleFor(x => x.CardNumber).IsCreditCard().WithMessageAwait(localizationService.GetResourceAsync("Payment.CardNumber.Wrong"));
            this.RuleFor(x => x.CardCode).Matches(@"^[0-9]{3,4}$").WithMessageAwait(localizationService.GetResourceAsync("Payment.CardCode.Wrong"));
            this.RuleFor(x => x.ExpireMonth).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Payment.ExpireMonth.Required"));
            this.RuleFor(x => x.ExpireYear).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Payment.ExpireYear.Required"));
            this.RuleFor(x => x.ExpireMonth).Must((x, context) =>
            {
                //not specified yet
                if (string.IsNullOrEmpty(x.ExpireYear) || string.IsNullOrEmpty(x.ExpireMonth))
                    return true;

                //the cards remain valid until the last calendar day of that month
                //If, for example, an expiration date reads 06/15, this means it can be used until midnight on June 30, 2015
                var enteredDate = new DateTime(int.Parse(x.ExpireYear), int.Parse(x.ExpireMonth), 1).AddMonths(1);

                if (enteredDate < DateTime.Now)
                    return false;

                return true;
            }).WithMessageAwait(localizationService.GetResourceAsync("Payment.ExpirationDate.Expired"));
        }
    }
}