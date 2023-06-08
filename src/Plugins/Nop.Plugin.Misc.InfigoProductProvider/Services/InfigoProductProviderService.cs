using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.InfigoProductProvider.Api;
using Nop.Plugin.Misc.InfigoProductProvider.Mapping;
using Nop.Plugin.Misc.InfigoProductProvider.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;

namespace Nop.Plugin.Misc.InfigoProductProvider.Services;

public class InfigoProductProviderService : IInfigoProductProviderService
{
    private readonly InfigoProductProviderHttpClient _infigoProductProviderHttpClient;
    private readonly ISettingService _settingService;
    private readonly IProductAttributeService _productAttributeService;
    private readonly IProductService _productService;
    private readonly IProductMappingService _productMappingService;

    public InfigoProductProviderService(InfigoProductProviderHttpClient infigoProductProviderHttpClient, ISettingService settingService, IProductAttributeService productAttributeService, IProductService productService, IProductMappingService productMappingService)
    {
        _infigoProductProviderHttpClient = infigoProductProviderHttpClient;
        _settingService = settingService;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _productMappingService = productMappingService;
    }

    public async Task GetApiProducts()
    {
        var productIds = await GetAllProductsIds();
        foreach (var productId in productIds)
        {
            var productModel = await GetProductById(productId);

            await SaveApiProductsInDb(productModel);
        }
    }
    
    private async Task<List<int>> GetAllProductsIds()
    {
        var settings = await _settingService.LoadSettingAsync<InfigoProductProviderConfiguration>();
        var url = settings.ApiBase + settings.ProductListUrl;
        var userName = settings.ApiUserName;
        
        var data = await _infigoProductProviderHttpClient.RequestAsync(url, userName);
        
        var productIdList = JsonConvert.DeserializeObject<List<int>>(data);
        
        return productIdList;
    }

    private async Task<ApiProductModel> GetProductById(int id)
    {
        var settings = await _settingService.LoadSettingAsync<InfigoProductProviderConfiguration>();
        var url = settings.ApiBase + settings.ProductDetailsUrl + $"?id={id}";
        var userName = settings.ApiUserName;

        var data = await _infigoProductProviderHttpClient.RequestAsync(url, userName);

        var product = JsonConvert.DeserializeObject<ApiProductModel>(data);

        return product;
    }
    
    private async Task SaveApiProductsInDb(ApiProductModel model)
    {
        var nopProduct = _productMappingService.GetNopProductEntity(model);
        await _productService.InsertProductAsync(nopProduct);

        foreach (var productAttribute in model.ProductAttributes)
        {
            var nopProductAttribute = _productMappingService.GetNopProductAttributeEntity(productAttribute);
            
            await _productAttributeService.InsertProductAttributeAsync(nopProductAttribute);
            
            foreach (var productAttributeValue in productAttribute.ProductAttributeValues)
            {
                var nopProductAttributeValue =
                    _productMappingService.GetNopProductAttributeValueEntity(productAttributeValue, nopProductAttribute,
                        nopProduct);

                await _productAttributeService.InsertProductAttributeValueAsync(nopProductAttributeValue);
            }

            var nopProductAttributeMapping =
                _productMappingService.GetNopProductAttributeMappingEntity(nopProductAttribute, nopProduct,
                    productAttribute);

            await _productAttributeService.InsertProductAttributeMappingAsync(nopProductAttributeMapping);
        }
    }
}