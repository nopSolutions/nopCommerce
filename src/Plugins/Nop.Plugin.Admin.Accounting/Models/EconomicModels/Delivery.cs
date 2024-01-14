using Newtonsoft.Json;
using System;

namespace Nop.Plugin.Admin.Accounting.Models.EconomicModels
{
    public class Delivery
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("deliveryDate")]
        public DateTime DeliveryDate { get; set; }
    }
}
