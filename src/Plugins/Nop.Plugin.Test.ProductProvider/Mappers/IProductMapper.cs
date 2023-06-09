using Nop.Core.Domain.Catalog;
using Nop.Plugin.Test.ProductProvider.Models;

namespace Nop.Plugin.Test.ProductProvider.Mappers;

public interface IProductMapper
{
    Product Map(ExternalProductModel model);
}