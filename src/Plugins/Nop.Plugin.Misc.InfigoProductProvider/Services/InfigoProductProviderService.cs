using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.InfigoProductProvider.Api;
using Nop.Plugin.Misc.InfigoProductProvider.Mapping;
using Nop.Plugin.Misc.InfigoProductProvider.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Media;

namespace Nop.Plugin.Misc.InfigoProductProvider.Services;

public class InfigoProductProviderService : IInfigoProductProviderService
{
    private readonly InfigoProductProviderHttpClient _infigoProductProviderHttpClient;
    private readonly ISettingService _settingService;
    private readonly IProductAttributeService _productAttributeService;
    private readonly IProductService _productService;
    private readonly IProductMapper _productMapper;
    private readonly ISpecificationAttributeService _specificationAttributeService;
    private readonly IPictureService _pictureService;
    private readonly ILogger<InfigoProductProviderService> _logger;

    public InfigoProductProviderService(InfigoProductProviderHttpClient infigoProductProviderHttpClient, ISettingService settingService, IProductAttributeService productAttributeService, IProductService productService, IProductMapper productMapper, ISpecificationAttributeService specificationAttributeService, IPictureService pictureService, ILogger<InfigoProductProviderService> logger)
    {
        _infigoProductProviderHttpClient = infigoProductProviderHttpClient;
        _settingService = settingService;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _productMapper = productMapper;
        _specificationAttributeService = specificationAttributeService;
        _pictureService = pictureService;
        _logger = logger;
    }

    public async Task GetApiProducts()
    {
        var specificationAttributeId = await GetSpecificationAttributeIdForExternalId();
        var existingExternalSpecifications =
            (await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(
                specificationAttributeId));

        var productIds = await GetAllProductsIds();

        foreach (var productId in productIds)
        {
            if (existingExternalSpecifications.Select(x => x.Name).Contains(productId.ToString()))
            {
                _logger.LogInformation("Product {id} already exists. Updating", productId);
                
                var productModel = await GetProductById(productId);

                await SetNewProductValues(productModel, existingExternalSpecifications);
            }
            else
            {
                _logger.LogInformation("New product {id} detected. Adding new product", productId);
                
                var productModel = await GetProductById(productId);

                await SaveApiProductsInDb(productModel);
            }
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
        var nopProduct = _productMapper.GetNopProductEntity(model);
        await _productService.InsertProductAsync(nopProduct);
        
        await SetPicture(model, nopProduct);
        
        await SetExternalId(model, nopProduct);
        
        await SetProductAttributes(model, nopProduct);
    }

    private async Task SetPicture(ApiProductModel model, Product nopProduct)
    {
        _logger.LogInformation("Setting picture for product {id}", model.Id);
        
        var pictureUrl = model.ThumbnailUrls.FirstOrDefault();
        if (pictureUrl == null)
        {
            _logger.LogInformation("Product {id} doesn't have it's own picture", model.Id);
            
            if (InfigoProductProviderDefaults.DefaultPictureUrl == null)
            {
                return;
            }
            
            _logger.LogInformation($"Setting default picture for product {model.Id}");
            
            pictureUrl = InfigoProductProviderDefaults.DefaultPictureUrl;
        }
        
        var pictureData = await _infigoProductProviderHttpClient.GetPictureBinaryAsync(pictureUrl);
        
        var picture = await _pictureService.InsertPictureAsync(pictureData, "image/webp", model.Name, model.Name, model.Name);

        var nopProductPicture = new ProductPicture { PictureId = picture.Id, ProductId = nopProduct.Id };
        await _productService.InsertProductPictureAsync(nopProductPicture);
    }

    private async Task SetExternalId(ApiProductModel model, Product nopProduct)
    {
        _logger.LogInformation("Setting external Id for product {id}", model.Id);
        
        var specificationAttributeId = await GetSpecificationAttributeIdForExternalId();

        var nopSpecificationAttributeOption =
            _productMapper.GetNopSpecificationAttributeOption(model, specificationAttributeId);
        await _specificationAttributeService.InsertSpecificationAttributeOptionAsync(nopSpecificationAttributeOption);

        var nopProductSpecificationAttribute =
            _productMapper.GetNopProductSpecificationAttribute(nopProduct, nopSpecificationAttributeOption);
        await _specificationAttributeService.InsertProductSpecificationAttributeAsync(nopProductSpecificationAttribute);
    }

    private async Task SetProductAttributes(ApiProductModel model, Product nopProduct)
    {
        _logger.LogInformation("Setting product attributes for product {id}", model.Id);
        
        foreach (var productAttribute in model.ProductAttributes)
        {
            var nopProductAttribute = _productMapper.GetNopProductAttributeEntity(productAttribute);
            
            await _productAttributeService.InsertProductAttributeAsync(nopProductAttribute);
            
            var nopProductAttributeMapping =
                _productMapper.GetNopProductAttributeMappingEntity(nopProductAttribute, nopProduct,
                    productAttribute);

            await _productAttributeService.InsertProductAttributeMappingAsync(nopProductAttributeMapping);
            
            foreach (var productAttributeValue in productAttribute.ProductAttributeValues)
            {
                var nopProductAttributeValue =
                    _productMapper.GetNopProductAttributeValueEntity(productAttributeValue, nopProductAttributeMapping,
                        nopProduct);

                await _productAttributeService.InsertProductAttributeValueAsync(nopProductAttributeValue);
            }
        }
    }

    private async Task SetNewProductValues(ApiProductModel model, IList<SpecificationAttributeOption> existingExternalSpecifications)
    {
        _logger.LogInformation("Checking Product {id} values for update", model.Id);
        
        var nopSpecificationAttributeOptionId = existingExternalSpecifications.FirstOrDefault(x => x.Name == model.Id.ToString()).Id;

        var nopProductId = (await _specificationAttributeService.GetProductSpecificationAttributesAsync())
            .FirstOrDefault(x => x.SpecificationAttributeOptionId == nopSpecificationAttributeOptionId).ProductId;

        var nopProduct = await _productService.GetProductByIdAsync(nopProductId);

        if (nopProduct.Price != model.Price)
        {
            _logger.LogInformation("Price change detected. Updating price");
            
            nopProduct.Price = model.Price;
            
            await _productService.UpdateProductAsync(nopProduct);
        }
    }

    private async Task<int> GetSpecificationAttributeIdForExternalId()
    {
        var specificationAttributeId =
            (await _specificationAttributeService.GetSpecificationAttributesAsync()).FirstOrDefault(sa =>
                sa.Name == InfigoProductProviderDefaults.SpecificationAttributeForExternalId).Id;

        return specificationAttributeId;
    }
}