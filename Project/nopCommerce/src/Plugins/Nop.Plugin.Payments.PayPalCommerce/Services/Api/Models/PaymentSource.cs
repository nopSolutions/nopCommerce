using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.PaymentSources;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the payment source
/// </summary>
public class PaymentSource
{
    #region Properties

    /// <summary>
    /// Gets or sets the payment card to use to fund a payment. Can be a credit or debit card.
    /// </summary>
    [JsonProperty(PropertyName = "card")]
    public Card Card { get; set; }

    /// <summary>
    /// Gets or sets the tokenized payment source to fund a payment.
    /// </summary>
    [JsonProperty(PropertyName = "token")]
    public Token Token { get; set; }

    /// <summary>
    /// Gets or sets the PayPal Wallet payment source. Main use of this selection is to provide additional instructions associated with this choice like vaulting.
    /// </summary>
    [JsonProperty(PropertyName = "paypal")]
    public PayPal PayPal { get; set; }

    /// <summary>
    /// Gets or sets the information needed to indicate that Venmo is being used to fund the payment.
    /// </summary>
    [JsonProperty(PropertyName = "venmo")]
    public Venmo Venmo { get; set; }

    /// <summary>
    /// Gets or sets the Apple Pay info.
    /// </summary>
    [JsonProperty(PropertyName = "applepay")]
    public ApplePay ApplePay { get; set; }

    /// <summary>
    /// Gets the instruction to vault the payment source based on the specified strategy.
    /// </summary>
    [JsonIgnore]
    public Vault Vault => PayPal?.Attributes?.Vault ?? Card?.Attributes?.Vault ?? Venmo?.Attributes?.Vault;

    #endregion
}