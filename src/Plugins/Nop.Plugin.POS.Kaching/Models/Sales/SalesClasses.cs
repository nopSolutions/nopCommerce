using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nop.Plugin.POS.Kaching.Models.Sales
{
    public class SalesClass
    {
        public Dictionary<string, Transaction> Transactions { get; set; }
    }

    public class Transaction
    {
        [JsonProperty(PropertyName = "base_currency_code")]
        public string BaseCurrencyCode { get; set; }

        [JsonProperty(PropertyName = "checkout_identifier")]
        public string CheckoutIdentifier { get; set; }

        [JsonProperty(PropertyName = "identifier")]
        public string Identifier { get; set; }

        [JsonProperty(PropertyName = "payments")]
        public List<Payment> Payments { get; set; }

        [JsonProperty(PropertyName = "receipt_metadata")]
        public ReceiptMetadata ReceiptMetadata { get; set; }

        [JsonProperty(PropertyName = "sequence_number")]
        public int SequenceNumber { get; set; }

        [JsonProperty(PropertyName = "source")]
        public Source Source { get; set; }

        [JsonProperty(PropertyName = "summary")]
        public Summary Summary { get; set; }

        [JsonProperty(PropertyName = "timing")]
        public Timing Timing { get; set; }

        [JsonProperty(PropertyName = "voided")]
        public bool Voided { get; set; }
    }

    public class Payment
    {
        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }

        [JsonProperty(PropertyName = "identifier")]
        public string Identifier { get; set; }

        [JsonProperty(PropertyName = "payment_type")]
        public string PaymentType { get; set; }

        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        public List<Receipt> receipts { get; set; }

        public Metadata metadata { get; set; }

    }

    public class ShopAddress
    {
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "street")]
        public string Street { get; set; }

        [JsonProperty(PropertyName = "zip")]
        public string Zip { get; set; }
    }

    public class ReceiptMetadata
    {
        [JsonProperty(PropertyName = "footer")]
        public string Footer { get; set; }

        [JsonProperty(PropertyName = "header")]
        public string Header { get; set; }

        [JsonProperty(PropertyName = "shop_address")]
        public ShopAddress ShopAddress { get; set; }

        [JsonProperty(PropertyName = "vat_number")]
        public string VatNumber { get; set; }
    }

    public class Source
    {
        [JsonProperty(PropertyName = "build_number")]
        public string BuildNumber { get; set; }

        [JsonProperty(PropertyName = "bundle_id")]
        public string BundleId { get; set; }

        [JsonProperty(PropertyName = "cashier_full_name")]
        public string CashierFullName { get; set; }

        [JsonProperty(PropertyName = "cashier_id")]
        public string CashierId { get; set; }

        [JsonProperty(PropertyName = "cashier_name")]
        public string CashierName { get; set; }

        [JsonProperty(PropertyName = "market_id")]
        public string MarketId { get; set; }

        [JsonProperty(PropertyName = "market_name")]
        public string MarketName { get; set; }

        [JsonProperty(PropertyName = "register_id")]
        public string RegisterId { get; set; }

        [JsonProperty(PropertyName = "register_name")]
        public string RegisterName { get; set; }

        [JsonProperty(PropertyName = "shop_id")]
        public string ShopId { get; set; }

        [JsonProperty(PropertyName = "shop_name")]
        public string ShopName { get; set; }

        [JsonProperty(PropertyName = "version_number")]
        public string VersionNumber { get; set; }
    }

    public class Summary
    {
        [JsonProperty(PropertyName = "base_price")]
        public double BasePrice { get; set; }

        [JsonProperty(PropertyName = "is_return")]
        public bool IsReturn { get; set; }

        [JsonProperty(PropertyName = "line_items")]
        public List<LineItem> LineItems { get; set; }

        [JsonProperty(PropertyName = "margin")]
        public double Margin { get; set; }

        [JsonProperty(PropertyName = "margin_total")]
        public double MarginTotal { get; set; }

        [JsonProperty(PropertyName = "sales_tax_amount")]
        public double SalesTaxAmount { get; set; }

        [JsonProperty(PropertyName = "sub_total")]
        public double SubTotal { get; set; }

        [JsonProperty(PropertyName = "taxes")]
        public List<Taxis> Taxes { get; set; }

        [JsonProperty(PropertyName = "total")]
        public double Total { get; set; }

        [JsonProperty(PropertyName = "total_discounts")]
        public double TotalDiscounts { get; set; }

        [JsonProperty(PropertyName = "total_tax_amount")]
        public double TotalTaxAmount { get; set; }

        [JsonProperty(PropertyName = "vat_amount")]
        public double VatAmount { get; set; }
    }

    public class Timing
    {
        [JsonProperty(PropertyName = "timestamp")]
        public double Timestamp { get; set; }

        [JsonProperty(PropertyName = "timestamp_date_string")]
        public string TimestampDateString { get; set; }

        [JsonProperty(PropertyName = "timestamp_string")]
        public string TimestampString { get; set; }

        [JsonProperty(PropertyName = "timestamp_week_string")]
        public string TimestampWeekString { get; set; }

        [JsonProperty(PropertyName = "timezone")]
        public string Timezone { get; set; }
    }

    public class Label
    {
        public string style { get; set; }
        public string text { get; set; }
    }

    public class LabelValue
    {
        public Label label { get; set; }
        public Value value { get; set; }
    }

    public class Metadata
    {
        public string aid { get; set; }
        public int amount_authorized { get; set; }
        public string app_transaction_counter { get; set; }
        public string authorization_code { get; set; }
        public string base_amount_receipt { get; set; }
        public string card_acceptor_id { get; set; }
        public string card_type { get; set; }
        public string cardholder_string { get; set; }
        public string cless_indication_label { get; set; }
        public string currency_text { get; set; }
        public string cvr { get; set; }
        public string footer_msg_customer { get; set; }
        public string footer_msg_merchant { get; set; }
        public string issuer_card_name { get; set; }
        public string issuer_id { get; set; }
        public string merchant_id { get; set; }
        public string merchant_id_customer_text { get; set; }
        public string merchant_id_merchant_text { get; set; }
        public int online { get; set; }
        public string pan { get; set; }
        public string pan_customer { get; set; }
        public string pan_merchant { get; set; }
        public string pan_sequence_number { get; set; }
        public string payment_application_name { get; set; }
        public string pos_entry_mode { get; set; }
        public string posref { get; set; }
        public string response_code { get; set; }
        public string stan { get; set; }
        public string terminal_id { get; set; }
        public string terminal_id_customer_text { get; set; }
        public string terminal_id_merchant_text { get; set; }
        public string total_amount_receipt { get; set; }
        public string total_text_customer { get; set; }
        public string total_text_merchant { get; set; }
        public string transaction_date { get; set; }
        public string transaction_name_customer { get; set; }
        public string transaction_name_merchant { get; set; }
        public int transaction_status { get; set; }
        public string transaction_time { get; set; }
        public string transaction_type { get; set; }
        public string trx_status_customer { get; set; }
        public string trx_status_merchant { get; set; }
        public string tvr { get; set; }
    }

    public class Receipt
    {
        public string recipient { get; set; }
        public Report report { get; set; }
        public bool requires_signature { get; set; }
        public string source { get; set; }
    }

    public class Report
    {
        public List<Item> items { get; set; }
    }

    public class Item
    {
        public Text text { get; set; }
        public bool? line { get; set; }
        public bool? linebreak { get; set; }
        public LabelValue label_value { get; set; }
    }

    public class StyledText
    {
        public string style { get; set; }
        public string text { get; set; }
    }

    public class Taxis
    {
        public double amount { get; set; }
        public string name { get; set; }
        public double rate { get; set; }
        public double source_amount { get; set; }
        public string type { get; set; }
    }

    public class Text
    {
        public string alignment { get; set; }
        public StyledText styled_text { get; set; }
    }

    public class Value
    {
        public string style { get; set; }
        public string text { get; set; }
    }
}