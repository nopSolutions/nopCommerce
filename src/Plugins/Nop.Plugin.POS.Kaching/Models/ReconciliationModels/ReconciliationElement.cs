using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Nop.Plugin.POS.Kaching.Models.ReconciliationModels
{
    public class ReconciliationElement
    {
        [JsonProperty("counted")]
        public double Counted { get; set; }

        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("deposited_amount", NullValueHandling = NullValueHandling.Ignore)]
        public double? DepositedAmount { get; set; }

        [JsonProperty("depositing")]
        public string Depositing { get; set; }

        [JsonProperty("payment_type_identifier")]
        public string PaymentTypeIdentifier { get; set; }

        [JsonProperty("should_be_reconciled")]
        public bool ShouldBeReconciled { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }

        [JsonIgnore()]
        [BindNever]
        public string TotalCountedPresentation { get; set; }

        [JsonIgnore()]
        [BindNever]
        public string TotalSalePresentation { get; set; }

    }
}