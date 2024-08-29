using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;

namespace Nop.Plugin.Misc.AbcCore.Services.Custom
{
    public interface IAbcProductService : IProductService
    {
        Task<IList<Product>> GetAllPublishedProductsAsync();

        Task<IList<Product>> GetProductsWithoutImagesAsync();
        Task<IList<Product>> GetNewProductsAsync();
    }
}