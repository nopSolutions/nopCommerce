using System;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.MercadoPago.FuraFila.Models
{
    public class Payment
    {
        [JsonProperty(PropertyName = "operation_type")]
        public string OperationType { get; set; }

        [JsonProperty(PropertyName = "fee_details")]
        public FeeDetail[] FeeDetails { get; set; }

        [JsonProperty(PropertyName = "date_approved")]
        public DateTime? DateApproved { get; set; }

        public Payer Payer { get; set; }

        [JsonProperty(PropertyName = "transaction_details")]
        public TransactionDetails TransactionDetails { get; set; }

        [JsonProperty(PropertyName = "statement_descriptor")]
        public string StatementDescriptor { get; set; }

        public int? Installments { get; set; }

        [JsonProperty(PropertyName = "external_reference")]
        public string ExternalReference { get; set; }

        [JsonProperty(PropertyName = "date_of_expiration")]
        public DateTime? DateOfExpiration { get; set; }

        public int? Id { get; set; }

        [JsonProperty(PropertyName = "payment_type_id")]
        public string PaymentTypeId { get; set; }

        public Order Order { get; set; }

        [JsonProperty(PropertyName = "status_detail")]
        public string StatusDetail { get; set; }

        [JsonProperty(PropertyName = "live_mode")]
        public bool? LiveMode { get; set; }

        public string Status { get; set; }

        [JsonProperty(PropertyName = "transaction_amount_refunded")]
        public decimal TransactionAmountRefunded { get; set; }

        [JsonProperty(PropertyName = "transaction_amount")]
        public decimal TransactionAmount { get; set; }

        public string Description { get; set; }

        [JsonProperty(PropertyName = "money_release_date")]
        public DateTime MoneyReleaseDate { get; set; }

        public bool? Captured { get; set; }

        public int? CollectorId { get; set; }

        [JsonProperty(PropertyName = "taxes_amount")]
        public decimal? TaxesAmount { get; set; }

        [JsonProperty(PropertyName = "date_last_updated")]
        public DateTime? DateLastUpdated { get; set; }

        [JsonProperty(PropertyName = "date_created")]
        public DateTime? DateCreated { get; set; }

        [JsonProperty(PropertyName = "shipping_amount")]
        public decimal? ShippingAmount { get; set; }

        [JsonProperty(PropertyName = "issuer_id")]
        public string IssuerId { get; set; }

        [JsonProperty(PropertyName = "payment_method_id")]
        public string PaymentMethodId { get; set; }

        [JsonProperty(PropertyName = "binary_mode")]
        public bool? BinaryMode { get; set; }

        [JsonProperty(PropertyName = "processing_mode")]
        public string ProcessingMode { get; set; }

        [JsonProperty(PropertyName = "currency_id")]
        public string CurrencyId { get; set; }

        [JsonProperty(PropertyName = "shipping_cost")]
        public decimal ShippingCost { get; set; }
    }
}
