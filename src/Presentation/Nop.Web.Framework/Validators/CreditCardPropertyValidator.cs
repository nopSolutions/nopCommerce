using System.Linq;
using FluentValidation.Validators;

namespace Nop.Web.Framework.Validators
{
    /// <summary>
    /// Credit card validator
    /// </summary>
    public class CreditCardPropertyValidator : PropertyValidator
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public CreditCardPropertyValidator()
            : base("Credit card number is not valid")
        {

        }

        /// <summary>
        /// Is valid?
        /// </summary>
        /// <param name="context">Validation context</param>
        /// <returns>Result</returns>
        protected override bool IsValid(PropertyValidatorContext context)
        {
            var ccValue = context.PropertyValue as string;
            if (string.IsNullOrWhiteSpace(ccValue))
                return false;

            ccValue = ccValue.Replace(" ", "");
            ccValue = ccValue.Replace("-", "");

            var checksum = 0;
            var evenDigit = false;

            //http://www.beachnet.com/~hstiles/cardtype.html
            foreach (var digit in ccValue.Reverse())
            {
                if (!char.IsDigit(digit))
                    return false;

                var digitValue = (digit - '0') * (evenDigit ? 2 : 1);
                evenDigit = !evenDigit;

                while (digitValue > 0)
                {
                    checksum += digitValue % 10;
                    digitValue /= 10;
                }
            }

            return (checksum % 10) == 0;
        }
    }
}
