using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Suppliers;

namespace Nop.Services.Suppliers
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface ISupplierShopUserFollowMappingService
    {
        void InsertEntity(SupplierShopUserFollowMapping entity);

        void DeleteEntity(SupplierShopUserFollowMapping entity);

        void DeleteEntities(IList<SupplierShopUserFollowMapping> entities);

        void UpdateEntity(SupplierShopUserFollowMapping entity);

    }
}