namespace Nop.Core.Domain.Orders;

/// <summary>
/// Represents an "order by country" report line
/// </summary>
public partial class OrderByCountryReportLine
{
    /// <summary>
    /// Country identifier; null for unknown country
    /// </summary>
    public int? CountryId { get; set; }

    /// <summary>
    /// Country name; null for unknown country
    /// </summary>
    public string CountryName { get; set; }

    /// <summary>
    /// Gets or sets the number of orders
    /// </summary>
    public int TotalOrders { get; set; }

    /// <summary>
    /// Gets or sets the order total summary
    /// </summary>
    public decimal SumOrders { get; set; }

    /// <summary>
    /// Gets or sets the order total summary string
    /// </summary>
    public string SumOrdersStr { get; set; }
}