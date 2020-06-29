using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IProductVisitorMappingService
    {
        void InsertEntity(ProductVisitorMapping entity);

        void DeleteEntity(ProductVisitorMapping entity);

        void DeleteEntities(IList<ProductVisitorMapping> entities);

        void UpdateEntity(ProductVisitorMapping entity);

        void UpdateEntities(IList<ProductVisitorMapping> entities);

        ProductVisitorMapping GetEntitiesByProductId(int productId);

        List<int> GetVisitorIdsByProductId(int productId);

    }
}