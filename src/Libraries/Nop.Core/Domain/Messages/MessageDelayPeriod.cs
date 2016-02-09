using System;

namespace Nop.Core.Domain.Messages
{
    /// <summary>
    /// Represents the period of message delay
    /// </summary>
    public enum MessageDelayPeriod
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
    /// MessageDelayPeriod Extensions
    /// </summary>
    public static class MessageDelayPeriodExtensions
    {
        /// <summary>
        /// Returns message delay in hours
        /// </summary>
        /// <param name="period">Message delay period</param>
        /// <param name="value">Value of delay send</param>
        /// <returns>Value of message delay in hours</returns>
        public static int ToHours(this MessageDelayPeriod period, int value)
        {
            switch (period)
            {
                case MessageDelayPeriod.Hours:
                    return value;
                case MessageDelayPeriod.Days:
                    return value * 24;
                default:
                    throw new ArgumentOutOfRangeException("MessageDelayPeriod");
            }
        }
    }
}
