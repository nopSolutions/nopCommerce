using Newtonsoft.Json;

namespace Nop.Web.Models.JsonLD;

public record JsonLdRatingModel : JsonLdModel
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