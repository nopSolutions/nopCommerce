using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Catalog;
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
        OrderAverageReportLine GetOrderAverageReportLine(int storeId, int vendorId, OrderStatus? os,
            PaymentStatus? ps, ShippingStatus? ss, DateTime? startTimeUtc,
            DateTime? endTimeUtc, string billingEmail, bool ignoreCancelledOrders = false);
        
        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="os">Order status</param>
        /// <returns>Result</returns>
        OrderAverageReportLineSummary OrderAverageReport(int storeId, OrderStatus os);
        
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
        IPagedList<BestsellersReportLine> BestSellersReport(int storeId = 0, int vendorId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            OrderStatus? os = null, PaymentStatus? ps = null, ShippingStatus? ss = null,
            int billingCountryId = 0,
            int orderBy = 1, int groupBy = 1,
            int pageIndex = 0, int pageSize = 2147483647,
            bool showHidden = false);
        
        /// <summary>
        /// Gets a list of products purchased by other customers who purchased the above
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="productId">Product identifier</param>
        /// <param name="recordsToReturn">Records to return</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product collection</returns>
        IList<Product> GetProductsAlsoPurchasedById(int storeId, int productId,
            int recordsToReturn = 5, bool showHidden = false);

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
        IPagedList<ProductVariant> ProductsNeverSold(int vendorId, 
            DateTime? createdFromUtc, DateTime? createdToUtc,
            int pageIndex, int pageSize, bool showHidden = false);

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
        decimal ProfitReport(int storeId, int vendorId, 
            OrderStatus? os, PaymentStatus? ps, ShippingStatus? ss, 
            DateTime? startTimeUtc, DateTime? endTimeUtc, string billingEmail);
    }
}
