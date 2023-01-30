using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;

namespace Nop.Plugin.Misc.AbcCore.Nop
{
    public interface IAbcProductAttributeService : IProductAttributeService
    {
        Task SaveProductAttributeAsync(ProductAttribute pa);
        Task<IList<ProductAttributeMapping>> SaveProductAttributeMappingsAsync(int productId, IList<ProductAttributeMapping> pams, string[] excludedPas);

        Task<ProductAttribute> GetProductAttributeByNameAsync(string name);

        Task<bool> ProductHasDeliveryOptionsAsync(int productId);
    }
}