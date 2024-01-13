using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Order;

public partial record CustomerRewardPointsModel : BaseNopModel
{
    public CustomerRewardPointsModel()
    {
        RewardPoints = new List<RewardPointsHistoryModel>();
    }

    public IList<RewardPointsHistoryModel> RewardPoints { get; set; }
    public PagerModel PagerModel { get; set; }
    public int RewardPointsBalance { get; set; }
    public string RewardPointsAmount { get; set; }
    public int MinimumRewardPointsBalance { get; set; }
    public string MinimumRewardPointsAmount { get; set; }

    #region Nested classes

    public partial record RewardPointsHistoryModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("RewardPoints.Fields.Points")]
        public int Points { get; set; }

        [NopResourceDisplayName("RewardPoints.Fields.PointsBalance")]
        public string PointsBalance { get; set; }

        [NopResourceDisplayName("RewardPoints.Fields.Message")]
        public string Message { get; set; }

        [NopResourceDisplayName("RewardPoints.Fields.CreatedDate")]
        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("RewardPoints.Fields.EndDate")]
        public DateTime? EndDate { get; set; }
    }

    #endregion
}