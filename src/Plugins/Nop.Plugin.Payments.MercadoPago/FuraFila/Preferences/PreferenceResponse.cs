using System;
using Newtonsoft.Json;
using Nop.Plugin.Payments.MercadoPago.FuraFila.Models;

namespace Nop.Plugin.Payments.MercadoPago.FuraFila.Preferences
{
    public class PreferenceResponse
    {
        [JsonProperty(PropertyName = "collector_id")]
        public int CollectorId { get; set; }

        [JsonProperty(PropertyName = "operation_type")]
        public string OperationType { get; set; }

        public Item[] Items { get; set; }

        public Payer Payer { get; set; }

        [JsonProperty(PropertyName = "back_urls")]
        public BackUrls BackUrls { get; set; }

        [JsonProperty(PropertyName = "auto_return")]
        public string AutoReturn { get; set; }

        [JsonProperty(PropertyName = "payment_methods")]
        public PaymentMethods PaymentMethods { get; set; }

        [JsonProperty(PropertyName = "client_id")]
        public string ClientId { get; set; }

        public string Marketplace { get; set; }

        [JsonProperty(PropertyName = "marketplace_fee")]
        public int? MarketplaceFee { get; set; }

        public Shipment Shipments { get; set; }

        //public object notification_url { get; set; }

        [JsonProperty(PropertyName = "external_reference")]
        public string ExternalReference { get; set; }

        [JsonProperty(PropertyName = "additional_info")]
        public string AdditionalInfo { get; set; }

        public bool expires { get; set; }

        //public object expiration_date_from { get; set; }

        //public object expiration_date_to { get; set; }

        [JsonProperty(PropertyName = "date_created")]
        public DateTime DateCreated { get; set; }

        public string Id { get; set; }

        [JsonProperty(PropertyName = "init_point")]
        public string InitPoint { get; set; }

        [JsonProperty(PropertyName = "sandbox_init_point")]
        public string SandboxInitPoint { get; set; }
    }
}
