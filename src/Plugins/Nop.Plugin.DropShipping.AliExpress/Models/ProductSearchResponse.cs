using System.Text.Json.Serialization;

namespace Nop.Plugin.DropShipping.AliExpress.Models;

/// <summary>
/// Response models for AliExpress product search
/// </summary>
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
