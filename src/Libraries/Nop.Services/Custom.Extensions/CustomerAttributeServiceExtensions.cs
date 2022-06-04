using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Customers
{
    public partial interface ICustomerAttributeService
    {
        Task<IList<CustomerAttributeValue>> GetCustomerAttributeValuesAsync(List<int> customerAttributeValueIds);
    }

    public partial class CustomerAttributeService
    {

        #region Fields

        #endregion

        #region Ctor

        #endregion

        #region Methods

        public virtual async Task<IList<CustomerAttributeValue>> GetCustomerAttributeValuesAsync(List<int> customerAttributeValueIds)
        {
            var query = from cav in _customerAttributeValueRepository.Table
                        orderby cav.DisplayOrder, cav.Id
                        where customerAttributeValueIds.Contains(cav.CustomerAttributeId)
                        select cav;

            var customerAttributeValues = await query.ToListAsync();
            return customerAttributeValues;
        }

        #endregion
    }
}
