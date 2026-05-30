using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public abstract class OrderEventBaseProperty
{
    [JsonProperty("billingAddress")] public AddressItem BillingAddress { get; set; }
    [JsonProperty("createdAt")] public string CreatedAt { get; set; }
    [JsonProperty("currency")] public string Currency { get; set; }
    [JsonProperty("discounts")] public IList<DiscountItem> Discounts { get; set; }
    [JsonProperty("fulfillmentStatus")] public string FulfillmentStatus { get; set; }
    [JsonProperty("lineItems")] public IList<OrderProductItem> LineItems { get; set; }
    [JsonProperty("note")] public string Note { get; set; }
    [JsonProperty("orderID")] public string OrderId { get; set; }
    [JsonProperty("orderNumber")] public int OrderNumber { get; set; }
    [JsonProperty("orderStatusURL")] public string OrderStatusURL { get; set; }
    [JsonProperty("paymentMethod")] public string PaymentMethod { get; set; }
    [JsonProperty("paymentStatus")] public string PaymentStatus { get; set; }
    [JsonProperty("shippingAddress")] public AddressItem ShippingAddress { get; set; }
    [JsonProperty("shippingMethod")] public string ShippingMethod { get; set; }
    [JsonProperty("shippingPrice")] public float ShippingPrice { get; set; }
    [JsonProperty("subTotalPrice")] public float SubTotalPrice { get; set; }
    [JsonProperty("subTotalTaxIncluded")] public bool SubTotalTaxIncluded { get; set; }
    [JsonProperty("tags")] public IList<string> Tags { get; set; }
    [JsonProperty("totalDiscount")] public float TotalDiscount { get; set; }
    [JsonProperty("totalPrice")] public float TotalPrice { get; set; }
    [JsonProperty("totalTax")] public float TotalTax { get; set; }
    [JsonProperty("tracking")] public TrackingItem Tracking { get; set; }
}