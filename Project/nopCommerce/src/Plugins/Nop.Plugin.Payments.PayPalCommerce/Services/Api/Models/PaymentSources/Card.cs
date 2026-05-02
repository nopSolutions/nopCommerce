using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.PaymentSources;

/// <summary>
/// Represents the payment card to use to fund a payment
/// </summary>
public class Card
{
    #region Properties

    /// <summary>
    /// Gets or sets the card holder's name as it appears on the card.
    /// </summary>
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the primary account number (PAN) for the payment card.
    /// </summary>
    [JsonProperty(PropertyName = "number")]
    public string Number { get; set; }

    /// <summary>
    /// Gets or sets the three- or four-digit security code of the card. Also known as the CVV, CVC, CVN, CVE, or CID. This parameter cannot be present in the request when `payment_initiator=MERCHANT`.
    /// </summary>
    [JsonProperty(PropertyName = "security_code")]
    public string SecurityCode { get; set; }

    /// <summary>
    /// Gets or sets the year and month, in ISO-8601 `YYYY-MM` date format. See [Internet date and time format](https://tools.ietf.org/html/rfc3339#section-5.6).
    /// </summary>
    [JsonProperty(PropertyName = "expiry")]
    public string Expiry { get; set; }

    /// <summary>
    /// Gets or sets the billing address for this card.
    /// </summary>
    [JsonProperty(PropertyName = "billing_address")]
    public Address BillingAddress { get; set; }

    /// <summary>
    /// Gets or sets the additional attributes associated with the use of this card.
    /// </summary>
    [JsonProperty(PropertyName = "attributes")]
    public Attributes Attributes { get; set; }

    /// <summary>
    /// Gets or sets the additional details to process a payment using a `card` that has been stored or is intended to be stored (also referred to as stored_credential or card-on-file).
    /// </summary>
    [JsonProperty(PropertyName = "stored_credential")]
    public StoredCredential StoredCredential { get; set; }

    /// <summary>
    /// Gets or sets the PayPal-generated ID for the saved card payment source. Typically stored on the merchant's server.
    /// </summary>
    [JsonProperty(PropertyName = "vault_id")]
    public string VaultId { get; set; }

    /// <summary>
    /// Gets or sets the 3rd party network token refers to a network token that the merchant provisions from and vaults with an external TSP (Token Service Provider) other than PayPal.
    /// </summary>
    [JsonProperty(PropertyName = "network_token")]
    public NetworkToken NetworkToken { get; set; }

    /// <summary>
    /// Gets or sets the payer experience during the 3DS Approval for payment.
    /// </summary>
    [JsonProperty(PropertyName = "experience_context")]
    public ExperienceContext ExperienceContext { get; set; }

    /// <summary>
    /// Gets or sets the results of Authentication such as 3D Secure.
    /// </summary>
    [JsonProperty(PropertyName = "authentication_result")]
    public AuthenticationResult AuthenticationResult { get; set; }

    /// <summary>
    /// Gets or sets the previous network transaction reference including id in response.
    /// </summary>
    [JsonProperty(PropertyName = "network_transaction_reference")]
    public NetworkTransactionReference NetworkTransactionReference { get; set; }

    /// <summary>
    /// Gets or sets the last digits of the payment card.
    /// </summary>
    [JsonProperty(PropertyName = "last_digits")]
    public string LastDigits { get; set; }

    /// <summary>
    /// Gets or sets the card brand or network. Typically used in the response.
    /// </summary>
    [JsonProperty(PropertyName = "card_type")]
    public string CardType { get; set; }

    /// <summary>
    /// Gets or sets the card Verification status.
    /// </summary>
    [JsonProperty(PropertyName = "verification_status")]
    public string VerificationStatus { get; set; }

    /// <summary>
    /// Gets or sets the Bank Identification Number (BIN) details used to fund a payment.
    /// </summary>
    [JsonProperty(PropertyName = "bin_details")]
    public BinDetails BinDetails { get; set; }

    /// <summary>
    /// Gets or sets the payment card type. Typically used in the response.
    /// </summary>
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the card brand or network. Typically used in the response.
    /// </summary>
    [JsonProperty(PropertyName = "brand")]
    public string Brand { get; set; }

    #endregion
}