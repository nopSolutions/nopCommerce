namespace AliExpressSdk.Models.ProductDetails;

/// <summary>
/// Request model for aliexpress.ds.product.get API
/// </summary>
public class ProductDetailsRequest
{
    /// <summary>
    /// Product ID (required)
    /// </summary>
    public string ProductId { get; set; } = string.Empty;
    
    /// <summary>
    /// Ship to country code (required)
    /// </summary>
    public string ShipToCountry { get; set; } = string.Empty;
    
    /// <summary>
    /// Target currency (optional)
    /// </summary>
    public string? TargetCurrency { get; set; }
    
    /// <summary>
    /// Target language (optional, e.g., "en", "zh_CN")
    /// </summary>
    public string? TargetLanguage { get; set; }
    
    /// <summary>
    /// Remove personal benefit (optional)
    /// </summary>
    public bool? RemovePersonalBenefit { get; set; }
    
    /// <summary>
    /// Business model (optional)
    /// </summary>
    public string? BizModel { get; set; }
    
    /// <summary>
    /// Province code (optional)
    /// </summary>
    public string? ProvinceCode { get; set; }
    
    /// <summary>
    /// City code (optional)
    /// </summary>
    public string? CityCode { get; set; }
    
    /// <summary>
    /// Convert to dictionary for API call
    /// </summary>
    public Dictionary<string, string> ToDictionary()
    {
        var dict = new Dictionary<string, string>
        {
            ["product_id"] = ProductId,
            ["ship_to_country"] = ShipToCountry
        };
        
        if (!string.IsNullOrWhiteSpace(TargetCurrency))
            dict["target_currency"] = TargetCurrency;
            
        if (!string.IsNullOrWhiteSpace(TargetLanguage))
            dict["target_language"] = TargetLanguage;
            
        if (RemovePersonalBenefit.HasValue)
            dict["remove_personal_benefit"] = RemovePersonalBenefit.Value.ToString().ToLower();
            
        if (!string.IsNullOrWhiteSpace(BizModel))
            dict["biz_model"] = BizModel;
            
        if (!string.IsNullOrWhiteSpace(ProvinceCode))
            dict["province_code"] = ProvinceCode;
            
        if (!string.IsNullOrWhiteSpace(CityCode))
            dict["city_code"] = CityCode;
        
        return dict;
    }
}
