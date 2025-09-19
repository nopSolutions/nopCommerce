using Newtonsoft.Json;

namespace Nop.Web.Models.JsonLD;

public partial record JsonLdBrandModel : JsonLdModel
{
    #region Properties

    [JsonProperty("@type")]
    public static string Type => "Brand";

    [JsonProperty("name")]
    public string Name { get; set; }

    #endregion
}