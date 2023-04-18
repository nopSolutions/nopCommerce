using Newtonsoft.Json;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.JsonLD
{
    public record JsonLdBreadcrumbListModel : BaseNopModel
    {
        public JsonLdBreadcrumbListModel()
        {
            ItemListElement = new List<JsonLdBreadcrumbListItemModel>();
        }
        [JsonProperty("@context")]
        public static string Context => "https://schema.org";

        [JsonProperty("@type")]
        public static string Type => "BreadcrumbList";

        [JsonProperty("itemListElement")]
        public IList<JsonLdBreadcrumbListItemModel> ItemListElement { get; set; }
    }
}