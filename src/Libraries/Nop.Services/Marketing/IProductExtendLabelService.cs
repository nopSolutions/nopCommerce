using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IProductExtendLabelService
    {
        void InsertEntity(ProductExtendLabel entity);

        void DeleteEntity(ProductExtendLabel entity);

        void DeleteEntities(IList<ProductExtendLabel> entities);

        void UpdateEntity(ProductExtendLabel entity);

        void UpdateEntities(IList<ProductExtendLabel> entities);

        ProductExtendLabel GetEntityById(int id);

        List<ProductExtendLabel> GetEntitiesByIds(int[] entityIds);

        IPagedList<ProductExtendLabel> GetEntities(
            int storeId = 0,
            int categoryId = 0,
            bool? published = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}