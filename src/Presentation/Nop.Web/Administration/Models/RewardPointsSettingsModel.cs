using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework;

namespace Nop.Admin.Models
{
    [Validator(typeof(RewardPointsSettingsValidator))]
    public class RewardPointsSettingsModel
    {
        [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.Enabled")]
        public bool Enabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.ExchangeRate")]
        public decimal ExchangeRate { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.PointsForRegistration")]
        public int PointsForRegistration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.PointsForPurchases_Amount")]
        public decimal PointsForPurchases_Amount { get; set; }

        public int PointsForPurchases_Points { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.PointsForPurchases_Awarded")]
        public int PointsForPurchases_Awarded { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.RewardPoints.PointsForPurchases_Canceled")]
        public int PointsForPurchases_Canceled { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }
    }
}