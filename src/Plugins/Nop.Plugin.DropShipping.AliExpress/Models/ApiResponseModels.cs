using System.Text.Json.Serialization;

namespace Nop.Plugin.DropShipping.AliExpress.Models;

/// <summary>
/// Response models matching the exact structure from AliExpress API
/// Based on SampleResponses from ConsoleHarness
/// </summary>

#region Product Search Models

public class ProductSearchResponse
{
    [JsonPropertyName("aliexpress_ds_text_search_response")]
    public AliexpressDsTextSearchResponse? AliexpressDsTextSearchResponse { get; set; }
}

public class AliexpressDsTextSearchResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("data")]
    public ProductSearchData? Data { get; set; }

    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }

    [JsonPropertyName("_trace_id_")]
    public string? TraceId { get; set; }
}

public class ProductSearchData
{
    [JsonPropertyName("pageIndex")]
    public int PageIndex { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("products")]
    public ProductsContainer? Products { get; set; }
}

public class ProductsContainer
{
    [JsonPropertyName("selection_search_product")]
    public List<SelectionSearchProduct>? SelectionSearchProduct { get; set; }
}

public class SelectionSearchProduct
{
    [JsonPropertyName("itemId")]
    public string? ItemId { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("itemMainPic")]
    public string? ItemMainPic { get; set; }

    [JsonPropertyName("itemUrl")]
    public string? ItemUrl { get; set; }

    [JsonPropertyName("originalPrice")]
    public string? OriginalPrice { get; set; }

    [JsonPropertyName("originalPriceCurrency")]
    public string? OriginalPriceCurrency { get; set; }

    [JsonPropertyName("salePrice")]
    public string? SalePrice { get; set; }

    [JsonPropertyName("salePriceCurrency")]
    public string? SalePriceCurrency { get; set; }

    [JsonPropertyName("salePriceFormat")]
    public string? SalePriceFormat { get; set; }

    [JsonPropertyName("targetSalePrice")]
    public string? TargetSalePrice { get; set; }

    [JsonPropertyName("targetOriginalPrice")]
    public string? TargetOriginalPrice { get; set; }

    [JsonPropertyName("targetOriginalPriceCurrency")]
    public string? TargetOriginalPriceCurrency { get; set; }

    [JsonPropertyName("discount")]
    public string? Discount { get; set; }

    [JsonPropertyName("score")]
    public string? Score { get; set; }

    [JsonPropertyName("evaluateRate")]
    public string? EvaluateRate { get; set; }

    [JsonPropertyName("orders")]
    public string? Orders { get; set; }

    [JsonPropertyName("cateId")]
    public string? CateId { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

#endregion

#region Product Details Models

public class ProductDetailsResponse
{
    [JsonPropertyName("aliexpress_ds_product_get_response")]
    public AliexpressDsProductGetResponse? AliexpressDsProductGetResponse { get; set; }
}

public class AliexpressDsProductGetResponse
{
    [JsonPropertyName("result")]
    public ProductDetailsResult? Result { get; set; }

    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }

    [JsonPropertyName("_trace_id_")]
    public string? TraceId { get; set; }
}

public class ProductDetailsResult
{
    [JsonPropertyName("ae_item_sku_info_dtos")]
    public AeItemSkuInfoDtos? AeItemSkuInfoDtos { get; set; }

    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    [JsonPropertyName("subject")]
    public string? Subject { get; set; }

    [JsonPropertyName("product_main_image_url")]
    public string? ProductMainImageUrl { get; set; }

    [JsonPropertyName("product_video_url")]
    public string? ProductVideoUrl { get; set; }

    [JsonPropertyName("category_id")]
    public string? CategoryId { get; set; }
}

public class AeItemSkuInfoDtos
{
    [JsonPropertyName("ae_item_sku_info_d_t_o")]
    public List<AeItemSkuInfoDto>? AeItemSkuInfoDto { get; set; }
}

public class AeItemSkuInfoDto
{
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    [JsonPropertyName("sku_attr")]
    public string? SkuAttr { get; set; }

    [JsonPropertyName("sku_price")]
    public string? SkuPrice { get; set; }

