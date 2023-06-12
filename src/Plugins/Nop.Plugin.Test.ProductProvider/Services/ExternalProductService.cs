using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LinqToDB.Linq.Internal;
using Newtonsoft.Json;
using Nop.Plugin.Test.ProductProvider.Api;
using Nop.Plugin.Test.ProductProvider.Exceptions;
using Nop.Plugin.Test.ProductProvider.Mappers;
using Nop.Plugin.Test.ProductProvider.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;

namespace Nop.Plugin.Test.ProductProvider.Services;

public class ExternalProductService : IExternalProductService
{
    private readonly ProductProviderHttpClient _httpClient;
    private readonly IProductService _productService;
    private readonly IProductMapper _productMapper;
    
    public ExternalProductService(ProductProviderHttpClient httpClient, IProductMapper productMapper, IProductService productService)
    {
        _httpClient = httpClient;
        _productMapper = productMapper;
        _productService = productService;
    }

    public async Task<IEnumerable<int>> GetAllProducts()
    {
        return await _httpClient.GetProductsAsync();
    }

    public async Task<ExternalProductModel> GetProductDetails(int id)
    {
        return await _httpClient.GetProductDetailsAsync(id);
    }

    public async Task SyncProducts()
    {
        var productIds = await GetAllProducts();
        foreach (int id in productIds)
        {
            var productFromDb = await _productService.GetProductBySkuAsync(id.ToString());

            if (productFromDb == null)
            {
                var mappedModel = _productMapper.Map(await GetProductDetails(id));
                await _productService.InsertProductAsync(mappedModel);
            }
        }
    }
}