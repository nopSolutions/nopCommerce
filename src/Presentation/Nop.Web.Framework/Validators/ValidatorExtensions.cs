using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Validators
{
    /// <summary>
    /// Validator extensions
    /// </summary>
    public static class ValidatorExtensions
    {
        /// <summary>
        /// Set credit card validator
        /// </summary>
        /// <typeparam name="TModel">Type of model being validated</typeparam>
        /// <param name="ruleBuilder">Rule builder</param>
        /// <returns>Result</returns>
        public static IRuleBuilderOptions<TModel, string> IsCreditCard<TModel>(this IRuleBuilder<TModel, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new CreditCardPropertyValidator());
        }

        /// <summary>
        /// Set decimal validator
        /// </summary>
        /// <typeparam name="TModel">Type of model being validated</typeparam>
        /// <param name="ruleBuilder">Rule builder</param>
        /// <param name="maxValue">Maximum value</param>
        /// <returns>Result</returns>
        public static IRuleBuilderOptions<TModel, decimal> IsDecimal<TModel>(this IRuleBuilder<TModel, decimal> ruleBuilder, decimal maxValue)
        {
            return ruleBuilder.SetValidator(new DecimalPropertyValidator(maxValue));
        }

        /// <summary>
        /// Set username validator
        /// </summary>
        /// <typeparam name="TModel">Type of model being validated</typeparam>
        /// <param name="ruleBuilder">Rule builder</param>
        /// <param name="customerSettings">Customer settings</param>
        /// <returns>Result</returns>
        public static IRuleBuilderOptions<TModel, string> IsUsername<TModel>(this IRuleBuilder<TModel, string> ruleBuilder,
            CustomerSettings customerSettings)
        {
            return ruleBuilder.SetValidator(new UsernamePropertyValidator(customerSettings));
        }

        /// <summary>
        /// Set phone number validator
        /// </summary>
        /// <typeparam name="TModel">Type of model being validated</typeparam>
        /// <param name="ruleBuilder">Rule builder</param>
        /// <param name="customerSettings">Customer settings</param>
        /// <returns>Result</returns>
        public static IRuleBuilderOptions<TModel, string> IsPhoneNumber<TModel>(this IRuleBuilder<TModel, string> ruleBuilder,
            CustomerSettings customerSettings)
        {
            return ruleBuilder.SetValidator(new PhoneNumberPropertyValidator(customerSettings));
        }

        /// <summary>
        /// Implement password validator
        /// </summary>
        /// <typeparam name="TModel">Type of model being validated</typeparam>
        /// <param name="ruleBuilder">Rule builder</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="customerSettings">Customer settings</param>
        /// <returns>Result</returns>
        public static IRuleBuilder<TModel, string> IsPassword<TModel>(this IRuleBuilder<TModel, string> ruleBuilder,
            ILocalizationService localizationService, CustomerSettings customerSettings)
        {
            var regExp = "^";
            //Passwords must be at least X characters and contain the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*-)
            regExp += customerSettings.PasswordRequireUppercase ? "(?=.*?[A-Z])" : "";
            regExp += customerSettings.PasswordRequireLowercase ? "(?=.*?[a-z])" : "";
            regExp += customerSettings.PasswordRequireDigit ? "(?=.*?[0-9])" : "";
            regExp += customerSettings.PasswordRequireNonAlphanumeric ? "(?=.*?[#?!@$%^&*-])" : "";
            regExp += $".{{{customerSettings.PasswordMinLength},}}$";

            var message = string.Format(localizationService.GetResource("Validation.Password.Rule"),
                string.Format(localizationService.GetResource("Validation.Password.LengthValidation"), customerSettings.PasswordMinLength),
                customerSettings.PasswordRequireUppercase ? localizationService.GetResource("Validation.Password.RequireUppercase") : "",
                customerSettings.PasswordRequireLowercase ? localizationService.GetResource("Validation.Password.RequireLowercase") : "",
                customerSettings.PasswordRequireDigit ? localizationService.GetResource("Validation.Password.RequireDigit") : "",
                customerSettings.PasswordRequireNonAlphanumeric ? localizationService.GetResource("Validation.Password.RequireNonAlphanumeric") : "");

            var options = ruleBuilder
                .NotEmpty().WithMessage(localizationService.GetResource("Validation.Password.IsNotEmpty"))
                .Matches(regExp).WithMessage(message);

            return options;
        }
    }
}