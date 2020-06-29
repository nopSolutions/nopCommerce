using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Suppliers;

namespace Nop.Services.Suppliers
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface ISupplierService
    {
        void InsertEntity(Supplier entity);

        void DeleteEntity(Supplier entity, bool delete = false);

        void DeleteEntities(IList<Supplier> entities, bool deleted = false);

        void UpdateEntity(Supplier entity);

        void UpdateEntities(IList<Supplier> entities);

        Supplier GetEntityById(int id);

        List<Supplier> GetEntitiesByIds(int[] entityIds);

        List<Supplier> GetEntitiesByStoreId(int stroeId);

        IPagedList<Supplier> GetEntities(
            string name = "",
            int storeId = 0,
            DateTime? startDateTimeUtc = null,
            DateTime? endDateTimeUtc = null,
            bool? isPersonal = null,
            bool? published = null,
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}