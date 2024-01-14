using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class ReceiptMetadata
    {
        [JsonProperty("shop_address")]
        public ShopAddress ShopAddress { get; set; }
    }
}