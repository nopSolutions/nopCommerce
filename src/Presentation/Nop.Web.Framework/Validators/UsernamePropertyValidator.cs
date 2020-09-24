using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation.Validators;
using Nop.Core.Domain.Customers;

namespace Nop.Web.Framework.Validators
{
    /// <summary>
    /// Username validator
    /// </summary>
    public class UsernamePropertyValidator : PropertyValidator
    {
        private readonly CustomerSettings _customerSettings;

        /// <summary>
        /// Ctor
        /// </summary>
        public UsernamePropertyValidator(CustomerSettings customerSettings)
            : base("Username is not valid")
        {
            _customerSettings = customerSettings;
        }

        /// <summary>
        /// Is valid?
        /// </summary>
        /// <param name="context">Validation context</param>
        /// <returns>Result</returns>
        protected override bool IsValid(PropertyValidatorContext context)
        {
            return IsValid(context.PropertyValue as string, _customerSettings);
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
    }
}
