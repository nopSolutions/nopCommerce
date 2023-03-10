namespace Nop.Core.Domain.Messages
{
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
            return period switch
            {
                MessageDelayPeriod.Hours => value,
                MessageDelayPeriod.Days => value * 24,
                _ => throw new ArgumentOutOfRangeException(nameof(period)),
            };
        }
    }
}