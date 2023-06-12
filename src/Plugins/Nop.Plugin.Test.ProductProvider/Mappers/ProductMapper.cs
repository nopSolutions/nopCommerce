using Nop.Core.Domain.Catalog;
using Nop.Plugin.Test.ProductProvider.Models;

namespace Nop.Plugin.Test.ProductProvider.Mappers;

public class ProductMapper : IProductMapper
{
    public Product Map(ExternalProductModel model)
    {
        return new Product()
        {
            Name = model.Name,
            ShortDescription = model.ShortDescrption,
            FullDescription = model.LongDescription,
            ProductTypeId = model.Type,
            StockQuantity = model.StockValue,
            Price = model.Price,
            Sku = model.Id.ToString()
        };
    }
}