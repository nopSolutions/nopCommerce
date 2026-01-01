using System.Text.Json;

namespace AliExpressSdk.Models.Freight;

/// <summary>
/// Request model for aliexpress.ds.freight.query API
/// </summary>
public class FreightQueryRequest
{
    /// <summary>
    /// Delivery query request details
    /// </summary>
    public DeliveryQueryRequest QueryDeliveryReq { get; set; } = new();
    
    /// <summary>
    /// Convert to dictionary for API call
    /// </summary>
    public Dictionary<string, string> ToDictionary()
    {
        var json = JsonSerializer.Serialize(QueryDeliveryReq, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        return new Dictionary<string, string>
        {
            ["queryDeliveryReq"] = json
        };
    }
}

/// <summary>
/// Delivery query request details
/// </summary>
public class DeliveryQueryRequest
{
    /// <summary>
    /// Quantity (required)
    /// </summary>
    public int Quantity { get; set; } = 1;
    
    /// <summary>
    /// Ship to country (required)
    /// </summary>
    public string ShipToCountry { get; set; } = string.Empty;
    
    /// <summary>
    /// Product ID (required)
    /// </summary>
    public string ProductId { get; set; } = string.Empty;
    
    /// <summary>
    /// Province code (optional)
    /// </summary>
    public string? ProvinceCode { get; set; }
    
    /// <summary>
    /// City code (optional)
    /// </summary>
    public string? CityCode { get; set; }
    
    /// <summary>
    /// Language (required)
    /// </summary>
    public string Language { get; set; } = "en_US";
    
    /// <summary>
    /// Locale (required)
    /// </summary>
    public string Locale { get; set; } = "en_US";
    
    /// <summary>
    /// Selected SKU ID (required)
    /// </summary>
    public string SelectedSkuId { get; set; } = string.Empty;
    
    /// <summary>
    /// Currency for calculating freight fee (required)
    /// </summary>
    public string Currency { get; set; } = "USD";
}
