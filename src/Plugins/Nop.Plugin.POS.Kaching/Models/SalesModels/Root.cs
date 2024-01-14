using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.POS.Kaching.Models.SalesModels
{
    public class Item
    {
        public Text text { get; set; }
        public bool? line { get; set; }
        public bool? linebreak { get; set; }
        public LabelValue label_value { get; set; }
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

    public class LineItem
    {
        private Dictionary<string, string>? _localizedNames;

        public Dictionary<string, string>? LocalizedNames
        {
            get => _localizedNames ??= new Dictionary<string, string>();
            set => _localizedNames = value;
        }

        public string? Text { get; set; }

        public string? GetLocalizedName(string l10n)
        {
            return LocalizedNames?[l10n];
        }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        //[JsonProperty("name")]
        //public string Name { get; set; }

        [JsonProperty("name")]
        [JsonIgnore]
        public string? Name
        {
            get => Text ?? (_localizedNames != null ? _localizedNames.Values.FirstOrDefault() : null);
            set => Text = value;
        }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("retail_price")]
        public decimal RetailPrice { get; set; }

        [JsonProperty("sales_tax_amount")]
        public decimal SalesTaxAmount { get; set; }

        [JsonProperty("sub_total")]
        public decimal SubTotal { get; set; }

        [JsonProperty("base_price")]
        public decimal BasePrice { get; set; }

        [JsonProperty("taxes")]
        public Tax[] Taxes { get; set; }

        [JsonProperty("total")]
        public decimal Total { get; set; }

        [JsonProperty("total_tax_amount")]
        public decimal TotalTaxAmount { get; set; }

        [JsonProperty("vat_amount")]
        public decimal VatAmount { get; set; }

        [JsonProperty("ecom_id", NullValueHandling = NullValueHandling.Ignore)]
        public string EcomId { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("line_item_id")]
        public string LineItemId { get; set; }

        [JsonProperty("variant_id", NullValueHandling = NullValueHandling.Ignore)]
        public string VariantId { get; set; }

        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Tags { get; set; }

        [JsonProperty("behavior", NullValueHandling = NullValueHandling.Ignore)]
        public Behavior Behavior { get; set; }

        [JsonProperty("cost_price", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? CostPrice { get; set; }

        [JsonProperty("margin", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Margin { get; set; }

        [JsonProperty("margin_total", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? MarginTotal { get; set; }
    }

    public class Metadata
    {
        public string aid { get; set; }
        public decimal amount_authorized { get; set; }
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

    public class Payment
    {
        public decimal amount { get; set; }
        public string identifier { get; set; }
        public Metadata metadata { get; set; }
        public string payment_type { get; set; }
        public List<Receipt> receipts { get; set; }
        public bool success { get; set; }
    }

    public class Receipt
    {
        public string recipient { get; set; }
        public Report report { get; set; }
        public bool requires_signature { get; set; }
        public string source { get; set; }
    }

    public class ReceiptMetadata
    {
        public string footer { get; set; }
        public string header { get; set; }
        public ShopAddress shop_address { get; set; }
        public string vat_number { get; set; }
    }

    public class Report
    {
        public List<Item> items { get; set; }
    }

    public class Root
    {
        public string base_currency_code { get; set; }
        public string checkout_identifier { get; set; }
        public string identifier { get; set; }
        public List<Payment> payments { get; set; }
        public ReceiptMetadata receipt_metadata { get; set; }
        public int sequence_number { get; set; }
        public Source source { get; set; }
        public Summary summary { get; set; }
        public Timing timing { get; set; }
        public bool voided { get; set; }
    }

    public class ShopAddress
    {
        public string city { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string street { get; set; }
        public string zip { get; set; }
    }

    public class Source
    {
        public string build_number { get; set; }
        public string bundle_id { get; set; }
        public string cashier_full_name { get; set; }
        public string cashier_id { get; set; }
        public string cashier_name { get; set; }
        public string market_id { get; set; }
        public string market_name { get; set; }
        public string register_id { get; set; }
        public string register_name { get; set; }
        public string shop_id { get; set; }
        public string shop_name { get; set; }
        public string version_number { get; set; }
    }

    public class StyledText
    {
        public string style { get; set; }
        public string text { get; set; }
    }

    public class Summary
    {
        public decimal base_price { get; set; }
        public bool is_return { get; set; }
        public List<LineItem> line_items { get; set; }
        public decimal sales_tax_amount { get; set; }
        public decimal sub_total { get; set; }
        public List<Taxis> taxes { get; set; }
        public decimal total { get; set; }
        public decimal total_discounts { get; set; }
        public decimal total_tax_amount { get; set; }
        public decimal vat_amount { get; set; }
    }

    public class Taxis
    {
        public decimal amount { get; set; }
        public string name { get; set; }
        public decimal rate { get; set; }
        public decimal source_amount { get; set; }
        public string type { get; set; }
    }

    public class Text
    {
        public string alignment { get; set; }
        public StyledText styled_text { get; set; }
    }

    public class Timing
    {
        public double timestamp { get; set; }
        public string timestamp_date_string { get; set; }
        public DateTime timestamp_string { get; set; }
        public string timestamp_week_string { get; set; }
        public string timezone { get; set; }
    }

    public class Value
    {
        public string style { get; set; }
        public string text { get; set; }
    }


}
