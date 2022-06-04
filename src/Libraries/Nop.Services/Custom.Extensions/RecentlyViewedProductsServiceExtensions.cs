using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Catalog
{
    public partial interface IRecentlyViewedProductsService
    {
        Task<IList<ProductCustom>> GetRelatedProductsAsync(int number);
    }

    public partial class RecentlyViewedProductsService
    {

        #region Fields

        private IWorkContext _workContext;

        #endregion

        #region Methods

        public virtual async Task<IList<ProductCustom>> GetRelatedProductsAsync(int number)
        {
            //get list of recently viewed product identifiers
            var productIds = GetRecentlyViewedProductsIds(number);

            _workContext = EngineContext.Current.Resolve<IWorkContext>();

            var customerId = (await _workContext.GetCurrentCustomerAsync()).Id;

            //return list of product
            return (await _productService.SearchProductsSimpleCustomAsync(productIds: productIds, customerId: customerId))
                .Where(product => product.Published && !product.Deleted).ToList();
        }

        #endregion
    }
}
