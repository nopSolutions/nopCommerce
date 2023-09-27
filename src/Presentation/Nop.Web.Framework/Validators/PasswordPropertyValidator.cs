using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Validators
{
    public class PasswordPropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>
    {
        #region Fields

        protected readonly CustomerSettings _customerSettings;
        protected readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public PasswordPropertyValidator(ILocalizationService localizationService, CustomerSettings customerSettings)
        {
            _localizationService = localizationService;
            _customerSettings = customerSettings;
        }

        #endregion

        /// <summary>
        /// Validates a specific property value
        /// </summary>
        /// <param name="context">The validation context. The parent object can be obtained from here</param>
        /// <param name="value">The current property value to validate</param>
        /// <returns>True if valid, otherwise false</returns>
        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            var password = value as string;

            if (string.IsNullOrEmpty(password))
            {
                context.AddFailure(_localizationService.GetResourceAsync("Validation.Password.IsNotEmpty").Result);
                return false;
            }

            var regExp = "^";
            //Passwords must be at least X characters and contain the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*-)
            regExp += _customerSettings.PasswordRequireUppercase ? "(?=.*?[A-Z])" : "";
            regExp += _customerSettings.PasswordRequireLowercase ? "(?=.*?[a-z])" : "";
            regExp += _customerSettings.PasswordRequireDigit ? "(?=.*?[0-9])" : "";
            regExp += _customerSettings.PasswordRequireNonAlphanumeric ? "(?=.*?[#?!@$%^&*-])" : "";
            regExp += $".{{{_customerSettings.PasswordMinLength},{_customerSettings.PasswordMaxLength}}}$";

            if (!Regex.IsMatch(password, regExp))
            {
                var regExpError = string.Format(_localizationService.GetResourceAsync("Validation.Password.Rule").Result,
                    string.Format(_localizationService.GetResourceAsync("Validation.Password.LengthValidation").Result, _customerSettings.PasswordMinLength, _customerSettings.PasswordMaxLength),
                    _customerSettings.PasswordRequireUppercase ? _localizationService.GetResourceAsync("Validation.Password.RequireUppercase").Result : "",
                    _customerSettings.PasswordRequireLowercase ? _localizationService.GetResourceAsync("Validation.Password.RequireLowercase").Result : "",
                    _customerSettings.PasswordRequireDigit ? _localizationService.GetResourceAsync("Validation.Password.RequireDigit").Result : "",
                    _customerSettings.PasswordRequireNonAlphanumeric ? _localizationService.GetResourceAsync("Validation.Password.RequireNonAlphanumeric").Result : "");

                context.AddFailure(regExpError);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the default error message template for this validator, when not overridden
        /// </summary>
        /// <param name="errorCode">The currently configured error code for the validator</param>
        protected override string GetDefaultMessageTemplate(string errorCode) => "Password is not valid";

        #region Properties

        public override string Name => "PasswordPropertyValidator";

        #endregion
    }
}