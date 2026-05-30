using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the captured payment
/// </summary>
public class Capture : IWebhookResource
{
    #region Properties

    /// <summary>
    /// Gets or sets the status of the captured payment.
    /// </summary>
    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the details of the captured payment status.
    /// </summary>
    [JsonProperty(PropertyName = "status_details")]
    public StatusDetails StatusDetails { get; set; }

    /// <summary>
    /// Gets or sets the PayPal-generated ID for the captured payment.
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
    /// Gets or sets a value indicating whether you can make additional captures against the authorized payment. Set to `true` if you do not intend to capture additional payments against the authorization. Set to `false` if you intend to capture additional payments against the authorization.
    /// </summary>
    [JsonProperty(PropertyName = "final_capture")]
    public bool? FinalCapture { get; set; }

    /// <summary>
    /// Gets or sets the funds that are held on behalf of the merchant.
    /// </summary>
    [JsonProperty(PropertyName = "disbursement_mode")]
    public string DisbursementMode { get; set; }

    /// <summary>
    /// Gets or sets the array of related [HATEOAS links](/docs/api/overview/#hateoas-links).
    /// </summary>
    [JsonProperty(PropertyName = "links")]
    public List<Link> Links { get; set; }

    /// <summary>
    /// Gets or sets the amount for this captured payment.
    /// </summary>
    [JsonProperty(PropertyName = "amount")]
    public Money Amount { get; set; }

    /// <summary>
    /// Gets or sets the reference values used by the card network to identify a transaction.
    /// </summary>
    [JsonProperty(PropertyName = "network_transaction_reference")]
    public NetworkTransactionReference NetworkTransactionReference { get; set; }

    /// <summary>
    /// Gets or sets the level of protection offered as defined by [PayPal Seller Protection for Merchants](https://www.paypal.com/us/webapps/mpp/security/seller-protection).
    /// </summary>
    [JsonProperty(PropertyName = "seller_protection")]
    public SellerProtection SellerProtection { get; set; }

    /// <summary>
    /// Gets or sets the detailed breakdown of the capture activity. This is not available for transactions that are in pending state.
    /// </summary>
    [JsonProperty(PropertyName = "seller_receivable_breakdown")]
    public PaymentBreakdown SellerReceivableBreakdowns { get; set; }

    /// <summary>
    /// Gets or sets the additional processor information.
    /// </summary>
    [JsonProperty(PropertyName = "processor_response")]
    public ProcessorResponse ProcessorResponse { get; set; }

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

    /// <summary>
    /// Gets or sets the details associated with the merchant for this transaction.
    /// </summary>
    [JsonProperty(PropertyName = "payee")]
    public Payee Payee { get; set; }

    #endregion
}