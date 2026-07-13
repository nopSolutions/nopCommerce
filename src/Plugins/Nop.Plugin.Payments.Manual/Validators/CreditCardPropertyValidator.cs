using FluentValidation;
using FluentValidation.Validators;

namespace Nop.Plugin.Payments.Manual.Validators;

/// <summary>
/// Credit card validator
/// </summary>
public class CreditCardPropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>, ICreditCardValidator
{
    public override string Name => "CreditCardPropertyValidator";

    /// <summary>
    /// Is valid?
    /// </summary>
    /// <param name="context">Validation context</param>
    /// <param name="value">The current property value to validate</param>
    /// <returns>Result</returns>
    public override bool IsValid(ValidationContext<T> context, TProperty value)
    {
        var ccValue = value as string;
        if (string.IsNullOrWhiteSpace(ccValue))
            return false;

        ccValue = ccValue.Replace(" ", "");
        ccValue = ccValue.Replace("-", "");

        var checksum = 0;
        var evenDigit = false;

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

        return checksum % 10 == 0;
    }

    /// <summary>
    /// Returns the default error message template for this validator, when not overridden.
    /// </summary>
    /// <param name="errorCode">The currently configured error code for the validator.</param>
    /// <returns></returns>
    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "Credit card number is not valid";
    }
}