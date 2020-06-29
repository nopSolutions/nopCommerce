using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IProductMarketLabelService
    {
        void InsertEntity(ProductMarketLabel entity);

        void DeleteEntity(ProductMarketLabel entity, bool delete = false);

        void DeleteEntities(IList<ProductMarketLabel> entities, bool deleted = false);

        void UpdateEntity(ProductMarketLabel entity);

        void UpdateEntities(IList<ProductMarketLabel> entities);

        ProductMarketLabel GetEntityById(int id);

        List<ProductMarketLabel> GetEntitiesByIds(int[] entityIds);

        IPagedList<ProductMarketLabel> GetEntities(
            bool? published = null,
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}