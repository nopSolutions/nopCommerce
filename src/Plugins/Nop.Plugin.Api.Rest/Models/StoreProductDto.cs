using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Rest.Models;

/// <summary>
/// A product as a storefront client sees it
/// </summary>
/// <remarks>
/// A projection of the catalog for callers that are browsing rather than administering. It carries the
/// buying information a product page needs, and nothing that only the admin area edits.
/// </remarks>
public record StoreProductDto
{
    /// <summary>
    /// Gets or sets the product identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the product name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the SKU
    /// </summary>
    public string? Sku { get; set; }

    /// <summary>
    /// Gets or sets the GTIN
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
    /// Gets or sets the price
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the compare at price
    /// </summary>
    public decimal OldPrice { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the customer enters the price
    /// </summary>
    public bool CustomerEntersPrice { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the buy button is disabled
    /// </summary>
    public bool DisableBuyButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product is shipped
    /// </summary>
    public bool IsShipEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product is free shipping
    /// </summary>
    public bool IsFreeShipping { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product is tax exempt
    /// </summary>
    public bool IsTaxExempt { get; set; }

    /// <summary>
    /// Gets or sets the stock quantity
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Gets or sets the minimum stock quantity
    /// </summary>
    public int MinStockQuantity { get; set; }

    /// <summary>
    /// Gets or sets the backorder mode identifier
    /// </summary>
    public int BackorderModeId { get; set; }

    /// <summary>
    /// Gets or sets the minimum order quantity
    /// </summary>
    public int OrderMinimumQuantity { get; set; }

    /// <summary>
    /// Gets or sets the maximum order quantity
    /// </summary>
    public int OrderMaximumQuantity { get; set; }

    /// <summary>
    /// Gets or sets the product type identifier
    /// </summary>
    public int ProductTypeId { get; set; }

    /// <summary>
    /// Gets or sets the parent grouped product identifier, for a grouped product's association
    /// </summary>
    public int ParentGroupedProductId { get; set; }

    /// <summary>
    /// Gets or sets the identifiers of the manufacturers the product is mapped to
    /// </summary>
    /// <remarks>
    /// nopCommerce maps a product to many manufacturers rather than holding a single identifier on the
    /// product, so this is a list
    /// </remarks>
    public List<int> ManufacturerIds { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the product is visible on its own page
    /// </summary>
    public bool VisibleIndividually { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product is shown on the home page
    /// </summary>
    public bool MarkAsNew { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether customers may review the product
    /// </summary>
    public bool AllowCustomerReviews { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

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
    /// Gets or sets the date and time the product becomes available (UTC)
    /// </summary>
    public DateTime? AvailableStartDateTimeUtc { get; set; }

    /// <summary>
    /// Gets or sets the date and time the product stops being available (UTC)
    /// </summary>
    public DateTime? AvailableEndDateTimeUtc { get; set; }

    /// <summary>
    /// Gets or sets the attribute combinations the product is sold in
    /// </summary>
    public List<ProductAttributeCombinationSummaryDto> Combinations { get; set; } = new();

    /// <summary>
    /// Gets or sets the product tags
    /// </summary>
    public List<string?> Tags { get; set; } = new();
}

/// <summary>
/// A sellable variant of a product, defined by a specific set of attribute values
/// </summary>
public record ProductAttributeCombinationSummaryDto
{
    /// <summary>
    /// Gets or sets the combination identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the SKU
    /// </summary>
    public string? Sku { get; set; }

    /// <summary>
    /// Gets or sets the GTIN
    /// </summary>
    public string? Gtin { get; set; }

    /// <summary>
    /// Gets or sets the manufacturer part number
    /// </summary>
    public string? ManufacturerPartNumber { get; set; }

    /// <summary>
    /// Gets or sets the stock quantity
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether orders are allowed while out of stock
    /// </summary>
    public bool AllowOutOfStockOrders { get; set; }

    /// <summary>
    /// Gets or sets the price that replaces the product price for this variant
    /// </summary>
    public decimal? OverriddenPrice { get; set; }

    /// <summary>
    /// Gets or sets the selected attribute values in nopCommerce AttributesXml format
    /// </summary>
    public string? AttributesXml { get; set; }
}
