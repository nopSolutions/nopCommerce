#nullable enable
using Newtonsoft.Json;

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

/// <summary>
/// Represents the root response for AliExpress search
/// </summary>
public class AliExpressSearchRootResponse
{
    [JsonProperty("aliexpress_ds_text_search_response")]
    public AliExpressSearchResponse? SearchResponse { get; set; }
}

/// <summary>
/// Represents the search response from AliExpress
/// </summary>
public class AliExpressSearchResponse
{
    [JsonProperty("code")]
    public string? Code { get; set; }

    [JsonProperty("data")]
    public AliExpressSearchData? Data { get; set; }

    [JsonProperty("request_id")]
    public string? RequestId { get; set; }

    [JsonProperty("_trace_id_")]
    public string? TraceId { get; set; }
}

/// <summary>
/// Represents the search data including pagination and product list
/// </summary>
public class AliExpressSearchData
{
    [JsonProperty("pageIndex")]
    public int PageIndex { get; set; }

    [JsonProperty("pageSize")]
    public int PageSize { get; set; }

    [JsonProperty("totalCount")]
    public int TotalCount { get; set; }

    [JsonProperty("products")]
    public AliExpressSearchProducts? Products { get; set; }
}

/// <summary>
/// Represents the collection of searched products
/// </summary>
public class AliExpressSearchProducts
{
    [JsonProperty("selection_search_product")]
    public List<AliExpressSearchProduct>? Items { get; set; }
}

/// <summary>
/// Represents a product in the search results
/// </summary>
public class AliExpressSearchProduct
{
    [JsonProperty("originalPrice")]
    public string? OriginalPrice { get; set; }

    [JsonProperty("originalPriceCurrency")]
    public string? OriginalPriceCurrency { get; set; }

    [JsonProperty("salePrice")]
    public string? SalePrice { get; set; }

    [JsonProperty("discount")]
    public string? Discount { get; set; }

    [JsonProperty("itemMainPic")]
    public string? ItemMainPic { get; set; }

    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("score")]
    public string? Score { get; set; }

    [JsonProperty("itemId")]
    public string? ItemId { get; set; }

    [JsonProperty("targetSalePrice")]
    public string? TargetSalePrice { get; set; }

    [JsonProperty("cateId")]
    public string? CateId { get; set; }

    [JsonProperty("targetOriginalPriceCurrency")]
    public string? TargetOriginalPriceCurrency { get; set; }

    [JsonProperty("originMinPrice")]
    public string? OriginMinPrice { get; set; }

    [JsonProperty("evaluateRate")]
    public string? EvaluateRate { get; set; }

    [JsonProperty("salePriceFormat")]
    public string? SalePriceFormat { get; set; }

    [JsonProperty("orders")]
    public string? Orders { get; set; }

    [JsonProperty("targetOriginalPrice")]
    public string? TargetOriginalPrice { get; set; }

    [JsonProperty("itemUrl")]
    public string? ItemUrl { get; set; }

    [JsonProperty("salePriceCurrency")]
    public string? SalePriceCurrency { get; set; }
}

// AliExpress Product Details Response Models
public class AliExpressProductDetailsRootResponse
{
    [JsonProperty("aliexpress_ds_product_get_response")]
    public AliExpressProductDetailsResponse? ProductDetailsResponse { get; set; }
}

public class AliExpressProductDetailsResponse
{
    [JsonProperty("result")]
    public AliExpressProductDetailsResult? Result { get; set; }

    [JsonProperty("rsp_code")]
    public int RspCode { get; set; }

    [JsonProperty("rsp_msg")]
    public string? RspMsg { get; set; }

    [JsonProperty("request_id")]
    public string? RequestId { get; set; }

    [JsonProperty("_trace_id_")]
    public string? TraceId { get; set; }
}

public class AliExpressProductDetailsResult
{
    [JsonProperty("ae_item_sku_info_dtos")]
    public AeItemSkuInfoDtos? AeItemSkuInfoDtos { get; set; }

    [JsonProperty("ae_multimedia_info_dto")]
    public AeMultimediaInfoDto? MultimediaInfo { get; set; }

    [JsonProperty("package_info_dto")]
    public PackageInfoDto? PackageInfo { get; set; }

    [JsonProperty("logistics_info_dto")]
    public LogisticsInfoDto? LogisticsInfo { get; set; }

    [JsonProperty("product_id_converter_result")]
    public ProductIdConverterResult? ProductIdConverterResult { get; set; }

    [JsonProperty("ae_item_base_info_dto")]
    public AeItemBaseInfoDto? BaseInfo { get; set; }

    [JsonProperty("has_whole_sale")]
    public bool HasWholeSale { get; set; }

    [JsonProperty("ae_item_properties")]
    public AeItemProperties? ItemProperties { get; set; }

    [JsonProperty("ae_store_info")]
    public AeStoreInfo? StoreInfo { get; set; }
}

