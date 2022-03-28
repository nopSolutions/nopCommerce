using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Domain;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public interface IProductAbcDescriptionService
    {
        Task<ProductAbcDescription> GetProductAbcDescriptionByProductIdAsync(int productId);
        Task<ProductAbcDescription> GetProductAbcDescriptionByAbcItemNumberAsync(string abcitemNumber);
    }
}