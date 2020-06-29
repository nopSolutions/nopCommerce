using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Suppliers;

namespace Nop.Services.Suppliers
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface ISupplierShopService
    {
        void InsertEntity(SupplierShop entity);

        void DeleteEntity(SupplierShop entity, bool delete = false);

        void DeleteEntities(IList<SupplierShop> entities, bool deleted = false);

        void UpdateEntity(SupplierShop entity);

        void UpdateEntities(IList<SupplierShop> entities);

        SupplierShop GetEntityById(int id);

        List<SupplierShop> GetEntitiesByIds(int[] entityIds);

        List<SupplierShop> GetEntitiesBySupplierId(int supplierId);

        IPagedList<SupplierShop> GetEntities(
            int supplierId = 0,
            string name = "",
            bool? published = null,
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}