using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Services.Orders;

namespace Nop.Services.Customers;

/// <summary>
/// Customer report service interface
/// </summary>
public partial interface ICustomerReportService
{
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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the report
    /// </returns>
    Task<IPagedList<BestCustomerReportLine>> GetBestCustomersReportAsync(DateTime? createdFromUtc,
        DateTime? createdToUtc, OrderStatus? os, PaymentStatus? ps, ShippingStatus? ss, OrderByEnum orderBy,
        int pageIndex = 0, int pageSize = 214748364);

    /// <summary>
    /// Gets a report of customers registered in the last days
    /// </summary>
    /// <param name="days">Customers registered in the last days</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of registered customers
    /// </returns>
    Task<int> GetRegisteredCustomersReportAsync(int days);
}