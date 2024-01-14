using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nop.Plugin.Admin.Accounting.Models.EconomicModels
{
    public class InvoiceBookModel
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("exchangeRate")]
        public int ExchangeRate { get; set; }

        [JsonProperty("netAmount")]
        public decimal NetAmount { get; set; }

        [JsonProperty("netAmountInBaseCurrency")]
        public decimal NetAmountInBaseCurrency { get; set; }

        [JsonProperty("grossAmount")]
        public decimal GrossAmount { get; set; }

        [JsonProperty("marginInBaseCurrency")]
        public decimal MarginInBaseCurrency { get; set; }

        [JsonProperty("marginPercentage")]
        public decimal MarginPercentage { get; set; }

        [JsonProperty("vatAmount")]
        public decimal VatAmount { get; set; }

        [JsonProperty("roundingAmount")]
        public decimal RoundingAmount { get; set; }

        [JsonProperty("costPriceInBaseCurrency")]
        public decimal CostPriceInBaseCurrency { get; set; }

        [JsonProperty("paymentTerms")]
        public PaymentTerms PaymentTerms { get; set; }

        [JsonProperty("customer")]
        public Customer Customer { get; set; }

        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }

        [JsonProperty("delivery")]
        public Delivery Delivery { get; set; }

        [JsonProperty("references")]
        public References References { get; set; }

        [JsonProperty("layout")]
        public Layout Layout { get; set; }

        [JsonProperty("lines")]
        public List<Line> Lines { get; set; }

        [JsonProperty("notes")]
        public Notes Notes { get; set; }
    }
}