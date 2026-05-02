using Newtonsoft.Json;

namespace Nop.Web.Models.JsonLD;

public partial record JsonLdAggregateRatingModel : JsonLdModel
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