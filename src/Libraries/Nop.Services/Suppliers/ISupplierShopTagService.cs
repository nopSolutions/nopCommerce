using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Suppliers;

namespace Nop.Services.Suppliers
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface ISupplierShopTagService
    {
        void InsertEntity(SupplierShopTag entity);

        void DeleteEntity(SupplierShopTag entity, bool delete = false);

        void DeleteEntities(IList<SupplierShopTag> entities, bool deleted = false);

        void UpdateEntity(SupplierShopTag entity);

        void UpdateEntities(IList<SupplierShopTag> entities);

        SupplierShopTag GetEntityById(int id);

        List<SupplierShopTag> GetEntitiesByIds(int[] entityIds);

        IPagedList<SupplierShopTag> GetEntities(
            string name = "",
            bool? published = null,
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}