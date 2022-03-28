using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Mattresses.Domain;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Mattresses
{
    public interface IAbcMattressProductService
    {
        List<string> GetMattressItemNos();
        Task<Product> UpsertAbcMattressProductAsync(AbcMattressModel model);
        Task SetManufacturerAsync(AbcMattressModel model, Product product);
        Task SetCategoriesAsync(AbcMattressModel model, Product product);
        Task SetProductAttributesAsync(AbcMattressModel model, Product product);
        bool IsMattressProduct(int productId);
        Task SetComfortRibbonAsync(AbcMattressModel model, Product product);
        Task SetSpecificationAttributesAsync(AbcMattressModel model, Product product);
    }
}
