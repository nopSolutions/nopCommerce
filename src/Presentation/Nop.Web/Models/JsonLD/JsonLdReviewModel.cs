using Newtonsoft.Json;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.JsonLD
{
    public record JsonLdReviewModel : BaseNopModel
    {
        #region Properties

        [JsonProperty("@type")]
        public static string Type => "Review";

        [JsonProperty("author")]
        public JsonLdPersonModel Author { get; set; }

        [JsonProperty("datePublished")]
        public string DatePublished { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("reviewBody")]
        public string ReviewBody { get; set; }

        [JsonProperty("reviewRating")]
        public JsonLdRatingModel ReviewRating { get; set; }

        #endregion
    }
}