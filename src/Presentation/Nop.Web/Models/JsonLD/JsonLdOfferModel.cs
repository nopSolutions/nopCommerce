using Newtonsoft.Json;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.JsonLD
{
    public record JsonLdOfferModel : BaseNopModel
    {
        #region Properties

        [JsonProperty("@type")]
        public static string Type => "Offer";

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("availability")]
        public string Availability { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("priceCurrency")]
        public string PriceCurrency { get; set; }

        [JsonProperty("priceValidUntil")]
        public DateTime? PriceValidUntil { get; set; }

        #endregion
    }
}