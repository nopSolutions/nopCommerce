using System.Text.Json.Serialization;

namespace Nop.Plugin.DropShipping.AliExpress.Models;

/// <summary>
/// Response models for AliExpress order creation
/// </summary>
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
