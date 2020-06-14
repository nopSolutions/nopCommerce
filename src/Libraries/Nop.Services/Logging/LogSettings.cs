using Nop.Core.Configuration;

namespace Nop.Services.Logging
{
    public class LogSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating how long to keep the log files
        /// </summary>
        public int NumberOfDaysToRetainLogs { get; set; }
    }
}