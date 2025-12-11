namespace Nop.Core.Domain.Customers;

/// <summary>
/// Represents a registered customer report line
/// </summary>
public partial class RegisteredCustomersReportLine
{
    /// <summary>
    /// Gets or sets the period.
    /// </summary>
    public string Period { get; set; }

    /// <summary>
    /// Gets or sets the customers.
    /// </summary>
    public int Customers { get; set; }
}