public class AeItemSkuInfoDtos
{
    [JsonProperty("ae_item_sku_info_d_t_o")]
    public List<AeItemSkuInfoDto>? SkuList { get; set; }
}

public class AeItemSkuInfoDto
{
    [JsonProperty("sku_attr")]
    public string? SkuAttr { get; set; }

    [JsonProperty("offer_sale_price")]
    public string? OfferSalePrice { get; set; }

    [JsonProperty("sku_id")]
    public string? SkuId { get; set; }

    [JsonProperty("price_include_tax")]
    public bool PriceIncludeTax { get; set; }

    [JsonProperty("currency_code")]
    public string? CurrencyCode { get; set; }

    [JsonProperty("sku_price")]
    public string? SkuPrice { get; set; }

    [JsonProperty("offer_bulk_sale_price")]
    public string? OfferBulkSalePrice { get; set; }

    [JsonProperty("sku_available_stock")]
    public int SkuAvailableStock { get; set; }

    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("ae_sku_property_dtos")]
    public AeSkuPropertyDtos? SkuPropertyDtos { get; set; }
}

public class AeSkuPropertyDtos
{
    [JsonProperty("ae_sku_property_d_t_o")]
    public List<AeSkuPropertyDto>? SkuProperties { get; set; }
}

public class AeSkuPropertyDto
{
    [JsonProperty("sku_property_value")]
    public string? SkuPropertyValue { get; set; }

    [JsonProperty("sku_image")]
    public string? SkuImage { get; set; }

    [JsonProperty("sku_property_name")]
    public string? SkuPropertyName { get; set; }

    [JsonProperty("property_value_definition_name")]
    public string? PropertyValueDefinitionName { get; set; }

    [JsonProperty("property_value_id")]
    public long PropertyValueId { get; set; }

    [JsonProperty("sku_property_id")]
    public long SkuPropertyId { get; set; }
}

public class AeMultimediaInfoDto
{
    [JsonProperty("image_urls")]
    public string? ImageUrls { get; set; }
}

public class PackageInfoDto
{
    [JsonProperty("package_width")]
    public int PackageWidth { get; set; }

    [JsonProperty("package_height")]
    public int PackageHeight { get; set; }

    [JsonProperty("package_length")]
    public int PackageLength { get; set; }

    [JsonProperty("gross_weight")]
    public string? GrossWeight { get; set; }

    [JsonProperty("package_type")]
    public bool PackageType { get; set; }

    [JsonProperty("product_unit")]
    public long ProductUnit { get; set; }
}

public class LogisticsInfoDto
{
    [JsonProperty("delivery_time")]
    public int DeliveryTime { get; set; }

    [JsonProperty("ship_to_country")]
    public string? ShipToCountry { get; set; }
}

public class ProductIdConverterResult
{
    [JsonProperty("main_product_id")]
    public long MainProductId { get; set; }

    [JsonProperty("sub_product_id")]
    public string? SubProductId { get; set; }
}

public class AeItemBaseInfoDto
{
    [JsonProperty("mobile_detail")]
    public string? MobileDetail { get; set; }

    [JsonProperty("subject")]
    public string? Subject { get; set; }

    [JsonProperty("evaluation_count")]
    public string? EvaluationCount { get; set; }

    [JsonProperty("sales_count")]
    public string? SalesCount { get; set; }

    [JsonProperty("product_status_type")]
    public string? ProductStatusType { get; set; }

    [JsonProperty("avg_evaluation_rating")]
    public string? AvgEvaluationRating { get; set; }

    [JsonProperty("separated_listing")]
    public bool SeparatedListing { get; set; }

    [JsonProperty("currency_code")]
    public string? CurrencyCode { get; set; }

    [JsonProperty("category_id")]
    public long CategoryId { get; set; }

    [JsonProperty("product_id")]
    public long ProductId { get; set; }

    [JsonProperty("detail")]
    public string? Detail { get; set; }
}

public class AeItemProperties
{
    [JsonProperty("ae_item_property")]
    public List<AeItemProperty>? Properties { get; set; }
}

public class AeItemProperty
{
    [JsonProperty("attr_name_id")]
    public long AttrNameId { get; set; }

    [JsonProperty("attr_value_id")]
    public long AttrValueId { get; set; }

    [JsonProperty("attr_name")]
    public string? AttrName { get; set; }

    [JsonProperty("attr_value")]
    public string? AttrValue { get; set; }
}

public class AeStoreInfo
{
    [JsonProperty("store_id")]
    public long StoreId { get; set; }

    [JsonProperty("shipping_speed_rating")]
    public string? ShippingSpeedRating { get; set; }

    [JsonProperty("communication_rating")]
    public string? CommunicationRating { get; set; }

    [JsonProperty("store_name")]
    public string? StoreName { get; set; }

    [JsonProperty("store_country_code")]
    public string? StoreCountryCode { get; set; }

    [JsonProperty("item_as_described_rating")]
    public string? ItemAsDescribedRating { get; set; }
}
#nullable restore
