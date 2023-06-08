using Nop.Core.Configuration;

namespace Nop.Plugin.Test.ProductProvider;

public class ProductProviderSettings : ISettings
{
    public string BaseUrl { get; set; }
    public string GetProductsIdsEndpoint { get; set; }
    public string GetProductByIdEndpoint { get; set; }
    public string AccessToken { get; set; }
}