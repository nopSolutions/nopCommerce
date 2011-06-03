using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Core.Domain.Common;
using Nop.Services.Security;

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

        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="orderRepository">Order repository</param>
        /// <param name="opvRepository">Order product variant repository</param>
        /// <param name="dateTimeHelper">Datetime helper</param>
        public OrderReportService(IRepository<Order> orderRepository,
            IRepository<OrderProductVariant> opvRepository,
            IDateTimeHelper dateTimeHelper)
        {
            this._orderRepository = orderRepository;
            this._opvRepository = opvRepository;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Get order product variant sales report
        /// </summary>
        /// <param name="startTime">Order start time; null to load all</param>
        /// <param name="endTime">Order end time; null to load all</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <returns>Result</returns>
        public virtual IList<OrderProductVariantReportLine> OrderProductVariantReport(DateTime? startTime,
            DateTime? endTime, OrderStatus? os, PaymentStatus? ps)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            var query1 = from opv in _opvRepository.Table
                         select opv;

            if (startTime.HasValue)
                query1 = query1.Where(opv => startTime.Value <= opv.Order.CreatedOnUtc);
            if (endTime.HasValue)
                query1 = query1.Where(opv => endTime.Value >= opv.Order.CreatedOnUtc);
            if (orderStatusId.HasValue)
                query1 = query1.Where(opv => orderStatusId == opv.Order.OrderStatusId);
            if (paymentStatusId.HasValue)
                query1 = query1.Where(opv => paymentStatusId == opv.Order.PaymentStatusId);
            query1 = query1.Where(opv => !opv.Order.Deleted);

            var query2 = from opv in query1
                         group opv by opv.ProductVariantId into g
                         select new
                         {
                             ProductVariantId = g.Key,
                             TotalPrice = g.Sum(x => x.PriceExclTax),
                             TotalQuantity = g.Sum(x => x.Quantity)
                         };
            query2 = query2.OrderByDescending(x => x.TotalPrice);

            var result = query2.ToList().Select(x =>
            {
                return new OrderProductVariantReportLine()
                         {
                             ProductVariantId = x.ProductVariantId,
                             TotalPrice = x.TotalPrice,
                             TotalQuantity = x.TotalQuantity
                         };
            }).ToList();
            return result;
        }

        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="os">Order status</param>
        /// <param name="ps">Payment status</param>
        /// <param name="ss">Shipping status</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <returns>Result</returns>
        public virtual OrderAverageReportLine GetOrderAverageReportLine(OrderStatus? os,
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

            var item = new OrderAverageReportLine();
            item.SumOrders = Convert.ToDecimal(query.Sum(o => (decimal?)o.OrderTotal));
            item.CountOrders = query.Count();
            return item;
        }

        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="os">Order status</param>
        /// <returns>Result</returns>
        public virtual OrderAverageReportLineSummary OrderAverageReport(OrderStatus os)
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
                var todayResult = GetOrderAverageReportLine(os, null,null, startTime1, endTime1);
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
                var weekResult = GetOrderAverageReportLine(os, null, null, startTime2, endTime2);
                item.SumThisWeekOrders = weekResult.SumOrders;
                item.CountThisWeekOrders = weekResult.CountOrders;
            }
            //month
            DateTime t3 = new DateTime(nowDt.Year, nowDt.Month, 1);
            if (!timeZone.IsInvalidTime(t3))
            {
                DateTime? startTime3 = _dateTimeHelper.ConvertToUtcTime(t3, timeZone);
                DateTime? endTime3 = null;
                var monthResult = GetOrderAverageReportLine(os, null, null, startTime3, endTime3);
                item.SumThisMonthOrders = monthResult.SumOrders;
                item.CountThisMonthOrders = monthResult.CountOrders;
            }
            //year
            DateTime t4 = new DateTime(nowDt.Year, 1, 1);
            if (!timeZone.IsInvalidTime(t4))
            {
                DateTime? startTime4 = _dateTimeHelper.ConvertToUtcTime(t4, timeZone);
                DateTime? endTime4 = null;
                var yearResult = GetOrderAverageReportLine(os, null, null, startTime4, endTime4);
                item.SumThisYearOrders = yearResult.SumOrders;
                item.CountThisYearOrders = yearResult.CountOrders;
            }
            //all time
            DateTime? startTime5 = null;
            DateTime? endTime5 = null;
            var allTimeResult = GetOrderAverageReportLine(os, null, null, startTime5, endTime5);
            item.SumAllTimeOrders = allTimeResult.SumOrders;
            item.CountAllTimeOrders = allTimeResult.CountOrders;

            return item;
        }

        #endregion
    }
}
