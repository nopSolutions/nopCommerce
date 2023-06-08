using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Plugin.Test.ProductProvider.Api;
using Nop.Services.Configuration;

namespace Nop.Plugin.Test.ProductProvider.Services;

public class ProductService : IProductService
{
    private readonly ISettingService _settingService;
    private readonly ProductProviderHttpClient _httpClient;

    public ProductService(ProductProviderHttpClient httpClient, ISettingService settingService)
    {
        _httpClient = httpClient;
        _settingService = settingService;
    }

    public async Task<IEnumerable<int>> GetAllProducts()
    {
        var settings = await _settingService.LoadSettingAsync<ProductProviderSettings>();

        var url = $"{settings.BaseUrl}/{settings.ProductListEndpoint}";
        
        var response = await _httpClient.RequestAsync(url, settings);
        
        return JsonConvert.DeserializeObject<IEnumerable<int>>(response);
    }

    public async Task GetProductDetails(int id)
    {
        var settings = await _settingService.LoadSettingAsync<ProductProviderSettings>();

        var url = $"{settings.BaseUrl}/{settings.ProductDetailEndpoint}?id={id}";

        await _httpClient.RequestAsync(url, settings);
    }
}