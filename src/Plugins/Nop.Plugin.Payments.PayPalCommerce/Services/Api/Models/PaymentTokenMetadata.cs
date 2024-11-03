using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the payment token metadata
/// </summary>
public class PaymentTokenMetadata
{
    #region Properties

    /// <summary>
    /// Gets or sets the related order ID.
    /// </summary>
    [JsonProperty(PropertyName = "order_id")]
    public string OrderId { get; set; }

    #endregion
}