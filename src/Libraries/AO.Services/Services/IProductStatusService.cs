using AO.Services.Domain;
using Nop.Core.Domain.Catalog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AO.Services.Services
{
    public interface IProductStatusService
    {
        Task<List<AOProductStatus>> GetAllAsync();
        Task<AOProductStatus> GetAsync(int productStatusId);
        Task<AOProductExtensionData> GetProductExtensionData(int id);
        Task SaveProductStatus(AOProductExtensionData productExtension, int oldStatusId);
        Task<IList<AOProductExtensionHistory>> GetProductStatusHistory(int productId);
        Task AddToProductStatusHistory(AOProductExtensionData productExtension, int oldStatusId);
        Task AddToProductStatusHistory(AOProductExtensionData productExtension, string comment);
        Task InsertNewProductStatus(AOProductExtensionData productExtension);

        /// <summary>
        /// Will try to find new product from old friliv product id
        /// </summary> 
        Task<Product> GetProductByOldId(int oldProductId);

        Task<List<int>> GetProductIdsWithStatus(int productStatusId);

        Task<List<int>> GetAllProductIds();
        Task SaveLastInventoryCountAsync(AOProductExtensionData productExtension);
    }
}