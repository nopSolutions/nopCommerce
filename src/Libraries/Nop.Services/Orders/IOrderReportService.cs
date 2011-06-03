using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Order report service interface
    /// </summary>
    public partial interface IOrderReportService
    {
        /// <summary>
        /// Get order product variant sales report
        /// </summary>
        /// <param name="startTime">Order start time; null to load all</param>
        /// <param name="endTime">Order end time; null to load all</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <returns>Result</returns>
        IList<OrderProductVariantReportLine> OrderProductVariantReport(DateTime? startTime,
            DateTime? endTime, OrderStatus? os, PaymentStatus? ps);

        
        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="os">Order status</param>
        /// <param name="ps">Payment status</param>
        /// <param name="ss">Shipping status</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <returns>Result</returns>
        OrderAverageReportLine GetOrderAverageReportLine(OrderStatus? os,
            PaymentStatus? ps, ShippingStatus? ss, DateTime? startTimeUtc, DateTime? endTimeUtc);
        
        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="os">Order status</param>
        /// <returns>Result</returns>
        OrderAverageReportLineSummary OrderAverageReport(OrderStatus os);
    }
}
