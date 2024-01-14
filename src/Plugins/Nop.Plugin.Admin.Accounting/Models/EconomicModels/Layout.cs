using Newtonsoft.Json;

namespace Nop.Plugin.Admin.Accounting.Models.EconomicModels
{
    public class Layout
    {
        [JsonProperty("layoutNumber")]
        public int LayoutNumber { get; set; }
    }
}
