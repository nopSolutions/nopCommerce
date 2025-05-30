using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.PaymentSources;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the recurring payment source
/// </summary>
public class RecurringPaymentSource
{
    #region Properties

    /// <summary>
    /// Gets or sets the vaulted PayPal Wallet payment source.
    /// </summary>
    [JsonProperty(PropertyName = "paypal")]
    public VaultPayPal PayPal { get; set; }

    /// <summary>
    /// Gets or sets the Resource representing a request to vault a Card.
    /// </summary>
    [JsonProperty(PropertyName = "card")]
    public Card Card { get; set; }

    /// <summary>
    /// Gets or sets the tokenized Payment Source representing a Request to Vault a Token.
    /// </summary>
    [JsonProperty(PropertyName = "token")]
    public Token Token { get; set; }

    #endregion
}