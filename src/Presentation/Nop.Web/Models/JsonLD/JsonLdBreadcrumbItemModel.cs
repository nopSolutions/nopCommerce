using Newtonsoft.Json;

namespace Nop.Web.Models.JsonLD;

public record JsonLdBreadcrumbItemModel : JsonLdModel
{
    #region Properties

    [JsonProperty("@id")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    #endregion
}