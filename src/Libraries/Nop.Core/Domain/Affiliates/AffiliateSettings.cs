using Nop.Core.Configuration;

namespace Nop.Core.Domain.Affiliates;

/// <summary>
/// Affiliate settings
/// </summary>
public partial class AffiliateSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether users can fill a form to become a new affiliate
    /// </summary>
    public bool AllowCustomersToApplyForAffiliateAccount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use the default affiliate commission if not specified on catalog
    /// </summary>
    public bool UseDefaultCommissionIfNotSetOnCatalog { get; set; }

    /// <summary>
    /// Gets or sets the default affiliate commission amount (if not specified on catalog)
    /// </summary>
    public decimal CommissionAmount { get; set; }

    /// <summary>
    /// Gets or sets the default affiliate commission percentage (if not specified on catalog)
    /// </summary>
    public decimal CommissionPercentage { get; set; }

    /// <summary>
    /// Gets or sets the default affiliate commission type (percentage or amount). 
    /// If true, then percentage is used. If false, then amount is used.
    /// </summary>
    public bool UsePercentage { get; set; }

    /// <summary>
    /// Gets or sets the holding (evaluation) period in days (days after order completion)
    /// This setting will determine which orders are eligible for commission
    /// </summary>
    public int HoldingPeriodInDays { get; set; }

    /// <summary>
    /// Gets or sets the affiliate storage strategy
    /// </summary>
    public AffiliateStorageStrategyType AffiliateStorageStrategy { get; set; }

    /// <summary>
    /// Gets or sets the page size for customer affiliates
    /// </summary>
    public int CustomerAffiliatePageSize { get; set; }
}
