using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.Infrastructure;

namespace Nop.Plugin.Api.Services
{
    public class WarehouseApiService : IWarehouseApiService
    {
        private readonly IRepository<Warehouse> _warehouseRepository;
        private readonly IRepository<ProductWarehouseInventory> _productWarehouseInventoryRepository;

        public WarehouseApiService(
            IRepository<Warehouse> warehouseRepository,
            IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository)
        {
            _warehouseRepository = warehouseRepository;
            _productWarehouseInventoryRepository = productWarehouseInventoryRepository;
        }

        public IList<Warehouse> GetWarehouses(IList<int> ids = null,
            int? productId = null)
        {
            var query = GetWarehousesQuery(productId, ids);

            return new ApiList<Warehouse>(query);
        }

        public Warehouse GetWarehouseById(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var warehouse = _warehouseRepository.Table.FirstOrDefault(cat => cat.Id == id);

            return warehouse;
        }

        private IQueryable<Warehouse> GetWarehousesQuery(int? productId, IList<int> ids)
        {
            var query = _warehouseRepository.Table;

            if (ids is { Count: > 0 })
            {
                query = query.Where(c => ids.Contains(c.Id));
            }

            if (productId != null)
            {
                var productWarehouseInventoryForProduct = from productWarehouseInventory in _productWarehouseInventoryRepository.Table
                                                          where productWarehouseInventory.ProductId == productId
                                                          select productWarehouseInventory;

                query = from warehouse in query
                        join productWarehouseInventory in productWarehouseInventoryForProduct on warehouse.Id equals productWarehouseInventory.WarehouseId
                        select warehouse;
            }

            query = query.OrderBy(warehouse => warehouse.Id);

            return query;
        }
    }
}
