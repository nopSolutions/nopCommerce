using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the payment method token
/// </summary>
public class PaymentToken : IWebhookResource
{
    #region Properties

    /// <summary>
    /// Gets or sets the unique ID for the vault token.
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the status of the payment token.
    /// </summary>
    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the vaulted payment method details.
    /// </summary>
    [JsonProperty(PropertyName = "payment_source")]
    public PaymentSource PaymentSource { get; set; }

    /// <summary>
    /// Gets or sets the array of request-related [HATEOAS links](/docs/api/reference/api-responses/#hateoas-links). To complete payer approval, use the `approve` link with the `GET` method.
    /// </summary>
    [JsonProperty(PropertyName = "links")]
    public List<Link> Links { get; set; }

    /// <summary>
    /// Gets or sets the customer in merchant's or partner's system of records.
    /// </summary>
    [JsonProperty(PropertyName = "customer")]
    public Payer Customer { get; set; }

    /// <summary>
    /// Gets or sets the ordinal number of customers' payment source for sorting.
    /// </summary>
    [JsonProperty(PropertyName = "ordinal")]
    public int? Ordinal { get; set; }

    /// <summary>
    /// Gets or sets the unique ID for the owner ID.
    /// </summary>
    [JsonProperty(PropertyName = "owner_id")]
    public string OwnerId { get; set; }

    /// <summary>
    /// Gets or sets the metadata.
    /// </summary>
    [JsonProperty(PropertyName = "metadata")]
    public PaymentTokenMetadata Metadata { get; set; }

    /// <summary>
    /// Gets or sets the API caller-provided external ID.
    /// </summary>
    [JsonIgnore]
    public string CustomId { get; set; }

    #endregion
}