using System;
using FluentValidation.Validators;

namespace Nop.Web.Framework.Validators
{
    /// <summary>
    /// Decimal validator
    /// </summary>
    public class DecimalPropertyValidator : PropertyValidator
    {
        private readonly decimal _maxValue;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="maxValue">Maximum value</param>
        public DecimalPropertyValidator(decimal maxValue)
        {
            _maxValue = maxValue;
        }

        protected override string GetDefaultMessageTemplate() => "Decimal value is out of range";

        /// <summary>
        /// Is valid?
        /// </summary>
        /// <param name="context">Validation context</param>
        /// <returns>Result</returns>
        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (decimal.TryParse(context.PropertyValue.ToString(), out decimal value))
                return Math.Round(value, 3) < _maxValue;

            return false;
        }
    }
}