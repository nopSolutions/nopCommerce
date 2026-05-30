using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the order details
/// </summary>
public class Order : IWebhookResource
{
    #region Properties

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
    /// Gets or sets the ID of the order.
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the instruction to process an order.
    /// </summary>
    [JsonProperty(PropertyName = "processing_instruction")]
    public string ProcessingInstruction { get; set; }

    /// <summary>
    /// Gets or sets the array of purchase units. Each purchase unit establishes a contract between a customer and merchant. Each purchase unit represents either a full or partial order that the customer intends to purchase from the merchant.
    /// </summary>
    [JsonProperty(PropertyName = "purchase_units")]
    public List<PurchaseUnit> PurchaseUnits { get; set; }

    /// <summary>
    /// Gets or sets the array of request-related [HATEOAS links](/docs/api/reference/api-responses/#hateoas-links). To complete payer approval, use the `approve` link with the `GET` method.
    /// </summary>
    [JsonProperty(PropertyName = "links")]
    public List<Link> Links { get; set; }

    /// <summary>
    /// Gets or sets the payment source used to fund the payment.
    /// </summary>
    [JsonProperty(PropertyName = "payment_source")]
    public PaymentSource PaymentSource { get; set; }

    /// <summary>
    /// Gets or sets the intent to either capture payment immediately or authorize a payment for an order after order creation.
    /// </summary>
    [JsonProperty(PropertyName = "intent")]
    public string Intent { get; set; }

    /// <summary>
    /// Gets or sets the customer who approves and pays for the order. The customer is also known as the payer.
    /// </summary>
    [JsonProperty(PropertyName = "payer")]
    public Payer Payer { get; set; }

    /// <summary>
    /// Gets or sets the order status.
    /// </summary>
    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; }

    /// <summary>
    /// Gets the payer-action URL.
    /// </summary>
    [JsonIgnore]
    public string PayerActionUrl => Status?.ToUpper() == OrderStatusType.CREATED.ToString() || Status?.ToUpper() == OrderStatusType.PAYER_ACTION_REQUIRED.ToString()
        ? Links?.FirstOrDefault(link => string.Equals(link.Rel, "payer-action", StringComparison.InvariantCultureIgnoreCase))?.Href
        : null;

    /// <summary>
    /// Gets or sets the API caller-provided external ID.
    /// </summary>
    [JsonIgnore]
    public string CustomId
    {
        get => PurchaseUnits?.FirstOrDefault()?.CustomId;
        set { }
    }

    #endregion
}