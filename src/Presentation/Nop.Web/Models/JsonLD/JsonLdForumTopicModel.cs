using Newtonsoft.Json;

namespace Nop.Web.Models.JsonLD;

public record JsonLdForumTopicModel : JsonLdModel
{
    #region Ctor

    public JsonLdForumTopicModel()
    {
        Comments = new List<JsonLdForumTopicCommentModel>();
        InteractionStatistic = new List<JsonLdInteractionStatisticModel>();
    }

    #endregion

    #region Properties

    [JsonProperty("@context")]
    public string Context => "https://schema.org";

    [JsonProperty("@type")]
    public string Type => "DiscussionForumPosting";

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("author")]
    public JsonLdPersonModel Author { get; set; }

    [JsonProperty("datePublished")]
    public string DatePublished { get; set; }

    [JsonProperty("headline")]
    public string Subject { get; set; }

    [JsonProperty("comment")]
    public IList<JsonLdForumTopicCommentModel> Comments { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("interactionStatistic")]
    public IList<JsonLdInteractionStatisticModel> InteractionStatistic { get; set; }

    #endregion
}