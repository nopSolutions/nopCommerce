using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a reward points settings model
/// </summary>
public partial record RewardPointsSettingsModel : BaseNopModel, ISettingsModel
{
    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.Enabled")]
    public bool Enabled { get; set; }
    public bool Enabled_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.ExchangeRate")]
    public decimal ExchangeRate { get; set; }
    public bool ExchangeRate_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.MinimumRewardPointsToUse")]
    public int MinimumRewardPointsToUse { get; set; }
    public bool MinimumRewardPointsToUse_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.MaximumRewardPointsToUsePerOrder")]
    public int MaximumRewardPointsToUsePerOrder { get; set; }
    public bool MaximumRewardPointsToUsePerOrder_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.MaximumRedeemedRate")]
    public decimal MaximumRedeemedRate { get; set; }
    public bool MaximumRedeemedRate_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.PointsForRegistration")]
    public int PointsForRegistration { get; set; }
    public bool PointsForRegistration_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.RegistrationPointsValidity")]
    [UIHint("Int32Nullable")]
    public int? RegistrationPointsValidity { get; set; }
    public bool RegistrationPointsValidity_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.PointsForPurchases_Amount")]
    public decimal PointsForPurchases_Amount { get; set; }
    public int PointsForPurchases_Points { get; set; }
    public bool PointsForPurchases_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.MinOrderTotalToAwardPoints")]
    public decimal MinOrderTotalToAwardPoints { get; set; }
    public bool MinOrderTotalToAwardPoints_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.PurchasesPointsValidity")]
    [UIHint("Int32Nullable")]
    public int? PurchasesPointsValidity { get; set; }
    public bool PurchasesPointsValidity_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.ActivatePointsImmediately")]
    public bool ActivatePointsImmediately { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.ActivationDelay")]
    public int ActivationDelay { get; set; }
    public bool ActivationDelay_OverrideForStore { get; set; }
    public int ActivationDelayPeriodId { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.DisplayHowMuchWillBeEarned")]
    public bool DisplayHowMuchWillBeEarned { get; set; }
    public bool DisplayHowMuchWillBeEarned_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.PointsAccumulatedForAllStores")]
    public bool PointsAccumulatedForAllStores { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.PageSize")]
    public int PageSize { get; set; }
    public bool PageSize_OverrideForStore { get; set; }

    public string PrimaryStoreCurrencyCode { get; set; }

    #endregion
}