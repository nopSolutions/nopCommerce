namespace Nop.Core.Domain.PriceLists;

/// <summary>
/// Represents the price list strategy
/// </summary>
public enum PriceListStrategy
{
    /// <summary>
    /// Specifies that the minimal price should be used as the pricing strategy
    /// </summary>
    MinimalPrice = 1,

    /// <summary>
    /// Specifies that price lists are used based on their assigned priority
    /// </summary>
    UseByPriority = 2
}
