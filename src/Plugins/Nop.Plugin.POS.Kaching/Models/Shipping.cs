using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class Shipping
    {
        [JsonProperty("address")]
        public Address Address { get; set; }

        [JsonProperty("customer_info")]
        public CustomerInfo CustomerInfo { get; set; }
    }
}