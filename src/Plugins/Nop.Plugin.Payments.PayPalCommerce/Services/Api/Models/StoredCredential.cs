using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the additional details to process a payment using a card that has been stored
/// </summary>
public class StoredCredential
{
    #region Properties

    /// <summary>
    /// Gets or sets the person or party who initiated or triggered the payment.
    /// </summary>
    [JsonProperty(PropertyName = "payment_initiator")]
    public string PaymentInitiator { get; set; }

    /// <summary>
    /// Gets or sets the type of the stored payment_source payment.
    /// </summary>
    [JsonProperty(PropertyName = "payment_type")]
    public string PaymentType { get; set; }

    /// <summary>
    /// Gets or sets the value if this is a first or subsequent payment using a stored payment source (also referred to as stored credential or card on file).
    /// </summary>
    [JsonProperty(PropertyName = "usage")]
    public string Usage { get; set; }

    /// <summary>
    /// Gets or sets the reference values used by the card network to identify a transaction.
    /// </summary>
    [JsonProperty(PropertyName = "previous_network_transaction_reference")]
    public NetworkTransactionReference PreviousNetworkTransactionReference { get; set; }

    #endregion
}