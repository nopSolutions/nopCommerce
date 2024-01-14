using Newtonsoft.Json;

namespace Nop.Plugin.Admin.Accounting.Models.EconomicModels
{
    public class Unit
    {
        [JsonProperty("unitNumber")]
        public int UnitNumber { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
