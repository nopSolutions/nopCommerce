using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Suppliers;

namespace Nop.Services.Suppliers
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface ISupplierUserAuthorityMappingService
    {
        void InsertEntity(SupplierUserAuthorityMapping entity);

        void DeleteEntity(SupplierUserAuthorityMapping entity, bool delete = false);

        void DeleteEntities(IList<SupplierUserAuthorityMapping> entities, bool deleted = false);

        void UpdateEntity(SupplierUserAuthorityMapping entity);

        void UpdateEntities(IList<SupplierUserAuthorityMapping> entities);

        SupplierUserAuthorityMapping GetEntityById(int id);

        SupplierUserAuthorityMapping GetEntityByUserId(int userId);

        List<SupplierUserAuthorityMapping> GetEntitiesByIds(int[] entityIds);

        List<SupplierUserAuthorityMapping> GetEntitiesBySupplierId(int supplierId, int supplierShopId = 0);

        IPagedList<SupplierUserAuthorityMapping> GetEntities(
            int supplierId = 0,
            int supplierShopId = 0,
            bool? financialManager = null,
            bool? verifyManager = null,
            bool? orderConfirmer = null,
            bool? productPulisher = null,
            bool? published = null,
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}