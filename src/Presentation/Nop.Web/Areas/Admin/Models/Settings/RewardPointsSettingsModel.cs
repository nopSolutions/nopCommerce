using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    public partial class RewardPointsSettingsModel : BaseNopModel
    {
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

        [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.PointsForRegistration")]
        public int PointsForRegistration { get; set; }
        public bool PointsForRegistration_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.PointsForPurchases_Amount")]
        public decimal PointsForPurchases_Amount { get; set; }
        public int PointsForPurchases_Points { get; set; }
        public bool PointsForPurchases_OverrideForStore { get; set; }

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
    }
}