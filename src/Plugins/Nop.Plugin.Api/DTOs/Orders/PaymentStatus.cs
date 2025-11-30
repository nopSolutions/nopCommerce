using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nop.Plugin.Api.DTOs.Orders
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentStatus
    {
        Pending = 10,
        Authorized = 20,
        Paid = 30,
        PartiallyRefunded = 35,
        Refunded = 40,
        Voided = 50
    }
}
