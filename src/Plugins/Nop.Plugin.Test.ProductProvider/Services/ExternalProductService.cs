using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LinqToDB.Linq.Internal;
using Newtonsoft.Json;
using Nop.Plugin.Test.ProductProvider.Api;
using Nop.Plugin.Test.ProductProvider.Mappers;
using Nop.Plugin.Test.ProductProvider.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;

namespace Nop.Plugin.Test.ProductProvider.Services;

public class ExternalProductService : IExternalProductService
{
    private readonly ISettingService _settingService;
    private readonly ProductProviderHttpClient _httpClient;
    private readonly IProductService _productService;
    private readonly IProductMapper _productMapper;

    public ExternalProductService(ProductProviderHttpClient httpClient, ISettingService settingService, IProductMapper productMapper, IProductService productService)
    {
        _httpClient = httpClient;
        _settingService = settingService;
        _productMapper = productMapper;
        _productService = productService;
    }

    public async Task<IEnumerable<int>> GetAllProducts()
    {
        var settings = await _settingService.LoadSettingAsync<ProductProviderSettings>();

        var url = $"{settings.BaseUrl}/{settings.ProductListEndpoint}";
        
        var response = await _httpClient.RequestAsync(url, settings);
        
        return JsonConvert.DeserializeObject<IEnumerable<int>>(response);
    }

    public async Task<ExternalProductModel> GetProductDetails(int id)
    {
        var settings = await _settingService.LoadSettingAsync<ProductProviderSettings>();

        var url = $"{settings.BaseUrl}/{settings.ProductDetailEndpoint}?id={id}";

        var result = await _httpClient.RequestAsync(url, settings);

        return JsonConvert.DeserializeObject<ExternalProductModel>(result);
    }

    public async Task SyncProducts()
    {
        var productIds = await GetAllProducts();
        foreach (int id in productIds)
        {
             
            var mappedModel = _productMapper.Map(await GetProductDetails(id));
            await _productService.InsertProductAsync(mappedModel);
        }
    }
}