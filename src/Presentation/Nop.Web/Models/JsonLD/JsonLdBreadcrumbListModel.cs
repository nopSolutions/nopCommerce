using Newtonsoft.Json;

namespace Nop.Web.Models.JsonLD;

public record JsonLdBreadcrumbListModel : JsonLdModel
{
    #region Ctor

    public JsonLdBreadcrumbListModel()
    {
        ItemListElement = new List<JsonLdBreadcrumbListItemModel>();
    }

    #endregion

    #region Properties

    [JsonProperty("@context")]
    public static string Context => "https://schema.org";

    [JsonProperty("@type")]
    public static string Type => "BreadcrumbList";

    [JsonProperty("itemListElement")]
    public IList<JsonLdBreadcrumbListItemModel> ItemListElement { get; set; }

    #endregion
}