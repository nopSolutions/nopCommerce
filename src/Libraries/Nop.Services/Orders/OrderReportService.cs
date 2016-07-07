using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Services.Helpers;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Order report service
    /// </summary>
    public partial class OrderReportService : IOrderReportService
    {
        #region Fields

        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="orderRepository">Order repository</param>
        /// <param name="orderItemRepository">Order item repository</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="dateTimeHelper">Datetime helper</param>
        public OrderReportService(IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<Product> productRepository,
            IDateTimeHelper dateTimeHelper)
        {
            this._orderRepository = orderRepository;
            this._orderItemRepository = orderItemRepository;
            this._productRepository = productRepository;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get "order by country" report
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="os">Order status</param>
        /// <param name="ps">Payment status</param>
        /// <param name="ss">Shipping status</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <returns>Result</returns>
        public virtual IList<OrderByCountryReportLine> GetCountryReport(int storeId, OrderStatus? os,
            PaymentStatus? ps, ShippingStatus? ss, DateTime? startTimeUtc, DateTime? endTimeUtc)
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

            var query = _orderRepository.Table;
            query = query.Where(o => !o.Deleted);
            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);
            if (orderStatusId.HasValue)
                query = query.Where(o => o.OrderStatusId == orderStatusId.Value);
            if (paymentStatusId.HasValue)
                query = query.Where(o => o.PaymentStatusId == paymentStatusId.Value);
            if (shippingStatusId.HasValue)
                query = query.Where(o => o.ShippingStatusId == shippingStatusId.Value);
            if (startTimeUtc.HasValue)
                query = query.Where(o => startTimeUtc.Value <= o.CreatedOnUtc);
            if (endTimeUtc.HasValue)
                query = query.Where(o => endTimeUtc.Value >= o.CreatedOnUtc);
            
            var report = (from oq in query
                        group oq by oq.BillingAddress.CountryId into result
                        select new
                        {
                            CountryId = result.Key,
                            TotalOrders = result.Count(),
                            SumOrders = result.Sum(o => o.OrderTotal)
                        }
                       )
                       .OrderByDescending(x => x.SumOrders)
                       .Select(r => new OrderByCountryReportLine
                       {
                           CountryId = r.CountryId,
                           TotalOrders = r.TotalOrders,
                           SumOrders = r.SumOrders
                       })

                       .ToList();

            return report;
        }

        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to ignore this parameter</param>
        /// <param name="vendorId">Vendor identifier; pass 0 to ignore this parameter</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all orders</param>
        /// <param name="orderId">Order identifier; pass 0 to ignore this parameter</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="osIds">Order status identifiers</param>
        /// <param name="psIds">Payment status identifiers</param>
        /// <param name="ssIds">Shipping status identifiers</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="orderNotes">Search in order notes. Leave empty to load all records.</param>
        /// <returns>Result</returns>
        public virtual OrderAverageReportLine GetOrderAverageReportLine(int storeId = 0,
            int vendorId = 0, int billingCountryId = 0, 
            int orderId = 0, string paymentMethodSystemName = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null,
            string billingEmail = null, string billingLastName = "", string orderNotes = null)
        {
            var query = _orderRepository.Table;
            query = query.Where(o => !o.Deleted);
            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);
            if (orderId > 0)
                query = query.Where(o => o.Id == orderId);
            if (vendorId > 0)
            {
                query = query
                    .Where(o => o.OrderItems
                    .Any(orderItem => orderItem.Product.VendorId == vendorId));
            }
            if (billingCountryId > 0)
                query = query.Where(o => o.BillingAddress != null && o.BillingAddress.CountryId == billingCountryId);
            if (!String.IsNullOrEmpty(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);
            if (osIds != null && osIds.Any())
                query = query.Where(o => osIds.Contains(o.OrderStatusId));
            if (psIds != null && psIds.Any())
                query = query.Where(o => psIds.Contains(o.PaymentStatusId));
            if (ssIds != null && ssIds.Any())
                query = query.Where(o => ssIds.Contains(o.ShippingStatusId));
            if (startTimeUtc.HasValue)
                query = query.Where(o => startTimeUtc.Value <= o.CreatedOnUtc);
            if (endTimeUtc.HasValue)
                query = query.Where(o => endTimeUtc.Value >= o.CreatedOnUtc);
            if (!String.IsNullOrEmpty(billingEmail))
                query = query.Where(o => o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.Email) && o.BillingAddress.Email.Contains(billingEmail));
            if (!String.IsNullOrEmpty(billingLastName))
                query = query.Where(o => o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.LastName) && o.BillingAddress.LastName.Contains(billingLastName));
            if (!String.IsNullOrEmpty(orderNotes))
                query = query.Where(o => o.OrderNotes.Any(on => on.Note.Contains(orderNotes)));
            
			var item = (from oq in query
						group oq by 1 into result
						select new
						           {
                                       OrderCount = result.Count(),
                                       OrderShippingExclTaxSum = result.Sum(o => o.OrderShippingExclTax),
                                       OrderTaxSum = result.Sum(o => o.OrderTax), 
                                       OrderTotalSum = result.Sum(o => o.OrderTotal)
						           }
					   ).Select(r => new OrderAverageReportLine
                       {
                           CountOrders = r.OrderCount,
                           SumShippingExclTax = r.OrderShippingExclTaxSum, 
                           SumTax = r.OrderTaxSum, 
                           SumOrders = r.OrderTotalSum
                       })
                       .FirstOrDefault();

			item = item ?? new OrderAverageReportLine
			                   {
                                   CountOrders = 0,
                                   SumShippingExclTax = decimal.Zero,
                                   SumTax = decimal.Zero,
                                   SumOrders = decimal.Zero, 
			                   };
            return item;
        }

        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="os">Order status</param>
        /// <returns>Result</returns>
        public virtual OrderAverageReportLineSummary OrderAverageReport(int storeId, OrderStatus os)
        {
            var item = new OrderAverageReportLineSummary();
            item.OrderStatus = os;
            var orderStatuses = new List<int>() { (int)os };

            DateTime nowDt = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            TimeZoneInfo timeZone = _dateTimeHelper.CurrentTimeZone;

            //today
            var t1 = new DateTime(nowDt.Year, nowDt.Month, nowDt.Day);
            if (!timeZone.IsInvalidTime(t1))
            {
                DateTime? startTime1 = _dateTimeHelper.ConvertToUtcTime(t1, timeZone);
                var todayResult = GetOrderAverageReportLine(storeId: storeId,
                    osIds: orderStatuses, 
                    startTimeUtc: startTime1);
                item.SumTodayOrders = todayResult.SumOrders;
                item.CountTodayOrders = todayResult.CountOrders;
            }
            //week
            DayOfWeek fdow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            var today = new DateTime(nowDt.Year, nowDt.Month, nowDt.Day);
            DateTime t2 = today.AddDays(-(today.DayOfWeek - fdow));
            if (!timeZone.IsInvalidTime(t2))
            {
                DateTime? startTime2 = _dateTimeHelper.ConvertToUtcTime(t2, timeZone);
                var weekResult = GetOrderAverageReportLine(storeId: storeId,
                    osIds: orderStatuses,
                    startTimeUtc: startTime2);
                item.SumThisWeekOrders = weekResult.SumOrders;
                item.CountThisWeekOrders = weekResult.CountOrders;
            }
            //month
            var t3 = new DateTime(nowDt.Year, nowDt.Month, 1);
            if (!timeZone.IsInvalidTime(t3))
            {
                DateTime? startTime3 = _dateTimeHelper.ConvertToUtcTime(t3, timeZone);
                var monthResult = GetOrderAverageReportLine(storeId: storeId,
                    osIds: orderStatuses,
                    startTimeUtc: startTime3);
                item.SumThisMonthOrders = monthResult.SumOrders;
                item.CountThisMonthOrders = monthResult.CountOrders;
            }
            //year
            var t4 = new DateTime(nowDt.Year, 1, 1);
            if (!timeZone.IsInvalidTime(t4))
            {
                DateTime? startTime4 = _dateTimeHelper.ConvertToUtcTime(t4, timeZone);
                var yearResult = GetOrderAverageReportLine(storeId: storeId,
                    osIds: orderStatuses,
                    startTimeUtc: startTime4);
                item.SumThisYearOrders = yearResult.SumOrders;
                item.CountThisYearOrders = yearResult.CountOrders;
            }
            //all time
            var allTimeResult = GetOrderAverageReportLine(storeId: storeId, osIds: orderStatuses);
            item.SumAllTimeOrders = allTimeResult.SumOrders;
            item.CountAllTimeOrders = allTimeResult.CountOrders;

            return item;
        }

        /// <summary>
        /// Get best sellers report
        /// </summary>
        /// <param name="storeId">Store identifier (orders placed in a specific store); 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="categoryId">Category identifier; 0 to load all records</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all records</param>
        /// <param name="createdFromUtc">Order created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Order created date to (UTC); null to load all records</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <param name="ss">Shipping status; null to load all records</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all records</param>
        /// <param name="orderBy">1 - order by quantity, 2 - order by total amount</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Result</returns>
        public virtual IPagedList<BestsellersReportLine> BestSellersReport(
            int categoryId = 0, int manufacturerId = 0,
            int storeId = 0, int vendorId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            OrderStatus? os = null, PaymentStatus? ps = null, ShippingStatus? ss = null,
            int billingCountryId = 0,
            int orderBy = 1,
            int pageIndex = 0, int pageSize = int.MaxValue, 
            bool showHidden = false)
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

            var query1 = from orderItem in _orderItemRepository.Table
                         join o in _orderRepository.Table on orderItem.OrderId equals o.Id
                         join p in _productRepository.Table on orderItem.ProductId equals p.Id
                         //join pc in _productCategoryRepository.Table on p.Id equals pc.ProductId into p_pc from pc in p_pc.DefaultIfEmpty()
                         //join pm in _productManufacturerRepository.Table on p.Id equals pm.ProductId into p_pm from pm in p_pm.DefaultIfEmpty()
                         where (storeId == 0 || storeId == o.StoreId) &&
                         (!createdFromUtc.HasValue || createdFromUtc.Value <= o.CreatedOnUtc) &&
                         (!createdToUtc.HasValue || createdToUtc.Value >= o.CreatedOnUtc) &&
                         (!orderStatusId.HasValue || orderStatusId == o.OrderStatusId) &&
                         (!paymentStatusId.HasValue || paymentStatusId == o.PaymentStatusId) &&
                         (!shippingStatusId.HasValue || shippingStatusId == o.ShippingStatusId) &&
                         (!o.Deleted) &&
                         (!p.Deleted) &&
                         (vendorId == 0 || p.VendorId == vendorId) &&
                         //(categoryId == 0 || pc.CategoryId == categoryId) &&
                         //(manufacturerId == 0 || pm.ManufacturerId == manufacturerId) &&
                         (categoryId == 0 || p.ProductCategories.Count(pc => pc.CategoryId == categoryId) > 0) &&
                         (manufacturerId == 0 || p.ProductManufacturers.Count(pm => pm.ManufacturerId == manufacturerId) > 0) &&
                         (billingCountryId == 0 || o.BillingAddress.CountryId == billingCountryId) &&
                         (showHidden || p.Published)
                         select orderItem;

            IQueryable<BestsellersReportLine> query2 = 
                //group by products
                from orderItem in query1
                group orderItem by orderItem.ProductId into g
                select new BestsellersReportLine
                {
                    ProductId = g.Key,
                    TotalAmount = g.Sum(x => x.PriceExclTax),
                    TotalQuantity = g.Sum(x => x.Quantity),
                }
                ;

            switch (orderBy)
            {
                case 1:
                    {
                        query2 = query2.OrderByDescending(x => x.TotalQuantity);
                    }
                    break;
                case 2:
                    {
                        query2 = query2.OrderByDescending(x => x.TotalAmount);
                    }
                    break;
                default:
                    throw new ArgumentException("Wrong orderBy parameter", "orderBy");
            }

            var result = new PagedList<BestsellersReportLine>(query2, pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// Gets a list of products (identifiers) purchased by other customers who purchased a specified product
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="productId">Product identifier</param>
        /// <param name="recordsToReturn">Records to return</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only products marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Products</returns>
        public virtual int[] GetAlsoPurchasedProductsIds(int storeId, int productId,
            int recordsToReturn = 5, bool visibleIndividuallyOnly = true, bool showHidden = false)
        {
            if (productId == 0)
                throw new ArgumentException("Product ID is not specified");

            //this inner query should retrieve all orders that contains a specified product ID
            var query1 = from orderItem in _orderItemRepository.Table
                          where orderItem.ProductId == productId
                          select orderItem.OrderId;

            var query2 = from orderItem in _orderItemRepository.Table
                         join p in _productRepository.Table on orderItem.ProductId equals p.Id
                         where (query1.Contains(orderItem.OrderId)) &&
                         (p.Id != productId) &&
                         (showHidden || p.Published) &&
                         (!orderItem.Order.Deleted) &&
                         (storeId == 0 || orderItem.Order.StoreId == storeId) &&
                         (!p.Deleted) &&
                         (!visibleIndividuallyOnly || p.VisibleIndividually)
                         select new { orderItem, p };

            var query3 = from orderItem_p in query2
                         group orderItem_p by orderItem_p.p.Id into g
                         select new
                         {
                             ProductId = g.Key,
                             ProductsPurchased = g.Sum(x => x.orderItem.Quantity),
                         };
            query3 = query3.OrderByDescending(x => x.ProductsPurchased);

            if (recordsToReturn > 0)
                query3 = query3.Take(recordsToReturn);

            var report = query3.ToList();
            
            var ids = new List<int>();
            foreach (var reportLine in report)
                ids.Add(reportLine.ProductId);

            return ids.ToArray();
        }

        /// <summary>
        /// Gets a list of products that were never sold
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="createdFromUtc">Order created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Order created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Products</returns>
        public virtual IPagedList<Product> ProductsNeverSold(int vendorId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            //this inner query should retrieve all purchased product identifiers
            var query1 = (from orderItem in _orderItemRepository.Table
                          join o in _orderRepository.Table on orderItem.OrderId equals o.Id
                          where (!createdFromUtc.HasValue || createdFromUtc.Value <= o.CreatedOnUtc) &&
                                (!createdToUtc.HasValue || createdToUtc.Value >= o.CreatedOnUtc) &&
                                (!o.Deleted)
                          select orderItem.ProductId).Distinct();

            var simpleProductTypeId = (int)ProductType.SimpleProduct;

            var query2 = from p in _productRepository.Table
                         orderby p.Name
                         where (!query1.Contains(p.Id)) &&
                             //include only simple products
                               (p.ProductTypeId == simpleProductTypeId) &&
                               (!p.Deleted) &&
                               (vendorId == 0 || p.VendorId == vendorId) &&
                               (showHidden || p.Published)
                         select p;

            var products = new PagedList<Product>(query2, pageIndex, pageSize);
            return products;
        }

        /// <summary>
        /// Get profit report
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to ignore this parameter</param>
        /// <param name="vendorId">Vendor identifier; pass 0 to ignore this parameter</param>
        /// <param name="orderId">Order identifier; pass 0 to ignore this parameter</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all orders</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <param name="osIds">Order status identifiers; null to load all records</param>
        /// <param name="psIds">Payment status identifiers; null to load all records</param>
        /// <param name="ssIds">Shipping status identifiers; null to load all records</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="orderNotes">Search in order notes. Leave empty to load all records.</param>
        /// <returns>Result</returns>
        public virtual decimal ProfitReport(int storeId = 0, int vendorId = 0,
            int billingCountryId = 0, int orderId = 0, string paymentMethodSystemName = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null,
            string billingEmail = null, string billingLastName = "", string orderNotes = null)
        {
            //We cannot use String.IsNullOrEmpty() in SQL Compact
            bool dontSearchEmail = String.IsNullOrEmpty(billingEmail);
            //We cannot use String.IsNullOrEmpty() in SQL Compact
            bool dontSearchLastName = String.IsNullOrEmpty(billingLastName);
            //We cannot use String.IsNullOrEmpty() in SQL Compact
            bool dontSearchOrderNotes = String.IsNullOrEmpty(orderNotes);
            //We cannot use String.IsNullOrEmpty() in SQL Compact
            bool dontSearchPaymentMethods = String.IsNullOrEmpty(paymentMethodSystemName);

            var orders = _orderRepository.Table;
            if (osIds != null && osIds.Any())
                orders = orders.Where(o => osIds.Contains(o.OrderStatusId));
            if (psIds != null && psIds.Any())
                orders = orders.Where(o => psIds.Contains(o.PaymentStatusId));
            if (ssIds != null && ssIds.Any())
                orders = orders.Where(o => ssIds.Contains(o.ShippingStatusId));

            var query = from orderItem in _orderItemRepository.Table
                        join o in orders on orderItem.OrderId equals o.Id
                        where (storeId == 0 || storeId == o.StoreId) &&
                              (orderId == 0 || orderId == o.Id) &&
                              (billingCountryId ==0 || (o.BillingAddress != null && o.BillingAddress.CountryId == billingCountryId)) &&
                              (dontSearchPaymentMethods || paymentMethodSystemName == o.PaymentMethodSystemName) &&
                              (!startTimeUtc.HasValue || startTimeUtc.Value <= o.CreatedOnUtc) &&
                              (!endTimeUtc.HasValue || endTimeUtc.Value >= o.CreatedOnUtc) &&
                              (!o.Deleted) &&
                              (vendorId == 0 || orderItem.Product.VendorId == vendorId) &&
                              //we do not ignore deleted products when calculating order reports
                              //(!p.Deleted)
                              (dontSearchEmail || (o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.Email) && o.BillingAddress.Email.Contains(billingEmail))) &&
                              (dontSearchLastName || (o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.LastName) && o.BillingAddress.LastName.Contains(billingLastName))) &&
                              (dontSearchOrderNotes || o.OrderNotes.Any(oNote => oNote.Note.Contains(orderNotes)))
                        select orderItem;

            var productCost = Convert.ToDecimal(query.Sum(orderItem => (decimal?)orderItem.OriginalProductCost * orderItem.Quantity));

            var reportSummary = GetOrderAverageReportLine(
                storeId: storeId,
                vendorId: vendorId,
                billingCountryId: billingCountryId,
                orderId: orderId,
                paymentMethodSystemName: paymentMethodSystemName,
                osIds: osIds, 
                psIds: psIds, 
                ssIds: ssIds,
                startTimeUtc: startTimeUtc,
                endTimeUtc: endTimeUtc,
                billingEmail: billingEmail,
                billingLastName: billingLastName,
                orderNotes: orderNotes);
            var profit = reportSummary.SumOrders - reportSummary.SumShippingExclTax - reportSummary.SumTax - productCost;
            return profit;
        }

        #endregion
    }
}
