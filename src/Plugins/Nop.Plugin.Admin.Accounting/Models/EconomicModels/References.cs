using Newtonsoft.Json;

namespace Nop.Plugin.Admin.Accounting.Models.EconomicModels
{
    public class References
    {
        [JsonProperty("other")]
        public string Other { get; set; }
    }

}
