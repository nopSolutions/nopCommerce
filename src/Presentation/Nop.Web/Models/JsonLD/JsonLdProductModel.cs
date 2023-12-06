using Newtonsoft.Json;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.JsonLD
{
    public record JsonLdProductModel : BaseNopModel
    {
        #region Ctor

        public JsonLdProductModel()
        {
            Brand = new List<JsonLdBrandModel>();
            Review = new List<JsonLdReviewModel>();
            HasVariant = new List<JsonLdProductModel>();
        }

        #endregion

        #region Properties

        [JsonProperty("@context")]
        public static string Context => "https://schema.org";

        [JsonProperty("@type")]
        public static string Type => "Product";

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("gtin")]
        public string Gtin { get; set; }

        [JsonProperty("mpn")]
        public string Mpn { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("brand")]
        public IList<JsonLdBrandModel> Brand { get; set; }

        [JsonProperty("offers")]
        public JsonLdOfferModel Offer { get; set; }

        [JsonProperty("aggregateRating")]
        public JsonLdAggregateRatingModel AggregateRating { get; set; }

        [JsonProperty("review")]
        public IList<JsonLdReviewModel> Review { get; set; }

        [JsonProperty("hasVariant")]
        public IList<JsonLdProductModel> HasVariant { get; set; }

        #endregion
    }
}