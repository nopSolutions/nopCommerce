namespace Nop.Core.Domain.Catalog;

/// <summary>
/// Represents a recurring product cycle period
/// </summary>
public enum RecurringProductCyclePeriod
{
    /// <summary>
    /// Days
    /// </summary>
    Days = 0,

    /// <summary>
    /// Weeks
    /// </summary>
    Weeks = 10,

    /// <summary>
    /// Months
    /// </summary>
    Months = 20,

    /// <summary>
    /// Years
    /// </summary>
    Years = 30,
}