using Newtonsoft.Json;

namespace Nop.Plugin.Admin.Accounting.Models.EconomicModels
{
    public class Notes
    {
        [JsonProperty("heading")]
        public string Heading { get; set; }

        [JsonProperty("textLine1")]
        public string TextLine1 { get; set; }

        [JsonProperty("textLine2")]
        public string TextLine2 { get; set; }
    }
}
