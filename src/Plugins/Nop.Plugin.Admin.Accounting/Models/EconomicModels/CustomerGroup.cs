using Newtonsoft.Json;

namespace Nop.Plugin.Admin.Accounting.Models.EconomicModels
{
    public class CustomerGroup
    {
        [JsonProperty("customerGroupNumber")]
        public int CustomerGroupNumber { get; set; }
    }
}