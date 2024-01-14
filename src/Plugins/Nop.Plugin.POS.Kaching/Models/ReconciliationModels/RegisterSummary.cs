using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models.ReconciliationModels
{
    public class RegisterSummary
    {
        [JsonProperty("all")]
        public All All { get; set; }

        [JsonProperty("cash_drawer_open_count")]
        public long CashDrawerOpenCount { get; set; }

        [JsonProperty("cash_total_at_open")]
        public long CashTotalAtOpen { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("number_of_aborted_sales")]
        public long NumberOfAbortedSales { get; set; }

        [JsonProperty("number_of_initiated_sales")]
        public long NumberOfInitiatedSales { get; set; }

        [JsonProperty("opened_at")]
        public long OpenedAt { get; set; }

        [JsonProperty("returns")]
        public Returns Returns { get; set; }

        [JsonProperty("sales")]
        public All Sales { get; set; }
    }
}