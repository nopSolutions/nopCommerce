using Newtonsoft.Json;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.JsonLD
{
    public record JsonLdPersonModel : BaseNopModel
    {
        #region Properties

        [JsonProperty("@type")]
        public static string Type => "Person";

        [JsonProperty("name")]
        public string Name { get; set; }

        #endregion
    }
}