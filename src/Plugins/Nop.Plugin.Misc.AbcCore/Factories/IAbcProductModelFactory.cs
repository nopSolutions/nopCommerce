using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Misc.AbcCore.Factories
{
    public interface IAbcProductModelFactory : IProductModelFactory
    {
        Task<IList<ProductDetailsModel.ProductAttributeModel>> PrepareProductAttributeModelsAsync(
            Product product,
            ShoppingCartItem updatecartitem,
            string[] attributesToInclude
        );
    }
}
