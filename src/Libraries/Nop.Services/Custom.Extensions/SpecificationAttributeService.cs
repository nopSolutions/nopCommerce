using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.Catalog
{
    public partial interface ISpecificationAttributeService
    {
        Task<IList<ProductSpecificationAttribute>> GetProductSpecificationsBySpecificationAttributeIdAsync(int productId, int specificationAttributeId);
    }

    public partial class SpecificationAttributeService
    {
        #region Fields

        #endregion

        #region Methods

        public virtual async Task<IList<ProductSpecificationAttribute>> GetProductSpecificationsBySpecificationAttributeIdAsync(int productId, int specificationAttributeId)
        {
            var query = from psam in _productSpecificationAttributeRepository.Table
                        join spao in _specificationAttributeOptionRepository.Table on psam.SpecificationAttributeOptionId equals spao.Id
                        join psa in _specificationAttributeRepository.Table on spao.SpecificationAttributeId equals psa.Id
                        where spao.SpecificationAttributeId == specificationAttributeId
                        orderby psam.Id
                        select psam;

            query = query.Where(psam => psam.ProductId == productId);
            return await query.ToListAsync();
        }

        #endregion
    }
}
