using System;
using FluentValidation;
using Nop.Plugin.Payments.CyberSource.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.CyberSource.Validators
{
    /// <summary>
    /// Represents customer token model validator
    /// </summary>
    public class CustomerTokenModelValidator : BaseNopValidator<CustomerTokenModel>
    {
        #region Ctor

        public CustomerTokenModelValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.CardNumber)
                .IsCreditCard()
                .When(model => model.Id == 0)
                .WithMessageAwait(localizationService.GetResourceAsync("Payment.CardNumber.Wrong"));

            RuleFor(model => model.ExpireMonth)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Payment.ExpireMonth.Required"));

            RuleFor(model => model.ExpireYear)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Payment.ExpireYear.Required"));

            RuleFor(model => model.ExpireMonth)
                .Must((model, _) =>
                {
                    //not specified yet
                    if (string.IsNullOrEmpty(model.ExpireYear) || string.IsNullOrEmpty(model.ExpireMonth))
                        return true;

                    //the cards remain valid until the last calendar day of that month
                    //If, for example, an expiration date reads 06/15, this means it can be used until midnight on June 30, 2015
                    var enteredDate = new DateTime(int.Parse(model.ExpireYear), int.Parse(model.ExpireMonth), 1).AddMonths(1);

                    if (enteredDate < DateTime.Now)
                        return false;

                    return true;
                })
                .WithMessageAwait(localizationService.GetResourceAsync("Payment.ExpirationDate.Expired"));
        }

        #endregion
    }
}