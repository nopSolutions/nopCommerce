namespace Nop.Plugin.Payments.Manual.Services;

/// <summary>
/// Payments manual helper class
/// </summary>
public class PaymentsManualHelper
{
    /// <summary>
    /// Gets masked credit card number
    /// </summary>
    /// <param name="creditCardNumber">Credit card number</param>
    /// <returns>Masked credit card number</returns>
    public static string GetMaskedCreditCardNumber(string creditCardNumber)
    {
        if (string.IsNullOrEmpty(creditCardNumber))
            return string.Empty;

        if (creditCardNumber.Length <= 4)
            return creditCardNumber;

        var last4 = creditCardNumber[^4..];
        var maskedChars = string.Empty;

        for (var i = 0; i < creditCardNumber.Length - 4; i++)
            maskedChars += "*";

        return maskedChars + last4;
    }
}
