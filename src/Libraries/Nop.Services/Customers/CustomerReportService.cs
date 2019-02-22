using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Services.Helpers;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer report service
    /// </summary>
    public partial class CustomerReportService : ICustomerReportService
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Order> _orderRepository;

        #endregion

        #region Ctor

        public CustomerReportService(ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IRepository<Customer> customerRepository,
            IRepository<Order> orderRepository)
        {
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get best customers
        /// </summary>
        /// <param name="createdFromUtc">Order created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Order created date to (UTC); null to load all records</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <param name="ss">Order shipment status; null to load all records</param>
        /// <param name="orderBy">1 - order by order total, 2 - order by number of orders</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Report</returns>
        public virtual IPagedList<BestCustomerReportLine> GetBestCustomersReport(DateTime? createdFromUtc,
            DateTime? createdToUtc, OrderStatus? os, PaymentStatus? ps, ShippingStatus? ss, int orderBy,
            int pageIndex = 0, int pageSize = 214748364)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            int? shippingStatusId = null;
            if (ss.HasValue)
                shippingStatusId = (int)ss.Value;
            var query1 = from c in _customerRepository.Table
                         join o in _orderRepository.Table on c.Id equals o.CustomerId
                         where (!createdFromUtc.HasValue || createdFromUtc.Value <= o.CreatedOnUtc) &&
                         (!createdToUtc.HasValue || createdToUtc.Value >= o.CreatedOnUtc) &&
                         (!orderStatusId.HasValue || orderStatusId == o.OrderStatusId) &&
                         (!paymentStatusId.HasValue || paymentStatusId == o.PaymentStatusId) &&
                         (!shippingStatusId.HasValue || shippingStatusId == o.ShippingStatusId) &&
                         !o.Deleted &&
                         !c.Deleted
                         select new { c, o };

            var query2 = from co in query1
                         group co by co.c.Id into g
                         select new
                         {
                             CustomerId = g.Key,
                             OrderTotal = g.Sum(x => x.o.OrderTotal),
                             OrderCount = g.Count()
                         };
            switch (orderBy)
            {
                case 1:
                    query2 = query2.OrderByDescending(x => x.OrderTotal);
                    break;
                case 2:
                    query2 = query2.OrderByDescending(x => x.OrderCount);
                    break;
                default:
                    throw new ArgumentException("Wrong orderBy parameter", "orderBy");
            }

            var tmp = new PagedList<dynamic>(query2, pageIndex, pageSize);
            return new PagedList<BestCustomerReportLine>(tmp.Select(x => new BestCustomerReportLine
            {
                CustomerId = x.CustomerId,
                OrderTotal = x.OrderTotal,
                OrderCount = x.OrderCount
            }),
                tmp.PageIndex, tmp.PageSize, tmp.TotalCount);
        }

        /// <summary>
        /// Gets a report of customers registered in the last days
        /// </summary>
        /// <param name="days">Customers registered in the last days</param>
        /// <returns>Number of registered customers</returns>
        public virtual int GetRegisteredCustomersReport(int days)
        {
            var date = _dateTimeHelper.ConvertToUserTime(DateTime.Now).AddDays(-days);

            var registeredCustomerRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.RegisteredRoleName);
            if (registeredCustomerRole == null)
                return 0;

            var query = from c in _customerRepository.Table
                        from mapping in c.CustomerCustomerRoleMappings
                        where !c.Deleted &&
                        mapping.CustomerRoleId == registeredCustomerRole.Id &&
                        c.CreatedOnUtc >= date
                        //&& c.CreatedOnUtc <= DateTime.UtcNow
                        select c;

            var count = query.Count();
            return count;
        }

        #endregion
    }
}