using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Infrastructure;

namespace Nop.Plugin.Api.Services;

public interface IProductWarehouseInventoriesApiService
{
    IList<ProductWarehouseInventory> GetMappings(
        int? productId = null,
        int? categoryId = null, int limit = Constants.Configurations.DefaultLimit,
        int page = Constants.Configurations.DefaultPageValue, int sinceId = Constants.Configurations.DefaultSinceId);

    int GetIvnentoriesCount(int? productId = null, int? warehouseId = null);

    Task<ProductWarehouseInventory> GetByIdAsync(int id);
}