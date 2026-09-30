using System;

namespace Nop.Plugin.Api.Rest.Models.Requests
{
    /// <summary>
    /// Represents a request to create a product.
    /// </summary>
    public class AddProductRequest
    {
        /// <summary>
        /// Gets or sets the product name; required
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the product SKU
        /// </summary>
        public string? Sku { get; set; }

        /// <summary>
        /// Gets or sets the Global Trade Item Number (GTIN)
        /// </summary>
        public string? Gtin { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer part number
        /// </summary>
        public string? ManufacturerPartNumber { get; set; }

        /// <summary>
        /// Gets or sets the short description
        /// </summary>
        public string? ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the full description
        /// </summary>
        public string? FullDescription { get; set; }

        /// <summary>
        /// Gets or sets the product type identifier; see the ProductType enum; defaults to a simple product
        /// </summary>
        public int ProductTypeId { get; set; } = 5;

        /// <summary>
        /// Gets or sets the product template identifier
        /// </summary>
        public int ProductTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the vendor identifier; 0 assigns the product to no vendor
        /// </summary>
        public int VendorId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this product is visible in the catalog; defaults to true
        /// </summary>
        public bool VisibleIndividually { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the product is published; defaults to true
        /// </summary>
        public bool Published { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the price is hidden and the customer is asked for it
        /// </summary>
        public bool CallForPrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer enters the price
        /// </summary>
        public bool CustomerEntersPrice { get; set; }

        /// <summary>
        /// Gets or sets the price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the old price
        /// </summary>
        public decimal OldPrice { get; set; }

        /// <summary>
        /// Gets or sets the product cost
        /// </summary>
        public decimal ProductCost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is shipped; defaults to true
        /// </summary>
        public bool IsShipEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the product is shipped for free
        /// </summary>
        public bool IsFreeShipping { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is tax exempt
        /// </summary>
        public bool IsTaxExempt { get; set; }

        /// <summary>
        /// Gets or sets the inventory tracking method identifier; see the ManageInventoryMethod enum
        /// </summary>
        public int ManageInventoryMethodId { get; set; }

        /// <summary>
        /// Gets or sets the stock quantity
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Gets or sets the minimum stock quantity
        /// </summary>
        public int MinStockQuantity { get; set; }

        /// <summary>
        /// Gets or sets the backorder mode identifier; see the BackorderMode enum
        /// </summary>
        public int BackorderModeId { get; set; }

        /// <summary>
        /// Gets or sets the minimum quantity a customer can order
        /// </summary>
        public int OrderMinimumQuantity { get; set; } = 1;

        /// <summary>
        /// Gets or sets the maximum quantity a customer can order
        /// </summary>
        public int OrderMaximumQuantity { get; set; }

        /// <summary>
        /// Gets or sets the weight
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// Gets or sets the length
        /// </summary>
        public decimal Length { get; set; }

        /// <summary>
        /// Gets or sets the width
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        /// Gets or sets the height
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is marked as new
        /// </summary>
        public bool MarkAsNew { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product allows customer reviews; defaults to true
        /// </summary>
        public bool AllowCustomerReviews { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the buy button is disabled
        /// </summary>
        public bool DisableBuyButton { get; set; }

        /// <summary>
        /// Gets or sets the date and time from which the product is available
        /// </summary>
        public DateTime? AvailableStartDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time until which the product is available
        /// </summary>
        public DateTime? AvailableEndDateTimeUtc { get; set; }
    }
}