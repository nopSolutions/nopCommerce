using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;

namespace Nop.Plugin.Misc.AbcCore.Services.Custom
{
    public interface ICustomProductService : IProductService
    {
        Task<IList<Product>> GetProductsWithoutImagesAsync();
    }
}