    [JsonPropertyName("offer_sale_price")]
    public string? OfferSalePrice { get; set; }

    [JsonPropertyName("offer_bulk_sale_price")]
    public string? OfferBulkSalePrice { get; set; }

    [JsonPropertyName("sku_available_stock")]
    public int SkuAvailableStock { get; set; }

    [JsonPropertyName("currency_code")]
    public string? CurrencyCode { get; set; }

    [JsonPropertyName("price_include_tax")]
    public bool PriceIncludeTax { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("ae_sku_property_dtos")]
    public AeSkuPropertyDtos? AeSkuPropertyDtos { get; set; }
}

public class AeSkuPropertyDtos
{
    [JsonPropertyName("ae_sku_property_d_t_o")]
    public List<AeSkuPropertyDto>? AeSkuPropertyDto { get; set; }
}

public class AeSkuPropertyDto
{
    [JsonPropertyName("sku_property_id")]
    public long SkuPropertyId { get; set; }

    [JsonPropertyName("sku_property_name")]
    public string? SkuPropertyName { get; set; }

    [JsonPropertyName("sku_property_value")]
    public string? SkuPropertyValue { get; set; }

    [JsonPropertyName("property_value_id")]
    public long PropertyValueId { get; set; }

    [JsonPropertyName("property_value_definition_name")]
    public string? PropertyValueDefinitionName { get; set; }

    [JsonPropertyName("sku_image")]
    public string? SkuImage { get; set; }
}

#endregion

#region Freight Estimation Models

public class FreightEstimationResponse
{
    [JsonPropertyName("aliexpress_ds_freight_query_response")]
    public AliexpressDsFreightQueryResponse? AliexpressDsFreightQueryResponse { get; set; }
}

public class AliexpressDsFreightQueryResponse
{
    [JsonPropertyName("result")]
    public FreightEstimationResult? Result { get; set; }

    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }

    [JsonPropertyName("_trace_id_")]
    public string? TraceId { get; set; }
}

public class FreightEstimationResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("msg")]
    public string? Msg { get; set; }

    [JsonPropertyName("delivery_options")]
    public DeliveryOptions? DeliveryOptions { get; set; }
}

public class DeliveryOptions
{
    [JsonPropertyName("delivery_option_d_t_o")]
    public List<DeliveryOptionDto>? DeliveryOptionDto { get; set; }
}

public class DeliveryOptionDto
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("company")]
    public string? Company { get; set; }

    [JsonPropertyName("ship_from_country")]
    public string? ShipFromCountry { get; set; }

    [JsonPropertyName("free_shipping")]
    public bool FreeShipping { get; set; }

    [JsonPropertyName("tracking")]
    public bool Tracking { get; set; }

    [JsonPropertyName("min_delivery_days")]
    public int MinDeliveryDays { get; set; }

    [JsonPropertyName("max_delivery_days")]
    public int MaxDeliveryDays { get; set; }

    [JsonPropertyName("guaranteed_delivery_days")]
    public string? GuaranteedDeliveryDays { get; set; }

    [JsonPropertyName("delivery_date_desc")]
    public string? DeliveryDateDesc { get; set; }

    [JsonPropertyName("available_stock")]
    public string? AvailableStock { get; set; }

    [JsonPropertyName("mayHavePFS")]
    public bool MayHavePFS { get; set; }
}

#endregion

#region Order Creation Models

public class OrderCreateResponse
{
    [JsonPropertyName("aliexpress_ds_order_create_response")]
    public AliexpressDsOrderCreateResponse? AliexpressDsOrderCreateResponse { get; set; }
}

public class AliexpressDsOrderCreateResponse
{
    [JsonPropertyName("result")]
    public OrderCreateResult? Result { get; set; }

    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }

    [JsonPropertyName("_trace_id_")]
    public string? TraceId { get; set; }
}

public class OrderCreateResult
{
    [JsonPropertyName("is_success")]
    public bool IsSuccess { get; set; }

    [JsonPropertyName("order_list")]
    public OrderList? OrderList { get; set; }

    [JsonPropertyName("error_code")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }
}

public class OrderList
{
    [JsonPropertyName("number")]
    public List<long>? Number { get; set; }
}

#endregion
