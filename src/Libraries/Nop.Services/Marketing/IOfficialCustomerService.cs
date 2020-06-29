using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IOfficialCustomerService
    {
        void InsertEntity(OfficialCustomer entity);

        void DeleteEntity(OfficialCustomer entity, bool delete = false);

        void DeleteEntities(IList<OfficialCustomer> entities, bool deleted = false);

        void UpdateEntity(OfficialCustomer entity);

        void UpdateEntities(IList<OfficialCustomer> entities);

        OfficialCustomer GetEntityById(int id);

        List<int> GetCategoryIdsById(int id);

        List<OfficialCustomer> GetEntitiesByIds(int[] entityIds);

        IPagedList<OfficialCustomer> GetEntities(
            int storeId = 0,
            bool? published = null,
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}