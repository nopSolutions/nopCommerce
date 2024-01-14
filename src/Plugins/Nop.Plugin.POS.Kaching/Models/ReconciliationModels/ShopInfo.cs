using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models.ReconciliationModels
{
    public class ShopInfo
    {
        [JsonProperty("shop_address")]
        public ShopAddress ShopAddress { get; set; }

        [JsonProperty("vat_number")]
        public string VatNumber { get; set; }
    }
}