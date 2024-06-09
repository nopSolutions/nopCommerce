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
    public partial interface ICustomerService
    {
        Task<string> GetCustomerAttributeAsync(Customer customer, string attribute);

        Task<Customer> GetCustomerByPhoneAsync(string phone);
    }

    public partial class CustomerService
    {

        #region Fields

        #endregion

        #region Ctor

        #endregion

        #region Methods

        public virtual async Task<string> GetCustomerAttributeAsync(Customer customer, string attribute)
        {
            if (customer != null)
            {
                var customerAttribute = await _genericAttributeService.GetAttributeAsync<string>(customer, attribute);
                return customerAttribute;
            }
            return string.Empty;
        }

        public virtual async Task<Customer> GetCustomerByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return null;

            var query = from c in _customerRepository.Table
                        orderby c.Id
                        where c.Phone == phone
                        select c;
            var customer = await query.FirstOrDefaultAsync();

            return customer;
        }

        #endregion
    }
}
