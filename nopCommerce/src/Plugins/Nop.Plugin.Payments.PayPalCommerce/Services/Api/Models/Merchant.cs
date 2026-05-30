using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the merchant details object
/// </summary>
public class Merchant : IWebhookResource
{
    #region Properties

    /// <summary>
    /// Gets or sets the payer ID of the seller after creation of their PayPal account.
    /// </summary>
    [JsonProperty(PropertyName = "merchant_id")]
    public string MerchantId { get; set; }

    /// <summary>
    /// Gets or sets the partner's unique identifier for this customer in their system which can be used to track user in PayPal.
    /// </summary>
    [JsonProperty(PropertyName = "tracking_id")]
    public string TrackingId { get; set; }

    /// <summary>
    /// Gets or sets the array of PayPal products to which the partner wants to onboard the customer.
    /// </summary>
    [JsonProperty(PropertyName = "products")]
    public List<PayPalProduct> Products { get; set; }

    /// <summary>
    /// Gets or sets the array of capabilities associated with the products integrated between seller and partner.
    /// </summary>
    [JsonProperty(PropertyName = "capabilities")]
    public List<ProductCapability> Capabilities { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the seller account can receive payments.
    /// </summary>
    [JsonProperty(PropertyName = "payments_receivable")]
    public bool? PaymentsReceivable { get; set; }

    /// <summary>
    /// Gets or sets the seller legal name.
    /// </summary>
    [JsonProperty(PropertyName = "legal_name")]
    public string LegalName { get; set; }

    /// <summary>
    /// Gets or sets the primary email address of the seller.
    /// </summary>
    [JsonProperty(PropertyName = "primary_email")]
    public string PrimaryEmail { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the primary email of the seller has been confirmed.
    /// </summary>
    [JsonProperty(PropertyName = "primary_email_confirmed")]
    public bool? PrimaryEmailConfirmed { get; set; }

    /// <summary>
    /// Gets or sets the array of information about OAuth integrations between partners and sellers.
    /// </summary>
    [JsonProperty(PropertyName = "oauth_integrations")]
    public List<Integration> OauthIntegrations { get; set; }

    /// <summary>
    /// Gets or sets the primary currency of the seller account.
    /// </summary>
    [JsonProperty(PropertyName = "primary_currency")]
    public string PrimaryCurrency { get; set; }

    /// <summary>
    /// Gets or sets the seller account country.
    /// </summary>
    [JsonProperty(PropertyName = "country")]
    public string Country { get; set; }

    /// <summary>
    /// Gets or sets the API caller-provided external ID
    /// </summary>
    [JsonIgnore]
    public string CustomId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Advanced Credit and Debit Card payments is active
    /// </summary>
    [JsonIgnore]
    public (bool Active, string Status) AdvancedCards { get; set; }

    /// <summary>
    /// Gets or sets additional details about Advanced Credit and Debit Card status
    /// </summary>
    [JsonIgnore]
    public (bool OnReview, bool NeedMoreData, bool BelowLimit, bool OverLimit, bool Denied) AdvancedCardsDetails { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Apple Pay is active
    /// </summary>
    [JsonIgnore]
    public (bool Active, string Status) ApplePay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Google Pay is active
    /// </summary>
    [JsonIgnore]
    public (bool Active, string Status) GooglePay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Vaulting is active
    /// </summary>
    [JsonIgnore]
    public (bool Active, string Status) Vaulting { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Messaging Configurator is supported
    /// </summary>
    [JsonIgnore]
    public bool ConfiguratorSupported { get; set; }

    #endregion
}