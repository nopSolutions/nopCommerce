using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nop.Plugin.Api.DTOs.Orders
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderStatus
    {
        Pending = 10,
        Processing = 20,
        Complete = 30,
        Cancelled = 40
    }
}
