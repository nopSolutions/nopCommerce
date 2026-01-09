using System.Text.Json.Serialization;

namespace Nop.Plugin.DropShipping.AliExpress.Models;

/// <summary>
/// Response models for AliExpress freight estimation
/// </summary>
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
