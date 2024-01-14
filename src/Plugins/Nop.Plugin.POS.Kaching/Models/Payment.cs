using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class Payment
    {
        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("payment_type")]
        public string PaymentType { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("metadata")]
        public Metadata MetaData { get; set; }
    }
}