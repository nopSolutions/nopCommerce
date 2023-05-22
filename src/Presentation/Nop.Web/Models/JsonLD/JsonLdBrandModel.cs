using Newtonsoft.Json;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.JsonLD
{
    public record JsonLdBrandModel : BaseNopModel
    {
        #region Properties

        [JsonProperty("@type")]
        public static string Type => "Brand";

        [JsonProperty("name")]
        public string Name { get; set; }

        #endregion
    }
}