using Newtonsoft.Json;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.JsonLD
{
    public record JsonLdRatingModel : BaseNopModel
    {
        #region Properties

        [JsonProperty("@type")]
        public static string Type => "Rating";

        [JsonProperty("bestRating")]
        public string BestRating { get; set; }

        [JsonProperty("ratingValue")]
        public int RatingValue { get; set; }

        [JsonProperty("worstRating")]
        public string WorstRating { get; set; }

        #endregion
    }
}