using Nop.Core.Configuration;

namespace Nop.Core.Domain.Customers;

/// <summary>
/// Reward points settings
/// </summary>
public partial class RewardPointsSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether Reward Points Program is enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets a value of Reward Points exchange rate
    /// </summary>
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// Gets or sets the minimum reward points to use
    /// </summary>
    public int MinimumRewardPointsToUse { get; set; }

    /// <summary>
    /// Gets or sets the maximum reward points to use per order
    /// </summary>
    public int MaximumRewardPointsToUsePerOrder { get; set; }

    /// <summary>
    /// Gets or sets the maximum redemption rate of the total order amount
    /// </summary>
    public decimal MaximumRedeemedRate { get; set; }

    /// <summary>
    /// Gets or sets a number of points awarded for registration
    /// </summary>
    public int PointsForRegistration { get; set; }

    /// <summary>
    /// Gets or sets a number of days when the points awarded for registration will be valid 
    /// </summary>
    public int? RegistrationPointsValidity { get; set; }

    /// <summary>
    /// Gets or sets a number of points awarded for purchases (amount in primary store currency)
    /// </summary>
    public decimal PointsForPurchases_Amount { get; set; }

    /// <summary>
    /// Gets or sets a number of points awarded for purchases
    /// </summary>
    public int PointsForPurchases_Points { get; set; }

    /// <summary>
    /// Gets or sets a number of days when the points awarded for purchases will be valid 
    /// </summary>
    public int? PurchasesPointsValidity { get; set; }

    /// <summary>
    /// Gets or sets the minimum order total (exclude shipping cost) to award points for purchases
    /// </summary>
    public decimal MinOrderTotalToAwardPoints { get; set; }

    /// <summary>
    /// Gets or sets a delay before activation points
    /// </summary>
    public int ActivationDelay { get; set; }

    /// <summary>
    /// Gets or sets the period of activation delay
    /// </summary>
    public int ActivationDelayPeriodId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether "You will earn" message should be displayed
    /// </summary>
    public bool DisplayHowMuchWillBeEarned { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether all reward points are accumulated in one balance for all stores and they can be used in any store. Otherwise, each store has its own rewards points and they can only be used in that store.
    /// </summary>
    public bool PointsAccumulatedForAllStores { get; set; }

    /// <summary>
    /// Gets or sets the page size is for history of reward points on my account page
    /// </summary>
    public int PageSize { get; set; }
}