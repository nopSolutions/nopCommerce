using Newtonsoft.Json;

namespace Nop.Web.Models.JsonLD;

public record JsonLdBreadcrumbListItemModel : JsonLdModel
{
    #region Properties

    [JsonProperty("@type")]
    public static string Type => "ListItem";

    [JsonProperty("position")]
    public int Position { get; set; }

    [JsonProperty("item")]
    public JsonLdBreadcrumbItemModel Item { get; set; }

    #endregion
}