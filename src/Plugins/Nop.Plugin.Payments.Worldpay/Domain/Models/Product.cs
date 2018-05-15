using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Worldpay.Domain.Models
{
    /// <summary>
    /// Represents a details of the product.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Gets or sets an alternate tax id.
        /// </summary>
        [JsonProperty("alternateTaxId")]
        public string AlternateTaxId { get; set; }

        /// <summary>
        /// Gets or sets a commodity code.
        /// </summary>
        [JsonProperty("commodityCode")]
        public string CommodityCode { get; set; }

        /// <summary>
        /// Gets or sets a discount amount.
        /// </summary>
        [JsonProperty("discountAmount")]
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets a discount rate.
        /// </summary>
        [JsonProperty("discountRate")]
        public decimal DiscountRate { get; set; }

        /// <summary>
        /// Gets or sets a discount indicator.
        /// </summary>
        [JsonProperty("discountIndicator")]
        public string DiscountIndicator { get; set; }

        /// <summary>
        /// Gets or sets a gross net indicator.
        /// </summary>
        [JsonProperty("grossNetIndicator")]
        public string GrossNetIndicator { get; set; }

        /// <summary>
        /// Gets or sets an item code.
        /// </summary>
        [JsonProperty("itemCode")]
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets an item name. 
        /// </summary>
        [JsonProperty("itemName")]
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets an item description.
        /// </summary>
        [JsonProperty("itemDescription")]
        public string ItemDescription { get; set; }

        /// <summary>
        /// Gets or sets an unit. 
        /// </summary>
        [JsonProperty("unit")]
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets an unit price. 
        /// </summary>
        [JsonProperty("unitPrice")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets a quantity.
        /// </summary>
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets a total amount.
        /// </summary>
        [JsonProperty("totalAmount")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets a tax amount.
        /// </summary>
        [JsonProperty("taxAmount")]
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Gets or sets a tax rate.
        /// </summary>
        [JsonProperty("taxRate")]
        public decimal TaxRate { get; set; }

        /// <summary>
        /// Gets or sets a tax type identifier.
        /// </summary>
        [JsonProperty("taxTypeIdentifier")]
        public string TaxTypeIdentifier { get; set; }

        /// <summary>
        /// Gets or sets a tax type applied.
        /// </summary>
        [JsonProperty("taxTypeApplied")]
        public string TaxTypeApplied { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether product is taxable.
        /// </summary>
        [JsonProperty("Taxable")]
        public bool IsTaxable { get; set; }
    }
}