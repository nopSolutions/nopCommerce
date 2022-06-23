using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using Nop.Core.Domain.Customers;

namespace Nop.Web.Framework.Validators
{
    /// <summary>
    /// Password validator
    /// </summary>
    public class IsPasswordPropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>
    {
        private readonly CustomerSettings _customerSettings;

        public override string Name => "PasswordPropertyValidator";

        /// <summary>
        /// Ctor
        /// </summary>
        public IsPasswordPropertyValidator(CustomerSettings customerSettings)
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
        /// <param name="password">Password</param>
        /// <param name="customerSettings">Customer settings</param>
        /// <returns>Result</returns>
        public static bool IsValid(string password, CustomerSettings customerSettings)
        {
            if (!customerSettings.PasswordValidationEnabled || string.IsNullOrEmpty(customerSettings.UsernameValidationRule))
                return true;

            if (string.IsNullOrEmpty(password))
                return false;

            return customerSettings.PasswordValidationUseRegex
                ? Regex.IsMatch(password, customerSettings.UsernameValidationRule, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
                : password.All(l => customerSettings.UsernameValidationRule.Contains(l));
        }

        protected override string GetDefaultMessageTemplate(string errorCode) => "Password is not valid";
    }
}
