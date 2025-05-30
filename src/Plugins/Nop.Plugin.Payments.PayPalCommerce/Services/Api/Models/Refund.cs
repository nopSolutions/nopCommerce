using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the refund
/// </summary>
public class Refund : IWebhookResource
{
    #region Properties

    /// <summary>
    /// Gets or sets the status of the refund.
    /// </summary>
    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the details of the refund status.
    /// </summary>
    [JsonProperty(PropertyName = "status_details")]
    public StatusDetails StatusDetails { get; set; }

    /// <summary>
    /// Gets or sets the PayPal-generated ID for the refund.
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the API caller-provided external invoice number for this order. Appears in both the payer's transaction history and the emails that the payer receives.
    /// </summary>
    [JsonProperty(PropertyName = "invoice_id")]
    public string InvoiceId { get; set; }

    /// <summary>
    /// Gets or sets the API caller-provided external ID. Used to reconcile API caller-initiated transactions with PayPal transactions. Appears in transaction and settlement reports.
    /// </summary>
    [JsonProperty(PropertyName = "custom_id")]
    public string CustomId { get; set; }

    /// <summary>
    /// Gets or sets the reference ID issued for the card transaction. This ID can be used to track the transaction across processors, card brands and issuing banks.
    /// </summary>
    [JsonProperty(PropertyName = "acquirer_reference_number")]
    public string AcquirerReferenceNumber { get; set; }

    /// <summary>
    /// Gets or sets the reason for the refund. Appears in both the payer's transaction history and the emails that the payer receives.
    /// </summary>
    [JsonProperty(PropertyName = "note_to_payer")]
    public string NoteToPayer { get; set; }

    /// <summary>
    /// Gets or sets the detailed breakdown of the refund.
    /// </summary>
    [JsonProperty(PropertyName = "seller_payable_breakdown")]
    public PaymentBreakdown SellerPayableBreakdown { get; set; }

    /// <summary>
    /// Gets or sets the array of related [HATEOAS links](/docs/api/overview/#hateoas-links).
    /// </summary>
    [JsonProperty(PropertyName = "links")]
    public List<Link> Links { get; set; }

    /// <summary>
    /// Gets or sets the amount that the payee refunded to the payer.
    /// </summary>
    [JsonProperty(PropertyName = "amount")]
    public Money Amount { get; set; }

    /// <summary>
    /// Gets or sets the details associated with the merchant for this transaction.
    /// </summary>
    [JsonProperty(PropertyName = "payee")]
    public Payee Payee { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the transaction occurred, in [Internet date and time format](https://tools.ietf.org/html/rfc3339#section-5.6).
    /// </summary>
    [JsonProperty(PropertyName = "create_time")]
    public string CreateTime { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the transaction was last updated, in [Internet date and time format](https://tools.ietf.org/html/rfc3339#section-5.6).
    /// </summary>
    [JsonProperty(PropertyName = "update_time")]
    public string UpdateTime { get; set; }

    #endregion
}