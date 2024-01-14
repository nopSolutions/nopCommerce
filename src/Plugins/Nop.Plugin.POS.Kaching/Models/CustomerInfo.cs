using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class CustomerInfo
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
    }
}