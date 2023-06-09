namespace Nop.Plugin.Test.ProductProvider.Models;

public class ConfigurationModel
{
    public string BaseUrl { get; set; }
    public string ProductListEndpoint { get; set; }
    public string ProductDetailsEndpoint { get; set; }
    public string ApiKey { get; set; }
    public string ApiKeyType { get; set; }
}