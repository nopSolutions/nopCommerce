using Newtonsoft.Json;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.JsonLD
{
    public record JsonLdAggregateRatingModel : BaseNopModel
    {
        #region Properties

        [JsonProperty("@type")]
        public static string Type => "AggregateRating";

        [JsonProperty("ratingValue")]
        public string RatingValue { get; set; }

        [JsonProperty("reviewCount")]
        public int ReviewCount { get; set; }

        #endregion
    }
}