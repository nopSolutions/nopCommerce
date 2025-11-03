using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTO.Categories
{
    public class CategoriesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
