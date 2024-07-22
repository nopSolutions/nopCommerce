namespace Nop.Plugin.Payments.AmazonPay.Enums;

/// <summary>
/// Represents payment type enumeration
/// </summary>
public enum PaymentType
{
    /// <summary>
    /// The merchant intends to capture payment immediately after the customer makes a payment.
    /// </summary>
    Capture = 1,

    /// <summary>
    /// The merchant intends to authorize a payment and place funds on hold after the customer makes a payment.
    /// </summary>
    Authorize = 2
}
