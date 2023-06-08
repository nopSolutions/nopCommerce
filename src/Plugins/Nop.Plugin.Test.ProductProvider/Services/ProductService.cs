using System.Net.Http;
using System.Threading.Tasks;
using Nop.Services.Configuration;

namespace Nop.Plugin.Test.ProductProvider.Services;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;
    private readonly ISettingService _settingService;

    public ProductService(HttpClient httpClient, ISettingService settingService)
    {
        _httpClient = httpClient;
        _settingService = settingService;
    }

    public async Task GetAllProducts()
    {
        var settings = await _settingService.LoadSettingAsync<ProductProviderSettings>();

        var productIds = _httpClient.GetAsync($"{settings.BaseUrl}/{settings.GetProductsIdsEndpoint}");
        var a = 5;
    }
}