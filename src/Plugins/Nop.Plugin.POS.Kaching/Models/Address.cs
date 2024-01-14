using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class Address
    {
        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }
    }
}