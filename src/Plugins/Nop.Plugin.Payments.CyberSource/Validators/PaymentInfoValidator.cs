using FluentValidation;
using Nop.Plugin.Payments.CyberSource.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.CyberSource.Validators
{
    /// <summary>
    /// Represents payment info model validator
    /// </summary>
    public class PaymentInfoValidator : BaseNopValidator<PaymentInfoModel>
    {
        #region Ctor

        public PaymentInfoValidator(ILocalizationService localizationService, CyberSourceSettings cyberSourceSettings)
        {
            if (cyberSourceSettings.TokenizationEnabled)
            {
                RuleFor(model => model.SelectedTokenId)
                    .GreaterThan(0)
                    .When(model => !model.NewCard)
                    .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.CyberSource.Payment.SelectedTokenId.Invalid"));

                RuleFor(model => model.CardNumber)
                    .IsCreditCard()
                    .When(model => model.NewCard && !model.IsFlexMicroFormEnabled)
                    .WithMessageAwait(localizationService.GetResourceAsync("Payment.CardNumber.Wrong"));

                RuleFor(model => model.Cvv)
                    .Matches(@"^[0-9]{3,4}$")
                    .When(model => model.NewCard && (cyberSourceSettings.CvvRequired || !string.IsNullOrEmpty(model.Cvv)))
                    .WithMessageAwait(localizationService.GetResourceAsync("Payment.CardCode.Wrong"));

                RuleFor(model => model.ExpireMonth).NotEmpty()
                    .When(model => model.NewCard)
                    .WithMessageAwait(localizationService.GetResourceAsync("Payment.ExpireMonth.Required"));

                RuleFor(model => model.ExpireYear)
                    .NotEmpty()
                    .When(model => model.NewCard)
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
                    .When(model => model.NewCard)
                    .WithMessageAwait(localizationService.GetResourceAsync("Payment.ExpirationDate.Expired"));
            }
            else
            {
                RuleFor(model => model.CardNumber)
                    .IsCreditCard()
                    .When(model => !model.IsFlexMicroFormEnabled)
                    .WithMessageAwait(localizationService.GetResourceAsync("Payment.CardNumber.Wrong"));

                RuleFor(model => model.Cvv)
                    .Matches(@"^[0-9]{3,4}$")
                    .When(model => model.IsFlexMicroFormEnabled && (cyberSourceSettings.CvvRequired || !string.IsNullOrEmpty(model.Cvv)))
                    .WithMessageAwait(localizationService.GetResourceAsync("Payment.CardCode.Wrong"));

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
        }

        #endregion
    }
}