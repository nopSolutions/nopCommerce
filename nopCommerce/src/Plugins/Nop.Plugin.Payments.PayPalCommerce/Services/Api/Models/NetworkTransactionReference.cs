using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the reference values used by the card network to identify a transaction
/// </summary>
public class NetworkTransactionReference
{
    #region Properties

    /// <summary>
    /// Gets or sets the transaction reference id returned by the scheme. For Visa and Amex, this is the "Tran id" field in response. For MasterCard, this is the "BankNet reference id" field in response. For Discover, this is the "NRID" field in response. The pattern we expect for this field from Visa/Amex/CB/Discover is numeric, Mastercard/BNPP is alphanumeric and Paysecure is alphanumeric with special character -.
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the date that the transaction was authorized by the scheme. This field may not be returned for all networks. MasterCard refers to this field as "BankNet reference date.
    /// </summary>
    [JsonProperty(PropertyName = "date")]
    public string Date { get; set; }

    /// <summary>
    /// Gets or sets the reference ID issued for the card transaction. This ID can be used to track the transaction across processors, card brands and issuing banks.
    /// </summary>
    [JsonProperty(PropertyName = "acquirer_reference_number")]
    public string AcquirerReferenceNumber { get; set; }

    /// <summary>
    /// Gets or sets the name of the card network through which the transaction was routed.
    /// </summary>
    [JsonProperty(PropertyName = "network")]
    public string Network { get; set; }

    #endregion
}