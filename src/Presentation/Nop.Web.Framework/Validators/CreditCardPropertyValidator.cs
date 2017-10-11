using System;
using System.Linq;
using FluentValidation.Validators;

namespace Nop.Web.Framework.Validators
{
    public class CreditCardPropertyValidator : PropertyValidator
    {
        public CreditCardPropertyValidator()
            : base("Credit card number is not valid")
        {

        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var ccValue = context.PropertyValue as string;
            if (String.IsNullOrWhiteSpace(ccValue))
                return false;

            ccValue = ccValue.Replace(" ", "");
            ccValue = ccValue.Replace("-", "");

            var checksum = 0;
            var evenDigit = false;

            //http://www.beachnet.com/~hstiles/cardtype.html
            foreach (var digit in ccValue.Reverse())
            {
                if (!Char.IsDigit(digit))
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
