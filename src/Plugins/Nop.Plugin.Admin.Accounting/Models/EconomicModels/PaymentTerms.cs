using Newtonsoft.Json;

namespace Nop.Plugin.Admin.Accounting.Models.EconomicModels
{
    public class PaymentTerms
    {
        [JsonProperty("paymentTermsNumber")]
        public int PaymentTermsNumber { get; set; }

        [JsonProperty("daysOfCredit")]
        public int DaysOfCredit { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("paymentTermsType")]
        public string PaymentTermsType { get; set; }
    }
}
