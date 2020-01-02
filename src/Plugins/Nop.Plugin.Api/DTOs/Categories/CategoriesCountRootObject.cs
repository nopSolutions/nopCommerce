using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Categories
{
    public class CategoriesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}