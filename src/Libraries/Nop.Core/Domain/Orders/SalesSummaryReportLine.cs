namespace Nop.Core.Domain.Orders;

/// <summary>
/// Represents sales summary report line
/// </summary>
public partial class SalesSummaryReportLine
{
    public string Summary { get; set; }

    public DateTime SummaryDate { get; set; }

    public int NumberOfOrders { get; set; }

    public decimal Profit { get; set; }
    public string ProfitStr { get; set; }

    public string Shipping { get; set; }

    public string Tax { get; set; }

    public string OrderTotal { get; set; }

    public int SummaryType { get; set; }
}