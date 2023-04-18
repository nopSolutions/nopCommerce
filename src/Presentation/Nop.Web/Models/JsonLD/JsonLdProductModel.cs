using Newtonsoft.Json;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.JsonLD
{
    public record JsonLdProductModel : BaseNopModel
    {
        public JsonLdProductModel()
        {
            Brand = new List<JsonLdBrandModel>();
            IsAccessoryOrSparePartFor = new List<JsonLdProductModel>();
            Review = new List<JsonLdReviewModel>();
        }
        [JsonProperty("@context")]
        public static string Context => "https://schema.org";

        [JsonProperty("@type")]
        public static string Type => "Product";

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("Offers")]
        public JsonLdOfferModel Offer { get; set; }

        [JsonProperty("aggregateRating")]
        public JsonLdAggregateRatingModel AggregateRating { get; set; }

        [JsonProperty("review")]
        public IList<JsonLdReviewModel> Review { get; set; }

        [JsonProperty("isAccessoryOrSparePartFor")]
        public IList<JsonLdProductModel> IsAccessoryOrSparePartFor { get; set; }

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

        public IList<JsonLdBrandModel> Brand { get; set; }
    }
}