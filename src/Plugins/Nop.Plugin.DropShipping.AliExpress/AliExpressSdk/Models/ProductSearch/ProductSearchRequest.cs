namespace AliExpressSdk.Models.ProductSearch;

/// <summary>
/// Request model for aliexpress.ds.text.search API
/// </summary>
public class ProductSearchRequest
{
    /// <summary>
    /// Search keyword
    /// </summary>
    public string? KeyWord { get; set; }
    
    /// <summary>
    /// Locale/Language (e.g., "en_US", "zh_CN")
    /// </summary>
    public string Local { get; set; } = "en_US";
    
    /// <summary>
    /// Ship to country code
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Category ID (optional)
    /// </summary>
    public string? CategoryId { get; set; }
    
    /// <summary>
    /// Sort by (e.g., "min_price,asc", "orders,desc")
    /// </summary>
    public string? SortBy { get; set; }
    
    /// <summary>
    /// Page size (default: 20)
    /// </summary>
    public int PageSize { get; set; } = 20;
    
    /// <summary>
    /// Page index (default: 1)
    /// </summary>
    public int PageIndex { get; set; } = 1;
    
    /// <summary>
    /// Currency code (e.g., "USD", "ZAR")
    /// </summary>
    public string Currency { get; set; } = "USD";
    
    /// <summary>
    /// Search extend parameters (optional)
    /// </summary>
    public string? SearchExtend { get; set; }
    
    /// <summary>
    /// Selection name (optional)
    /// </summary>
    public string? SelectionName { get; set; }
    
    /// <summary>
    /// Convert to dictionary for API call
    /// </summary>
    public Dictionary<string, string> ToDictionary()
    {
        var dict = new Dictionary<string, string>
        {
            ["local"] = Local,
            ["countryCode"] = CountryCode,
            ["currency"] = Currency,
            ["pageSize"] = PageSize.ToString(),
            ["pageIndex"] = PageIndex.ToString()
        };
        
        if (!string.IsNullOrWhiteSpace(KeyWord))
            dict["keyWord"] = KeyWord;
            
        if (!string.IsNullOrWhiteSpace(CategoryId))
            dict["categoryId"] = CategoryId;
            
        if (!string.IsNullOrWhiteSpace(SortBy))
            dict["sortBy"] = SortBy;
            
        if (!string.IsNullOrWhiteSpace(SearchExtend))
            dict["searchExtend"] = SearchExtend;
            
        if (!string.IsNullOrWhiteSpace(SelectionName))
            dict["selectionName"] = SelectionName;
        
        return dict;
    }
}
