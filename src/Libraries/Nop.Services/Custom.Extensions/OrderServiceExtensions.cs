using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Orders
{
    public partial interface IOrderService
    {
        IQueryable<CustomerOrderTemp> GetCustomerOrderTempTable();

        //Task<int> GetCustomerLatestValidOrder(int customerId);
    }

    public partial class OrderService
    {

        #region Methods

        public virtual IQueryable<CustomerOrderTemp> GetCustomerOrderTempTable()
        {
            var query = from o in _orderRepository.Table
                        join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                        join p in _productRepository.Table on oi.ProductId equals p.Id
                        join c in _customerRepository.Table on o.CustomerId equals c.Id
                        where o.OrderStatusId == 30 // Order Paid
                        select new { ProductId = c.VendorId, o.PaidDateUtc };

            var customerOrderTemp = query.Select(tmp => new CustomerOrderTemp
            {
                ProductId = tmp.ProductId,
                PremiumCustomer = (
                    tmp.ProductId == 16 && (tmp.PaidDateUtc.HasValue ? tmp.PaidDateUtc.Value.AddDays(90) <= DateTime.Now : false) ? true :
                    tmp.ProductId == 17 && (tmp.PaidDateUtc.HasValue ? tmp.PaidDateUtc.Value.AddDays(180) <= DateTime.Now : false) ? true :
                    tmp.ProductId == 18 && (tmp.PaidDateUtc.HasValue ? tmp.PaidDateUtc.Value.AddDays(365) <= DateTime.Now : false) ? true : false
                    )
            });

            return customerOrderTemp;
        }

        public bool IsPremiumCustomer(dynamic d)
        {
            var paidDateUtc = d.GetType().GetProperty("PaidDateUtc").GetValue(d, null);
            var productId = d.GetType().GetProperty("ProductId").GetValue(d, null);

            //var intProductId = Convert.ToInt32(productId);
            //var paidDateUtcWithDateTime = Convert.ToDateTime(paidDateUtc);

            //if (intProductId == 16 && paidDateUtcWithDateTime >= (DateTime.UtcNow.Date - 90))
            //{

            //}

            return true;
        }

        //public virtual async Task<int> GetSubscriptionAlottedCreditCountAsync(int customerId)
        //{
        //    var query = _orderRepository.Table;

        //    //filter by customer
        //    if (customerId > 0)
        //        query = query.Where(historyEntry => historyEntry.CustomerId == customerId);

        //    //filter by store
        //    if (!_rewardPointsSettings.PointsAccumulatedForAllStores && storeId > 0)
        //        query = query.Where(historyEntry => historyEntry.StoreId == storeId);

        //    //whether to show only the points that already activated
        //    if (!showNotActivated)
        //        query = query.Where(historyEntry => historyEntry.CreatedOnUtc < DateTime.UtcNow);

        //    query = query.Where(entry => entry.EndDateUtc.HasValue).OrderByDescending(entry => entry.CreatedOnUtc);

        //    return (await query.FirstOrDefaultAsync()).Points;
        //}

        #endregion
    }
}
