using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;
using Nop.Core.Domain.Customers;

namespace Nop.Web.Framework.Validators
{
    /// <summary>
    /// Username validator
    /// </summary>
    public partial class UsernamePropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>
    {
        private readonly CustomerSettings _customerSettings;

        public override string Name => "UsernamePropertyValidator";

        /// <summary>
        /// Ctor
        /// </summary>
        public UsernamePropertyValidator(CustomerSettings customerSettings)
        {
            _customerSettings = customerSettings;
        }

        /// <summary>
        /// Is valid?
        /// </summary>
        /// <param name="context">Validation context</param>
        /// <returns>Result</returns>
        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            return IsValid(value as string, _customerSettings);
        }

        /// <summary>
        /// Is valid?
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="customerSettings">Customer settings</param>
        /// <returns>Result</returns>
        public static bool IsValid(string username, CustomerSettings customerSettings)
        {
            if (!customerSettings.UsernameValidationEnabled || string.IsNullOrEmpty(customerSettings.UsernameValidationRule))
                return true;

            if (string.IsNullOrEmpty(username))
                return false;

            return customerSettings.UsernameValidationUseRegex
                ? Regex.IsMatch(username, customerSettings.UsernameValidationRule, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
                : username.All(l => customerSettings.UsernameValidationRule.Contains(l));
        }

        protected override string GetDefaultMessageTemplate(string errorCode) => "Username is not valid";
    }
}
