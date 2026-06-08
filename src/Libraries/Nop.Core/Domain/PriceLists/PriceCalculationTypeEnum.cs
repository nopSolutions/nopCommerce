namespace Nop.Core.Domain.PriceLists;

/// <summary>
/// Specifies the available methods for calculating prices
/// </summary>
public enum PriceCalculationTypeEnum
{
    /// <summary>
    /// Percentage by which to decrease the price
    /// </summary>
    PercentageDecrease = 0,

    /// <summary>
    /// Percentage by which to increase the price
    /// </summary>
    PercentageIncrease = 1,

    /// <summary>
    /// Amount by which to decrease the price
    /// </summary>
    AmountDecrease = 2,

    /// <summary>
    /// Amount by which to increase the price
    /// </summary>
    AmountIncrease = 3,

    /// <summary>
    /// Fixed price - the same price for all products
    /// </summary>
    FixedPrice = 4
}
