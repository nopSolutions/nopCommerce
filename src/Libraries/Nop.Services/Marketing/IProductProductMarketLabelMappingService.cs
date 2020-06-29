using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IProductProductMarketLabelMappingService
    {
        void InsertEntity(ProductProductMarketLabelMapping entity);

        void DeleteEntity(ProductProductMarketLabelMapping entity);

        void DeleteEntities(IList<ProductProductMarketLabelMapping> entities);

        void UpdateEntity(ProductProductMarketLabelMapping entity);

        void UpdateEntities(IList<ProductProductMarketLabelMapping> entities);

        ProductProductMarketLabelMapping GetEntityById(int productMarketLabelId, int productId);

        List<ProductProductMarketLabelMapping> GetEntitiesByProductMarketLabelId(int productMarketLabelId);

        List<ProductProductMarketLabelMapping> GetEntitiesByProductId(int productId);
    }
}