namespace Nop.Plugin.DropShipping.AliExpress.Models;

/// <summary>
/// Represents a product search result from AliExpress
/// </summary>
public class AliExpressProductSearchResultModel
{
    public long ProductId { get; set; }
    public string? ProductTitle { get; set; }
    public string? ProductUrl { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? OriginalPrice { get; set; }
    public decimal? SalePrice { get; set; }
    public string? Currency { get; set; }
    public int? SalesCount { get; set; }
    public decimal? Rating { get; set; }
    public int? ReviewCount { get; set; }
    public string? ShopName { get; set; }
}

/// <summary>
/// Represents detailed product information from AliExpress
/// </summary>
public class AliExpressProductDetailsModel
{
    public long ProductId { get; set; }
    public string? ProductTitle { get; set; }
    public string? ProductUrl { get; set; }
    public List<string>? ImageUrls { get; set; }
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public string? Currency { get; set; }
    public List<AliExpressProductAttributeModel>? Attributes { get; set; }
    public List<AliExpressProductSpecificationModel>? Specifications { get; set; }
    public bool IsAvailable { get; set; }
    public int? StockQuantity { get; set; }
}

/// <summary>
/// Represents a product attribute (e.g., Color, Size)
/// </summary>
public class AliExpressProductAttributeModel
{
    public string? AttributeName { get; set; }
    public List<AliExpressProductAttributeValueModel>? Values { get; set; }
}

/// <summary>
/// Represents an attribute value
/// </summary>
public class AliExpressProductAttributeValueModel
{
    public string? ValueId { get; set; }
    public string? ValueName { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? PriceAdjustment { get; set; }
}

/// <summary>
/// Represents a product specification
/// </summary>
public class AliExpressProductSpecificationModel
{
    public string? SpecName { get; set; }
    public string? SpecValue { get; set; }
}

/// <summary>
/// Represents freight/shipping information
/// </summary>
public class AliExpressFreightModel
{
    public string? ServiceName { get; set; }
    public decimal ShippingCost { get; set; }
    public string? Currency { get; set; }
    public int? EstimatedDeliveryDays { get; set; }
    public string? TrackingAvailable { get; set; }
}

/// <summary>
/// Represents order tracking information
/// </summary>
public class AliExpressOrderTrackingModel
{
    public long OrderId { get; set; }
    public string? OrderStatus { get; set; }
    public string? TrackingNumber { get; set; }
    public string? LogisticsService { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public List<AliExpressTrackingEventModel>? TrackingEvents { get; set; }
}

/// <summary>
/// Represents a tracking event
/// </summary>
public class AliExpressTrackingEventModel
{
    public DateTime EventDate { get; set; }
    public string? EventDescription { get; set; }
    public string? Location { get; set; }
}
