using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation.Validators;
using Nop.Core.Domain.Customers;

namespace Nop.Web.Framework.Validators
{
    /// <summary>
    /// Phohe number validator
    /// </summary>
    public class PhoneNumberPropertyValidator : PropertyValidator
    {
        private readonly CustomerSettings _customerSettings;

        /// <summary>
        /// Ctor
        /// </summary>
        public PhoneNumberPropertyValidator(CustomerSettings customerSettings)
            : base("Phone number is not valid")
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
        /// <param name="phoneNumber">Phone number</param>
        /// <param name="customerSettings">Customer settings</param>
        /// <returns>Result</returns>
        public static bool IsValid(string phoneNumber, CustomerSettings customerSettings)
        {
            if (!customerSettings.PhoneNumberValidationEnabled || string.IsNullOrEmpty(customerSettings.PhoneNumberValidationRule))
                return true;

            if (string.IsNullOrEmpty(phoneNumber))
                return false;

            return customerSettings.PhoneNumberValidationUseRegex
                ? Regex.IsMatch(phoneNumber, customerSettings.PhoneNumberValidationRule, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
                : phoneNumber.All(l => customerSettings.PhoneNumberValidationRule.Contains(l));
        }
    }
}
