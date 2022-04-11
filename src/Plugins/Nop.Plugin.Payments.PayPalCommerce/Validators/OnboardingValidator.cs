using FluentValidation;
using Nop.Plugin.Payments.PayPalCommerce.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.PayPalCommerce.Validators
{
    /// <summary>
    /// Represents onboarding model validator
    /// </summary>
    public class OnboardingValidator : BaseNopValidator<OnboardingModel>
    {
        #region Ctor

        public OnboardingValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"));
        }

        #endregion
    }
}