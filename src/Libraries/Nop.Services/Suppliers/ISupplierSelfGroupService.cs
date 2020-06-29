using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Suppliers;

namespace Nop.Services.Suppliers
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface ISupplierSelfGroupService
    {
        void InsertEntity(SupplierSelfGroup entity);

        void DeleteEntity(SupplierSelfGroup entity, bool delete = false);

        void DeleteEntities(IList<SupplierSelfGroup> entities, bool deleted = false);

        void UpdateEntity(SupplierSelfGroup entity);

        void UpdateEntities(IList<SupplierSelfGroup> entities);

        SupplierSelfGroup GetEntityById(int id);

        List<SupplierSelfGroup> GetEntitiesBySupplierId(int supplierId);

        List<SupplierSelfGroup> GetEntitiesByIds(int[] entityIds);

        IPagedList<SupplierSelfGroup> GetEntities(
            int supplierId = 0,
            bool? published = null,
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}