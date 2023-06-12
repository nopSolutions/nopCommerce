using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LinqToDB.Linq.Internal;
using Newtonsoft.Json;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Test.ProductProvider.Api;
using Nop.Plugin.Test.ProductProvider.Exceptions;
using Nop.Plugin.Test.ProductProvider.Mappers;
using Nop.Plugin.Test.ProductProvider.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.ExportImport;
using Nop.Services.Media;

namespace Nop.Plugin.Test.ProductProvider.Services;

public class ExternalProductService : IExternalProductService
{
    private readonly ProductProviderHttpClient _httpClient;
    private readonly IProductService _productService;
    private readonly IProductMapper _productMapper;
    private readonly IPictureService _pictureService;
    
    public ExternalProductService(ProductProviderHttpClient httpClient, IProductMapper productMapper, IProductService productService, IPictureService pictureService)
    {
        _httpClient = httpClient;
        _productMapper = productMapper;
        _productService = productService;
        _pictureService = pictureService;
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
                var product = await GetProductDetails(id);
                var mappedModel = _productMapper.Map(product);

                var imageUrl = product.ThumbnailUrls.FirstOrDefault();
                byte[] imageFromApi; 
                
                if (imageUrl != null)
                {
                    imageFromApi = await _httpClient.GetProductImageAsync(imageUrl);
                    var savedImage = await _pictureService.InsertPictureAsync(imageFromApi, "image/jpeg", product.Name);
                    await _productService.InsertProductAsync(mappedModel);

                    await _productService.InsertProductPictureAsync(new ProductPicture()
                    {
                        PictureId = savedImage.Id, ProductId = mappedModel.Id
                    });
                }
                else
                {
                    await _productService.InsertProductAsync(mappedModel);
                }
            }
        }
    }
}