namespace Nop.Plugin.Test.ProductProvider.Models;

public class ConfigurationModel
{
    public string BaseUrlKey { get; } = "BaseUrlKey";
    public string BaseUrl { get; set; }
    
    public string GetProductsIdsEndpointKey { get; } = "GetProductsIdsEndpointKey";
    public string GetProductsIdsEndpoint { get; set; }
    
    public string GetProductByIdEndpointKey { get; } = "GetProductByIdEndpointKey";
    public string GetProductByIdEndpoint { get; set; }
}