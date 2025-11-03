using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nop.Plugin.Api.DTOs.Orders
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ShippingStatus
    {
        ShippingNotRequired = 10,
        NotYetShipped = 20,
        PartiallyShipped = 25,
        Shipped = 30,
        Delivered = 40
    }
}
