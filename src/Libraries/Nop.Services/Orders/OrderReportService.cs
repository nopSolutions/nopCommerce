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

        private readonly IRepository<OrderProductVariant> _opvRepository;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="opvRepository">Order product variant repository</param>
        public OrderReportService(IRepository<OrderProductVariant> opvRepository)
        {
            this._opvRepository = opvRepository;
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

        #endregion
    }
}
