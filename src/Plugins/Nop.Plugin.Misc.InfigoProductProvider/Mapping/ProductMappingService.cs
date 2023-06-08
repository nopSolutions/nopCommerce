using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.InfigoProductProvider.Models;

namespace Nop.Plugin.Misc.InfigoProductProvider.Mapping;

public class ProductMappingService : IProductMappingService
{
    public Product GetNopProductEntity(ApiProductModel model)
    {
        return new Product
        {
            Name = model.Name,
            ShortDescription = model.ShortDescription,
            FullDescription = model.LongDescription,
            ProductTypeId = model.Type,
            Price = model.Price,
            StockQuantity = model.StockValue,
            Sku = model.Sku
        };
    }

    public ProductAttribute GetNopProductAttributeEntity(ApiProductAttributeModel productAttribute)
    {
        return new ProductAttribute
        {
            Name = productAttribute.Name, 
            Description = productAttribute.Description,
        };
    }

    public ProductAttributeValue GetNopProductAttributeValueEntity(ApiProductAttributeValueModel productAttributeValue,
        ProductAttribute nopProductAttribute, Product nopProduct)
    {
        return new ProductAttributeValue
        {
            Name = productAttributeValue.Name,
            ProductAttributeMappingId = nopProductAttribute.Id,
            AssociatedProductId = nopProduct.Id,
            PriceAdjustment = productAttributeValue.PriceAdjustment,
            WeightAdjustment = productAttributeValue.WeightAdjustment
        };
    }

    public ProductAttributeMapping GetNopProductAttributeMappingEntity(ProductAttribute nopProductAttribute,
        Product nopProduct, ApiProductAttributeModel productAttribute)
    {
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
        return new SpecificationAttributeOption
        {
            Name = model.Id.ToString(), 
            SpecificationAttributeId = specificationAttributeId,
            DisplayOrder = 0
        };
    }

    public ProductSpecificationAttribute GetNopProductSpecificationAttribute(Product nopProduct, SpecificationAttributeOption nopSpecificationAttributeOption)
    {
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