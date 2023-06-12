using Nop.Core.Configuration;

namespace Nop.Plugin.Test.ProductProvider;

public class ProductProviderSettings : ISettings
{
    public string BaseUrl { get; set; }
    public string ProductListEndpoint { get; set; }
    public string ProductDetailEndpoint { get; set; }
    public string ApiKey { get; set; }
}