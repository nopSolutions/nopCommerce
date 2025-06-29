using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public class OrderDto : IBatchSupport
{
    [JsonProperty("orderID")] public string OrderId { get; set; }
    [JsonProperty("email")] public string Email { get; set; }
    [JsonProperty("currency")] public string Currency { get; set; }
    [JsonProperty("orderSum")] public int OrderSum { get; set; }
    [JsonProperty("subTotalSum")] public int SubTotalSum { get; set; }
    [JsonProperty("discountSum")] public int DiscountSum { get; set; }
    [JsonProperty("taxSum")] public int TaxSum { get; set; }
    [JsonProperty("shippingSum")] public int ShippingSum { get; set; }
    [JsonProperty("createdAt")] public string CreatedAt { get; set; }
    //[JsonProperty("updatedAt")] public string UpdatedAt { get; set; }
    //[JsonProperty("canceledDate")] public string CanceledDate { get; set; }
    [JsonProperty("products")] public List<OrderItemDto> Products { get; set; }

    public class OrderItemDto : ProductItemDto
    {

    }
}