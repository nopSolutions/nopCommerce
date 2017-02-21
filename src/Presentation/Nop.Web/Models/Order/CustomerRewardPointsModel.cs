using System;
using System.Collections.Generic;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Order
{
    public partial class CustomerRewardPointsModel : BaseNopModel
    {
        public CustomerRewardPointsModel()
        {
            RewardPoints = new List<RewardPointsHistoryModel>();
        }

        public IList<RewardPointsHistoryModel> RewardPoints { get; set; }
        public PagerModel PagerModel { get; set; }
        public int RewardPointsBalance { get; set; }
        public int RewardPointsBalancePurchased { get; set; }
        public int RewardPointsBalanceTotal { get; set; }
        public string RewardPointsAmount { get; set; }
        public string RewardPointsAmountPurchased { get; set; }
        public string RewardPointsAmountTotal { get; set; }
        public int MinimumRewardPointsBalance { get; set; }
        public string MinimumRewardPointsAmount { get; set; }

        #region Nested classes

        public partial class RewardPointsHistoryModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("RewardPoints.Fields.Points")]
            public int Points { get; set; }
            [NopResourceDisplayName("RewardPoints.Fields.PointsPurchased")]
            public int PointsPurchased { get; set; }
            [NopResourceDisplayName("RewardPoints.Fields.PointsBalance")]
            public string PointsBalance { get; set; }

            [NopResourceDisplayName("RewardPoints.Fields.PointsBalancePurchased")]
            public string PointsBalancePurchased { get; set; }

            [NopResourceDisplayName("RewardPoints.Fields.Message")]
            public string Message { get; set; }

            [NopResourceDisplayName("RewardPoints.Fields.Date")]
            public DateTime CreatedOn { get; set; }
        }

        #endregion
    }
}