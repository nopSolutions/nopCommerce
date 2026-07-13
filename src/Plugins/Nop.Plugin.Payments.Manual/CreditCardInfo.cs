namespace Nop.Plugin.Payments.Manual;

/// <summary>
/// Represents credit or debit card info
/// </summary>
public class CreditCardInfo
{
    /// <summary>
    /// Gets or sets the card type
    /// </summary>
    public string CardType { get; set; }

    /// <summary>
    /// Gets or sets the card name
    /// </summary>
    public string CardName { get; set; }

    /// <summary>
    /// Gets or sets the card number
    /// </summary>
    public string CardNumber { get; set; }

    /// <summary>
    /// Gets or sets the masked credit card number
    /// </summary>
    public string MaskedCreditCardNumber { get; set; }

    /// <summary>
    /// Gets or sets the card CVV2
    /// </summary>
    public string CardCvv2 { get; set; }

    /// <summary>
    /// Gets or sets the card expiration month
    /// </summary>
    public string CardExpirationMonth { get; set; }

    /// <summary>
    /// Gets or sets the card expiration year
    /// </summary>
    public string CardExpirationYear { get; set; }
}
