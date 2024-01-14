using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace Nop.Plugin.POS.Kaching.Models.ReconciliationModels
{
    public partial class Reconciliation
    {
        [JsonProperty("base_currency_code")]
        public string BaseCurrencyCode { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("reconciliation_time")]
        public double ReconciliationTime { get; set; }
        
        [JsonProperty("reconciliations")]
        public ReconciliationElement[] Reconciliations { get; set; }

        [JsonProperty("register_summary")]
        public RegisterSummary RegisterSummary { get; set; }

        [JsonProperty("sequence_number")]
        public long SequenceNumber { get; set; }

        [JsonProperty("shop_info")]
        public ShopInfo ShopInfo { get; set; }

        [JsonProperty("source")]
        public Source Source { get; set; }

        // For presentation
        [JsonIgnore()]
        [BindNever]
        public DateTime ReconciliationTimePresentation { get; set; }

        [JsonIgnore()]
        [BindNever]
        public DateTime ReconciliationReceivedTimePresentation { get; set; }

        [JsonIgnore()]
        [BindNever]
        public string TotalCashPresentation { get; set; }

        [JsonIgnore()]
        [BindNever]
        public string TotalCashCountedPresentation { get; set; }

        [JsonIgnore()]
        [BindNever]
        public string TotalCashSalePresentation { get; set; }

        [JsonIgnore()]
        [BindNever]
        public string DepositedAmountPresentation { get; set; }

        [JsonIgnore()]
        [BindNever]
        public string GrandTotalPresentation { get; set; }
    }

    public partial class Reconciliation
    {
        public static Reconciliation FromJson(string json) => JsonConvert.DeserializeObject<Reconciliation>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Reconciliation self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}