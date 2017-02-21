

using System;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Reward points
    /// </summary>
    public class RewardPoints
    {
        #region Fields
        private readonly IRewardPointService _rewardPointService;
        #endregion
        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="rewardPointService">Reward point service</param>
        public RewardPoints(IRewardPointService rewardPointService)
        {
            this._rewardPointService = rewardPointService;
        }
        #endregion
        /// <summary>
        /// Points
        /// </summary>
        public int Points { get; set; } = 0;
        /// <summary>
        /// Points Amount. Same sign as points.
        /// </summary>
        public decimal Amount
        {
            get { return _rewardPointService.ConvertRewardPointsToAmount(Points); }
        }
        /// <summary>
        /// Purchased Points
        /// </summary>
        public int PointsPurchased { get; set; } = 0;
        /// <summary>
        /// Purchased Points Amount.  Same sign as points purchased.
        /// </summary>
        public decimal AmountPurchased
        {
            get { return _rewardPointService.ConvertRewardPointsToAmount(PointsPurchased); }
        }

        /// <summary>
        /// Total Points
        /// </summary>
        public int PointsTotal
        {
            get { return Points + PointsPurchased; }
        }
        /// <summary>
        /// Total Points Amount
        /// </summary>
        public decimal AmountTotal
        {
            get { return Amount + AmountPurchased; }
        }
        /// <summary>
        /// Total Points
        /// </summary>
        public int PointsTotalCorrectedForMinPointsToUse
        {
            get { return (_rewardPointService.CheckMinimumRewardPointsToUseRequirement(Math.Abs(Points)) ? Points : 0 ) + PointsPurchased; }
        }
        /// <summary>
        /// Total Points Amount
        /// </summary>
        public decimal AmountTotalCorrectedForMinPointsToUse
        {
            get { return (_rewardPointService.CheckMinimumRewardPointsToUseRequirement(Math.Abs(Points)) ? Amount : decimal.Zero ) + AmountPurchased; }
        }

        public void RevertSign()
        {
            Points = -Points;
            PointsPurchased = -PointsPurchased;
        }
    }
}
