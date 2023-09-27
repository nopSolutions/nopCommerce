using Newtonsoft.Json;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.JsonLD
{
    public record JsonLdBreadcrumbListItemModel : BaseNopModel
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
}