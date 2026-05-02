using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Orders;

/// <summary>
/// Represents the request to add tracking information for an order
/// </summary>
public class CreateTrackingRequest : IAuthorizedRequest
{
    #region Properties

    /// <summary>
    /// Gets or sets the ID of the order that the tracking information is associated with.
    /// </summary>
    [JsonIgnore]
    public string OrderId { get; set; }

    /// <summary>
    /// Gets or sets the tracking number for the shipment. This property supports Unicode.
    /// </summary>
    [JsonProperty(PropertyName = "tracking_number")]
    public string TrackingNumber { get; set; }

    /// <summary>
    /// Gets or sets the carrier for the shipment.
    /// </summary>
    [JsonProperty(PropertyName = "carrier")]
    public string Carrier { get; set; }

    /// <summary>
    /// Gets or sets the name of the carrier for the shipment. Provide this value only if the carrier parameter is OTHER.
    /// </summary>
    [JsonProperty(PropertyName = "carrier_name_other")]
    public string CarrierNameOther { get; set; }

    /// <summary>
    /// Gets or sets the PayPal capture ID.
    /// </summary>
    [JsonProperty(PropertyName = "capture_id")]
    public string CaptureId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to send an email notification to the payer of the PayPal transaction.
    /// </summary>
    [JsonProperty(PropertyName = "notify_payer")]
    public bool NotifyPayer { get; set; }

    /// <summary>
    /// Gets or sets the array of details of items in the shipment.
    /// </summary>
    [JsonProperty(PropertyName = "items")]
    public List<Item> Items { get; set; }

    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public string Path => $"v2/checkout/orders/{Uri.EscapeDataString(OrderId)}/track?";

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public string Method => HttpMethods.Post;

    #endregion
}