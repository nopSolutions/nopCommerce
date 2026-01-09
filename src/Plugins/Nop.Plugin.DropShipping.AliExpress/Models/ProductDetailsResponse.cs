using System.Text.Json.Serialization;

namespace Nop.Plugin.DropShipping.AliExpress.Models;

/// <summary>
/// Response models for AliExpress product details
/// Uses the comprehensive models defined in AliExpressModels.cs
/// </summary>
public class ProductDetailsResponse
{
    [JsonPropertyName("aliexpress_ds_product_get_response")]
    public AliexpressDsProductGetResponse? AliexpressDsProductGetResponse { get; set; }
}

public class AliexpressDsProductGetResponse
{
    [JsonPropertyName("result")]
    public AliExpressProductDetailsResult? Result { get; set; }

    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }

    [JsonPropertyName("_trace_id_")]
    public string? TraceId { get; set; }
}
