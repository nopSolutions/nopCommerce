using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.Infrastructure;

namespace Nop.Plugin.Api.Services
{
    public class ProductWarehouseInventoriesApiService : IProductWarehouseInventoriesApiService
    {
        private readonly IRepository<ProductWarehouseInventory> _productWarehouseRepository;
        public ProductWarehouseInventoriesApiService(IRepository<ProductWarehouseInventory> productWarehouseRepository)
        {
            _productWarehouseRepository = productWarehouseRepository;
        }

        public IList<ProductWarehouseInventory> GetMappings(
            int? productId = null,
            int? categoryId = null, int limit = Constants.Configurations.DefaultLimit,
            int page = Constants.Configurations.DefaultPageValue, int sinceId = Constants.Configurations.DefaultSinceId)
        {
            var query = GetInventoriesQuery(productId, categoryId, sinceId);

            return new ApiList<ProductWarehouseInventory>(query, page - 1, limit);
        }

        public int GetIvnentoriesCount(int? productId = null, int? warehouseId = null)
        {
            return GetInventoriesQuery(productId, warehouseId).Count();
        }
        
        public Task<ProductWarehouseInventory> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return _productWarehouseRepository.GetByIdAsync(id);
        }

        private IQueryable<ProductWarehouseInventory> GetInventoriesQuery(
            int? productId = null,
            int? warehouseId = null, int sinceId = Constants.Configurations.DefaultSinceId)
        {
            var query = _productWarehouseRepository.Table;

            if (productId != null)
            {
                query = query.Where(mapping => mapping.ProductId == productId);
            }

            if (warehouseId != null)
            {
                query = query.Where(mapping => mapping.WarehouseId == warehouseId);
            }

            if (sinceId > 0)
            {
                query = query.Where(mapping => mapping.Id > sinceId);
            }

            query = query.OrderBy(mapping => mapping.Id);

            return query;
        }
    }
}
