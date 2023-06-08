using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Nop.Plugin.Test.ProductProvider.Models;
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
        var externalUrlData = new ConfigurationModel();
        externalUrlData.BaseUrl = await _settingService.GetSettingByKeyAsync<string>(externalUrlData.BaseUrlKey);
        externalUrlData.GetProductsIdsEndpoint = await _settingService.GetSettingByKeyAsync<string>(externalUrlData.GetProductsIdsEndpointKey);
        externalUrlData.GetProductByIdEndpoint = await _settingService.GetSettingByKeyAsync<string>(externalUrlData.GetProductByIdEndpointKey);
        
        var response = _httpClient.GetAsync($"{externalUrlData.BaseUrl}/{externalUrlData.GetProductsIdsEndpoint}");
    }

    public ExternalProductModel GetProductById(int id)
    {
        throw new System.NotImplementedException();
    }
}