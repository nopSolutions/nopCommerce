using Newtonsoft.Json;

namespace Nop.Web.Models.JsonLD;

public record JsonLdForumTopicCommentModel : JsonLdModel
{
    #region Properties

    [JsonProperty("@type")]
    public static string Type => "Comment";

    [JsonProperty("author")]
    public JsonLdPersonModel Author { get; set; }

    [JsonProperty("datePublished")]
    public string DatePublished { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("interactionStatistic")]
    public JsonLdInteractionStatisticModel InteractionStatistic { get; set; }

    #endregion
}