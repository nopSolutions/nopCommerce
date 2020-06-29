using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Suppliers;

namespace Nop.Services.Suppliers
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface ISupplierImageService
    {
        void InsertEntity(SupplierImage entity);

        void DeleteEntity(SupplierImage entity, bool delete = false);

        void DeleteEntities(IList<SupplierImage> entities, bool deleted = false);

        void UpdateEntity(SupplierImage entity);

        void UpdateEntities(IList<SupplierImage> entities);

        SupplierImage GetEntityById(int id);

        List<SupplierImage> GetEntitiesByIds(int[] entityIds);

        IPagedList<SupplierImage> GetEntities(
            int supplierShopId = 0,
            int supplierImageTypeId = 0,
            bool? published = null,
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}