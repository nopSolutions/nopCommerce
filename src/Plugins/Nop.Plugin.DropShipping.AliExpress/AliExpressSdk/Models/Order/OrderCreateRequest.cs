using System.Text.Json;

namespace AliExpressSdk.Models.Order;

/// <summary>
/// Request model for aliexpress.ds.order.create API
/// </summary>
public class OrderCreateRequest
{
    /// <summary>
    /// DS extend request (optional)
    /// </summary>
    public DsExtendRequest? DsExtendRequest { get; set; }
    
    /// <summary>
    /// Place order request (required)
    /// </summary>
    public PlaceOrderRequest ParamPlaceOrderRequest4OpenApiDto { get; set; } = new();
    
    /// <summary>
    /// Convert to dictionary for API call
    /// </summary>
    public Dictionary<string, string> ToDictionary()
    {
        var dict = new Dictionary<string, string>();
        
        if (DsExtendRequest != null)
        {
            var extendJson = JsonSerializer.Serialize(DsExtendRequest, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
            dict["ds_extend_request"] = extendJson;
        }
        
        var orderJson = JsonSerializer.Serialize(ParamPlaceOrderRequest4OpenApiDto, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
        dict["param_place_order_request4_open_api_d_t_o"] = orderJson;
        
        return dict;
    }
}

/// <summary>
/// DS extend request parameters
/// </summary>
public class DsExtendRequest
{
    /// <summary>
    /// Promotion details (optional)
    /// </summary>
    public PromotionRequest? Promotion { get; set; }
    
    /// <summary>
    /// Payment details (optional)
    /// </summary>
    public PaymentRequest? Payment { get; set; }
    
    /// <summary>
    /// Trade extra parameters (optional)
    /// </summary>
    public TradeExtraParam? TradeExtraParam { get; set; }
}

/// <summary>
/// Promotion request
/// </summary>
public class PromotionRequest
{
    /// <summary>
    /// Promotion code (optional)
    /// </summary>
    public string? PromotionCode { get; set; }
    
    /// <summary>
    /// Promotion channel info (optional)
    /// </summary>
    public string? PromotionChannelInfo { get; set; }
}

/// <summary>
/// Payment request
/// </summary>
public class PaymentRequest
{
    /// <summary>
    /// Payment currency (optional)
    /// </summary>
    public string? PayCurrency { get; set; }
    
    /// <summary>
    /// Try to pay (optional)
    /// </summary>
    public string? TryToPay { get; set; }
}

/// <summary>
/// Trade extra parameters
/// </summary>
public class TradeExtraParam
{
    /// <summary>
    /// Business model (wholesale or retail)
    /// </summary>
    public string? BusinessModel { get; set; }
}

/// <summary>
/// Place order request
/// </summary>
public class PlaceOrderRequest
{
    /// <summary>
    /// Out order ID for idempotency (optional, recommended)
    /// </summary>
    public string? OutOrderId { get; set; }
    
    /// <summary>
    /// Logistics address (required)
    /// </summary>
    public LogisticsAddress LogisticsAddress { get; set; } = new();
    
    /// <summary>
    /// Product items (required)
    /// </summary>
    public List<ProductItem> ProductItems { get; set; } = new();
    
    /// <summary>
    /// Order memo (optional)
    /// </summary>
    public string? OrderMemo { get; set; }
}

/// <summary>
/// Logistics address information
/// </summary>
public class LogisticsAddress
{
    /// <summary>
    /// Address (required)
    /// </summary>
    public string Address { get; set; } = string.Empty;
    
    /// <summary>
    /// Address extension (optional)
    /// </summary>
    public string? Address2 { get; set; }
    
    /// <summary>
    /// City (required)
    /// </summary>
    public string City { get; set; } = string.Empty;
    
    /// <summary>
    /// Contact person (optional, required for Brazil CPF)
    /// </summary>
    public string? ContactPerson { get; set; }
    
    /// <summary>
    /// Country code (required, two-letter)
    /// </summary>
    public string Country { get; set; } = string.Empty;
    
    /// <summary>
    /// CPF/Taxpayer ID (required for Brazil)
    /// </summary>
    public string? Cpf { get; set; }
    
    /// <summary>
    /// Full name (optional)
    /// </summary>
    public string? FullName { get; set; }
    
    /// <summary>
    /// Locale (optional)
    /// </summary>
    public string? Locale { get; set; }
    
    /// <summary>
    /// Mobile number (optional)
    /// </summary>
    public string? MobileNo { get; set; }
    
    /// <summary>
    /// Passport number (required for Mexico)
    /// </summary>
    public string? PassportNo { get; set; }
    
    /// <summary>
    /// Passport expiry date (optional)
    /// </summary>
    public string? PassportNoDate { get; set; }
    
    /// <summary>
    /// Passport issuing organization (optional)
    /// </summary>
    public string? PassportOrganization { get; set; }
    
    /// <summary>
    /// Phone country code (optional)
    /// </summary>
    public string? PhoneCountry { get; set; }
    
    /// <summary>
    /// Province (required)
    /// </summary>
    public string Province { get; set; } = string.Empty;
    
    /// <summary>
    /// Tax number (optional)
    /// </summary>
    public string? TaxNumber { get; set; }
    
    /// <summary>
    /// ZIP code (optional)
    /// </summary>
    public string? Zip { get; set; }
    
    /// <summary>
    /// RUT number for Chile (optional)
    /// </summary>
    public string? RutNo { get; set; }
    
    /// <summary>
    /// Foreign passport number for Korea (optional)
    /// </summary>
    public string? ForeignerPassportNo { get; set; }
    
    /// <summary>
    /// Is foreigner (optional)
    /// </summary>
    public string? IsForeigner { get; set; }
    
    /// <summary>
    /// VAT number (optional)
    /// </summary>
    public string? VatNo { get; set; }
    
    /// <summary>
    /// Tax company name (optional)
    /// </summary>
    public string? TaxCompany { get; set; }
    
    /// <summary>
    /// Location tree address ID (optional)
    /// </summary>
    public string? LocationTreeAddressId { get; set; }
}

/// <summary>
/// Product item in order
/// </summary>
public class ProductItem
{
    /// <summary>
    /// Product ID (required)
    /// </summary>
    public long ProductId { get; set; }
    
    /// <summary>
    /// Product count/quantity (required)
    /// </summary>
    public int ProductCount { get; set; } = 1;
    
    /// <summary>
    /// SKU attributes (optional, from aliexpress.ds.product.get)
    /// </summary>
    public string? SkuAttr { get; set; }
    
    /// <summary>
    /// Logistics service name (optional, from aliexpress.ds.freight.query)
    /// </summary>
    public string? LogisticsServiceName { get; set; }
    
    /// <summary>
    /// Order memo (optional)
    /// </summary>
    public string? OrderMemo { get; set; }
}
