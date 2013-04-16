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
using Nop.Services.Catalog;
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
        private readonly IRepository<OrderProductVariant> _opvRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductVariant> _productVariantRepository;

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IProductService _productService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="orderRepository">Order repository</param>
        /// <param name="opvRepository">Order product variant repository</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="productVariantRepository">Product variant repository</param>
        /// <param name="dateTimeHelper">Datetime helper</param>
        /// <param name="productService">Product service</param>
        public OrderReportService(IRepository<Order> orderRepository,
            IRepository<OrderProductVariant> opvRepository,
            IRepository<Product> productRepository,
            IRepository<ProductVariant> productVariantRepository,
            IDateTimeHelper dateTimeHelper, IProductService productService)
        {
            this._orderRepository = orderRepository;
            this._opvRepository = opvRepository;
            this._productRepository = productRepository;
            this._productVariantRepository = productVariantRepository;
            this._dateTimeHelper = dateTimeHelper;
            this._productService = productService;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="os">Order status</param>
        /// <param name="ps">Payment status</param>
        /// <param name="ss">Shipping status</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="ignoreCancelledOrders">A value indicating whether to ignore cancelled orders</param>
        /// <returns>Result</returns>
        public virtual OrderAverageReportLine GetOrderAverageReportLine(int storeId, int vendorId, OrderStatus? os,
            PaymentStatus? ps, ShippingStatus? ss, DateTime? startTimeUtc, DateTime? endTimeUtc,
            string billingEmail, bool ignoreCancelledOrders = false)
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
            if (vendorId > 0)
            {
                query = query
                    .Where(o => o.OrderProductVariants
                    .Any(opv => opv.ProductVariant.Product.VendorId == vendorId));
            }
            if (ignoreCancelledOrders)
            {
                int cancelledOrderStatusId = (int)OrderStatus.Cancelled;
                query = query.Where(o => o.OrderStatusId != cancelledOrderStatusId);
            }
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
            if (!String.IsNullOrEmpty(billingEmail))
                query = query.Where(o => o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.Email) && o.BillingAddress.Email.Contains(billingEmail));

			var item = (from oq in query
						group oq by 1 into result
						select new { OrderCount = result.Count(), OrderTaxSum = result.Sum(o => o.OrderTax), OrderTotalSum = result.Sum(o => o.OrderTotal) }
					   ).Select(r => new OrderAverageReportLine(){ SumTax = r.OrderTaxSum, CountOrders=r.OrderCount, SumOrders = r.OrderTotalSum}).FirstOrDefault();

			item = item ?? new OrderAverageReportLine() { CountOrders = 0, SumOrders = decimal.Zero, SumTax = decimal.Zero };
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

            DateTime nowDt = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            TimeZoneInfo timeZone = _dateTimeHelper.CurrentTimeZone;

            //today
            DateTime t1 = new DateTime(nowDt.Year, nowDt.Month, nowDt.Day);
            if (!timeZone.IsInvalidTime(t1))
            {
                DateTime? startTime1 = _dateTimeHelper.ConvertToUtcTime(t1, timeZone);
                DateTime? endTime1 = null;
                var todayResult = GetOrderAverageReportLine(storeId, 0, os, null,null, startTime1, endTime1, null);
                item.SumTodayOrders = todayResult.SumOrders;
                item.CountTodayOrders = todayResult.CountOrders;
            }
            //week
            DayOfWeek fdow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            DateTime today = new DateTime(nowDt.Year, nowDt.Month, nowDt.Day);
            DateTime t2 = today.AddDays(-(today.DayOfWeek - fdow));
            if (!timeZone.IsInvalidTime(t2))
            {
                DateTime? startTime2 = _dateTimeHelper.ConvertToUtcTime(t2, timeZone);
                DateTime? endTime2 = null;
                var weekResult = GetOrderAverageReportLine(storeId, 0, os, null, null, startTime2, endTime2, null);
                item.SumThisWeekOrders = weekResult.SumOrders;
                item.CountThisWeekOrders = weekResult.CountOrders;
            }
            //month
            DateTime t3 = new DateTime(nowDt.Year, nowDt.Month, 1);
            if (!timeZone.IsInvalidTime(t3))
            {
                DateTime? startTime3 = _dateTimeHelper.ConvertToUtcTime(t3, timeZone);
                DateTime? endTime3 = null;
                var monthResult = GetOrderAverageReportLine(storeId, 0, os, null, null, startTime3, endTime3, null);
                item.SumThisMonthOrders = monthResult.SumOrders;
                item.CountThisMonthOrders = monthResult.CountOrders;
            }
            //year
            DateTime t4 = new DateTime(nowDt.Year, 1, 1);
            if (!timeZone.IsInvalidTime(t4))
            {
                DateTime? startTime4 = _dateTimeHelper.ConvertToUtcTime(t4, timeZone);
                DateTime? endTime4 = null;
                var yearResult = GetOrderAverageReportLine(storeId, 0, os, null, null, startTime4, endTime4, null);
                item.SumThisYearOrders = yearResult.SumOrders;
                item.CountThisYearOrders = yearResult.CountOrders;
            }
            //all time
            DateTime? startTime5 = null;
            DateTime? endTime5 = null;
            var allTimeResult = GetOrderAverageReportLine(storeId, 0, os, null, null, startTime5, endTime5, null);
            item.SumAllTimeOrders = allTimeResult.SumOrders;
            item.CountAllTimeOrders = allTimeResult.CountOrders;

            return item;
        }

        /// <summary>
        /// Get best sellers report
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="createdFromUtc">Order created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Order created date to (UTC); null to load all records</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <param name="ss">Shipping status; null to load all records</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all records</param>
        /// <param name="orderBy">1 - order by quantity, 2 - order by total amount</param>
        /// <param name="groupBy">1 - group by product variants, 2 - group by products</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Result</returns>
        public virtual IPagedList<BestsellersReportLine> BestSellersReport(int storeId = 0, int vendorId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            OrderStatus? os = null, PaymentStatus? ps = null, ShippingStatus? ss = null,
            int billingCountryId = 0,
            int orderBy = 1, int groupBy = 1,
            int pageIndex = 0, int pageSize = 2147483647, 
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

            var query1 = from opv in _opvRepository.Table
                         join o in _orderRepository.Table on opv.OrderId equals o.Id
                         join pv in _productVariantRepository.Table on opv.ProductVariantId equals pv.Id
                         join p in _productRepository.Table on pv.ProductId equals p.Id
                         where (storeId == 0 || storeId == o.StoreId) &&
                         (!createdFromUtc.HasValue || createdFromUtc.Value <= o.CreatedOnUtc) &&
                         (!createdToUtc.HasValue || createdToUtc.Value >= o.CreatedOnUtc) &&
                         (!orderStatusId.HasValue || orderStatusId == o.OrderStatusId) &&
                         (!paymentStatusId.HasValue || paymentStatusId == o.PaymentStatusId) &&
                         (!shippingStatusId.HasValue || shippingStatusId == o.ShippingStatusId) &&
                         (!o.Deleted) &&
                         (!p.Deleted) &&
                         (vendorId == 0 || p.VendorId == vendorId) &&
                         (!pv.Deleted) &&
                         (billingCountryId == 0 || o.BillingAddress.CountryId == billingCountryId) &&
                         (showHidden || p.Published) &&
                         (showHidden || pv.Published)
                         select opv;

            IQueryable<BestsellersReportLine> query2 = groupBy == 1 ?
                //group by product variants
                from opv in query1
                group opv by opv.ProductVariantId into g
                select new BestsellersReportLine()
                           {
                               EntityId = g.Key,
                               TotalAmount = g.Sum(x => x.PriceExclTax),
                               TotalQuantity = g.Sum(x => x.Quantity),
                           }
                :
                //group by products
                from opv in query1
                group opv by opv.ProductVariant.ProductId into g
                select new BestsellersReportLine()
                {
                    EntityId = g.Key,
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
        /// Gets a list of products purchased by other customers who purchased the above
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="productId">Product identifier</param>
        /// <param name="recordsToReturn">Records to return</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product collection</returns>
        public virtual IList<Product> GetProductsAlsoPurchasedById(int storeId, int productId,
            int recordsToReturn = 5, bool showHidden = false)
        {
            if (productId == 0)
                throw new ArgumentException("Product ID is not specified");

            //this inner query should retrieve all orders that have contained the productID
            var query1 = (from opv in _opvRepository.Table
                          join pv in _productVariantRepository.Table on opv.ProductVariantId equals pv.Id
                          join p in _productRepository.Table on pv.ProductId equals p.Id
                          where p.Id == productId
                          select opv.OrderId).Distinct();

            var query2 = from opv in _opvRepository.Table
                         join pv in _productVariantRepository.Table on opv.ProductVariantId equals pv.Id
                         join p in _productRepository.Table on pv.ProductId equals p.Id
                         where (query1.Contains(opv.OrderId)) &&
                         (p.Id != productId) &&
                         (showHidden || p.Published) &&
                         (!opv.Order.Deleted) &&
                         (storeId == 0 || opv.Order.StoreId == storeId) &&
                         (!p.Deleted) &&
                         (showHidden || pv.Published) &&
                         (showHidden || !pv.Deleted)
                         select new { opv, p };

            var query3 = from opv_p in query2
                         group opv_p by opv_p.p.Id into g
                         select new
                         {
                             ProductId = g.Key,
                             ProductsPurchased = g.Sum(x => x.opv.Quantity),
                         };
            query3 = query3.OrderByDescending(x => x.ProductsPurchased);

            if (recordsToReturn > 0)
                query3 = query3.Take(recordsToReturn);

            var report = query3.ToList();
            var products = new List<Product>();
            foreach (var reportLine in report)
                products.Add(_productService.GetProductById(reportLine.ProductId));

            return products;
        }

        /// <summary>
        /// Gets a list of product variants that were never sold
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="createdFromUtc">Order created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Order created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product variants</returns>
        public virtual IPagedList<ProductVariant> ProductsNeverSold(int vendorId,
            DateTime? createdFromUtc, DateTime? createdToUtc, 
            int pageIndex, int pageSize, bool showHidden = false)
        {
            //this inner query should retrieve all purchased order product varint identifiers
            var query1 = (from opv in _opvRepository.Table
                          join o in _orderRepository.Table on opv.OrderId equals o.Id
                          where (!createdFromUtc.HasValue || createdFromUtc.Value <= o.CreatedOnUtc) &&
                                (!createdToUtc.HasValue || createdToUtc.Value >= o.CreatedOnUtc) &&
                                (!o.Deleted)
                          select opv.ProductVariantId).Distinct();

            var query2 = from pv in _productVariantRepository.Table
                         join p in _productRepository.Table on pv.ProductId equals p.Id
                         where (!query1.Contains(pv.Id)) &&
                               (!p.Deleted) &&
                               (!pv.Deleted) &&
                               (vendorId == 0 || p.VendorId == vendorId) &&
                               (showHidden || p.Published) &&
                               (showHidden || pv.Published)
                         select pv;

            //only distinct products (group by ID)
            //if we use standard Distinct() method, then all fields will be compared (low performance)
            //it'll not work in SQL Server Compact when searching products by a keyword)
            var query3 = from pv in query2
                         group pv by pv.Id
                         into pvGroup
                         orderby pvGroup.Key
                         select pvGroup.FirstOrDefault();

            query3 = query3.OrderBy(x => x.Id);

            var productVariants = new PagedList<ProductVariant>(query3, pageIndex, pageSize);
            return productVariants;
        }

        /// <summary>
        /// Get profit report
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <param name="ss">Shipping status; null to load all records</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <returns>Result</returns>
        public virtual decimal ProfitReport(int storeId, int vendorId, 
            OrderStatus? os, PaymentStatus? ps, ShippingStatus? ss, 
            DateTime? startTimeUtc, DateTime? endTimeUtc,
            string billingEmail)
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
            //We cannot use String.IsNullOrEmpty(billingEmail) in SQL Compact
            bool dontSearchEmail = String.IsNullOrEmpty(billingEmail);
            var query = from opv in _opvRepository.Table
                        join o in _orderRepository.Table on opv.OrderId equals o.Id
                        join pv in _productVariantRepository.Table on opv.ProductVariantId equals pv.Id
                        join p in _productRepository.Table on pv.ProductId equals p.Id
                        where (storeId == 0 || storeId == o.StoreId) && 
                              (!startTimeUtc.HasValue || startTimeUtc.Value <= o.CreatedOnUtc) &&
                              (!endTimeUtc.HasValue || endTimeUtc.Value >= o.CreatedOnUtc) &&
                              (!orderStatusId.HasValue || orderStatusId == o.OrderStatusId) &&
                              (!paymentStatusId.HasValue || paymentStatusId == o.PaymentStatusId) &&
                              (!shippingStatusId.HasValue || shippingStatusId == o.ShippingStatusId) &&
                              (!o.Deleted) &&
                              (vendorId == 0 || opv.ProductVariant.Product.VendorId == vendorId) &&
                              //we do not ignore deleted products when calculating order reports
                              //(!p.Deleted) &&
                              //(!pv.Deleted) &&
                              (dontSearchEmail || (o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.Email) && o.BillingAddress.Email.Contains(billingEmail)))
                        select new { opv, pv };
            
            var productCost = Convert.ToDecimal(query.Sum(o => (decimal?) o.pv.ProductCost * o.opv.Quantity));

            var reportSummary = GetOrderAverageReportLine(storeId, vendorId, os, ps, ss, startTimeUtc, endTimeUtc, billingEmail);
            var profit = reportSummary.SumOrders - reportSummary.SumTax - productCost;
            return profit;
        }

        #endregion
    }
}
