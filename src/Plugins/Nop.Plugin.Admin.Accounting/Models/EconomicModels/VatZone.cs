using Newtonsoft.Json;

namespace Nop.Plugin.Admin.Accounting.Models.EconomicModels
{
    public class VatZone
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("vatZoneNumber")]
        public int VatZoneNumber { get; set; }

        [JsonProperty("enabledForCustomer")]
        public bool EnabledForCustomer { get; set; }

        [JsonProperty("enabledForSupplier")]
        public bool EnabledForSupplier { get; set; }
    }
}
