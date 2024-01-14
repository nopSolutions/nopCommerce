using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class Source
    {
        [JsonProperty("cashier_name")]
        public string CashierName { get; set; }

        [JsonProperty("cashier_id")]
        public string CashierId { get; set; }

        [JsonProperty("market_id")]
        public string MarketId { get; set; }

        [JsonProperty("market_name")]
        public string MarketName { get; set; }

        [JsonProperty("register_id")]
        public string RegisterId { get; set; }

        [JsonProperty("register_name")]
        public string RegisterName { get; set; }

        [JsonProperty("shop_id")]
        public string ShopId { get; set; }

        [JsonProperty("shop_name")]
        public string ShopName { get; set; }
    }
}