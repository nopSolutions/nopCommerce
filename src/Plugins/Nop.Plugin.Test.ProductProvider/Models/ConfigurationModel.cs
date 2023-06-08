namespace Nop.Plugin.Test.ProductProvider.Models;

public class ConfigurationModel
{
    public string BaseUrl { get; set; }
    public string GetProductsIdsEndpoint { get; set; }
    public string GetProductByIdEndpoint { get; set; }
    public string AccessToken { get; set; }
}