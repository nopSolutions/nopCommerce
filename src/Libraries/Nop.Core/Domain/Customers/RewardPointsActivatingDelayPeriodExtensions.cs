namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// RewardPointsActivatingDelayPeriod Extensions
    /// </summary>
    public static class RewardPointsActivatingDelayPeriodExtensions
    {
        /// <summary>
        /// Returns a delay period before activating points in hours
        /// </summary>
        /// <param name="period">Reward points activating delay period</param>
        /// <param name="value">Value of delay</param>
        /// <returns>Value of delay in hours</returns>
        public static int ToHours(this RewardPointsActivatingDelayPeriod period, int value)
        {
            return period switch
            {
                RewardPointsActivatingDelayPeriod.Hours => value,
                RewardPointsActivatingDelayPeriod.Days => value * 24,
                _ => throw new ArgumentOutOfRangeException(nameof(period)),
            };
        }
    }
}