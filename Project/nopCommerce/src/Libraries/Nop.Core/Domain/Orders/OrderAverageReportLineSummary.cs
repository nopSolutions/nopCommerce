namespace Nop.Core.Domain.Orders;

/// <summary>
/// Represents an order average report line summary
/// </summary>
public partial class OrderAverageReportLineSummary
{
    /// <summary>
    /// Gets or sets the order status
    /// </summary>
    public OrderStatus OrderStatus { get; set; }

    /// <summary>
    /// Gets or sets the sum today total
    /// </summary>
    public decimal SumTodayOrders { get; set; }

    /// <summary>
    /// Gets or sets the today count
    /// </summary>
    public int CountTodayOrders { get; set; }

    /// <summary>
    /// Gets or sets the sum this week total
    /// </summary>
    public decimal SumThisWeekOrders { get; set; }

    /// <summary>
    /// Gets or sets the this week count
    /// </summary>
    public int CountThisWeekOrders { get; set; }

    /// <summary>
    /// Gets or sets the sum this month total
    /// </summary>
    public decimal SumThisMonthOrders { get; set; }

    /// <summary>
    /// Gets or sets the this month count
    /// </summary>
    public int CountThisMonthOrders { get; set; }

    /// <summary>
    /// Gets or sets the sum this year total
    /// </summary>
    public decimal SumThisYearOrders { get; set; }

    /// <summary>
    /// Gets or sets the this year count
    /// </summary>
    public int CountThisYearOrders { get; set; }

    /// <summary>
    /// Gets or sets the sum all time total
    /// </summary>
    public decimal SumAllTimeOrders { get; set; }

    /// <summary>
    /// Gets or sets the all time count
    /// </summary>
    public int CountAllTimeOrders { get; set; }
}