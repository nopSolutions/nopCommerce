using Newtonsoft.Json;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.JsonLD
{
    public record JsonLdBreadcrumbItemModel : BaseNopModel
    {
        #region Properties

        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        #endregion
    }
}