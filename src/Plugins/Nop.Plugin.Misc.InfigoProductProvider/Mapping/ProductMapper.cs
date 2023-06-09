using System;
using Microsoft.Extensions.Logging;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.InfigoProductProvider.Models;

namespace Nop.Plugin.Misc.InfigoProductProvider.Mapping;

public class ProductMapper : IProductMapper
{
    private readonly ILogger<ProductMapper> _logger;

    public ProductMapper(ILogger<ProductMapper> logger)
    {
        _logger = logger;
    }

    public Product GetNopProductEntity(ApiProductModel model)
    {
        _logger.LogInformation("Getting Nop product entity");
        
        return new Product
        {
            Name = model.Name,
            ShortDescription = model.ShortDescription,
            FullDescription = model.LongDescription,
            ProductTypeId = model.Type,
            Price = model.Price,
            StockQuantity = model.StockValue,
            Sku = model.Sku,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
    }

    public ProductAttribute GetNopProductAttributeEntity(ApiProductAttributeModel productAttribute)
    {
        _logger.LogInformation("Getting Nop product attribute entity");
        
        return new ProductAttribute
        {
            Name = productAttribute.Name, 
            Description = productAttribute.Description
        };
    }

    public ProductAttributeValue GetNopProductAttributeValueEntity(ApiProductAttributeValueModel productAttributeValue,
        ProductAttributeMapping nopProductAttributeMapping, Product nopProduct)
    {
        _logger.LogInformation("Getting Nop product attribute value entity");
        
        return new ProductAttributeValue
        {
            Name = productAttributeValue.Name,
            ProductAttributeMappingId = nopProductAttributeMapping.Id,
            AssociatedProductId = nopProduct.Id,
            PriceAdjustment = productAttributeValue.PriceAdjustment,
            WeightAdjustment = productAttributeValue.WeightAdjustment,
            DisplayOrder = productAttributeValue.DisplayOrder
        };
    }

    public ProductAttributeMapping GetNopProductAttributeMappingEntity(ProductAttribute nopProductAttribute,
        Product nopProduct, ApiProductAttributeModel productAttribute)
    {
        _logger.LogInformation("Getting Nop product attribute mapping entity");
        
        return new ProductAttributeMapping
        {
            ProductAttributeId = nopProductAttribute.Id,
            ProductId = nopProduct.Id,
            IsRequired = productAttribute.IsRequired,
            AttributeControlTypeId = productAttribute.AttributeControlType
        };
    }

    public SpecificationAttributeOption GetNopSpecificationAttributeOption(ApiProductModel model, int specificationAttributeId)
    {
        _logger.LogInformation("Getting Nop specification attribute option entity");
        
        return new SpecificationAttributeOption
        {
            Name = model.Id.ToString(), 
            SpecificationAttributeId = specificationAttributeId,
            DisplayOrder = 0
        };
    }

    public ProductSpecificationAttribute GetNopProductSpecificationAttribute(Product nopProduct, SpecificationAttributeOption nopSpecificationAttributeOption)
    {
        _logger.LogInformation("Getting Nop product specification attribute entity");
        
        return new ProductSpecificationAttribute
        {
            ProductId = nopProduct.Id, 
            SpecificationAttributeOptionId = nopSpecificationAttributeOption.Id,
            AttributeTypeId = 0,
            AllowFiltering = false,
            ShowOnProductPage = true
        };
    }
}