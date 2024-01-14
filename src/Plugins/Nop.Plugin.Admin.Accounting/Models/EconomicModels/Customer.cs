using Newtonsoft.Json;

namespace Nop.Plugin.Admin.Accounting.Models.EconomicModels
{
    public class Customer
    {
        [JsonProperty("customerNumber")]
        public int CustomerNumber { get; set; }
    }
}