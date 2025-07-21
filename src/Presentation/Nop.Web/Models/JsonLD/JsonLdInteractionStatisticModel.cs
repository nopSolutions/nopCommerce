using Newtonsoft.Json;

namespace Nop.Web.Models.JsonLD;

public record JsonLdInteractionStatisticModel : JsonLdModel
{
    #region Properties

    [JsonProperty("@type")]
    public static string Type => "InteractionCounter";

    [JsonProperty("interactionType")]
    public string InteractionType { get; set; }

    [JsonProperty("userInteractionCount")]
    public int VoteCount { get; set; }

    #endregion
}