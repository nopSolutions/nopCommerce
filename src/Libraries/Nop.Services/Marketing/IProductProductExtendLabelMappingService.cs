using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Marketing;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IProductProductExtendLabelMappingService
    {
        void InsertEntity(ProductProductExtendLabelMapping entity);

        void DeleteEntity(ProductProductExtendLabelMapping entity);

        void UpdateEntity(ProductProductExtendLabelMapping entity);

        ProductProductExtendLabelMapping GetEntityByProductId(int productId);
        List<int> GetProductExtendLabelIdsByProductId(int productId);

    }
}