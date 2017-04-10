using System;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents the period of delay
    /// </summary>
    public enum RewardPointsActivatingDelayPeriod
    {
        /// <summary>
        /// Hours
        /// </summary>
        Hours = 0,
        /// <summary>
        /// Days
        /// </summary>
        Days = 1
    }

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
            switch (period)
            {
                case RewardPointsActivatingDelayPeriod.Hours:
                    return value;
                case RewardPointsActivatingDelayPeriod.Days:
                    return value * 24;
                default:
                    throw new ArgumentOutOfRangeException("RewardPointsActivatingDelayPeriod");
            }
        }
    }
}