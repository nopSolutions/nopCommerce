using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a affiliate settings model
/// </summary>
public partial record AffiliateSettingsModel : BaseNopModel, ISettingsModel
{
    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Affiliate.AllowCustomersToApplyForAffiliateAccount")]
    public bool AllowCustomersToApplyForAffiliateAccount { get; set; }
    public bool AllowCustomersToApplyForAffiliateAccount_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Affiliate.UseDefaultCommissionIfNotSetOnCatalog")]
    public bool UseDefaultCommissionIfNotSetOnCatalog { get; set; }
    public bool UseDefaultCommissionIfNotSetOnCatalog_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Affiliate.CommissionAmount")]
    public decimal CommissionAmount { get; set; }
    public bool CommissionAmount_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Affiliate.CommissionPercentage")]
    public decimal CommissionPercentage { get; set; }
    public bool CommissionPercentage_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Affiliate.UsePercentage")]
    public bool UsePercentage { get; set; }
    public bool UsePercentage_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Affiliate.HoldingPeriodInDays")]
    public int HoldingPeriodInDays { get; set; }
    public bool HoldingPeriodInDays_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Affiliate.AffiliateStorageStrategy")]
    public int AffiliateStorageStrategy { get; set; }
    public bool AffiliateStorageStrategy_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Affiliate.CustomerAffiliatePageSize")]
    public int CustomerAffiliatePageSize { get; set; }
    public bool CustomerAffiliatePageSize_OverrideForStore { get; set; }

    #endregion
}