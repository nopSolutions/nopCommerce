namespace Nop.Core.Configuration
{
    /// <summary>
    /// Represents common configuration parameters
    /// </summary>
    public partial class CommonConfig : IConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether to display the full error in production environment. It's ignored (always enabled) in development environment
        /// </summary>
        public bool DisplayFullErrorStack { get; set; } = false;

        /// <summary>
        /// Gets or sets path to database with user agent strings
        /// </summary>
        public string UserAgentStringsPath { get; set; } = "~/App_Data/browscap.xml";

        /// <summary>
        /// Gets or sets path to database with crawler only user agent strings
        /// </summary>
        public string CrawlerOnlyUserAgentStringsPath { get; set; } = "~/App_Data/browscap.crawlersonly.xml";

        /// <summary>
        /// Gets or sets a value indicating whether to store TempData in the session state. By default the cookie-based TempData provider is used to store TempData in cookies.
        /// </summary>
        public bool UseSessionStateTempDataProvider { get; set; } = false;

        /// <summary>
        /// Gets or sets a value that indicates whether to use MiniProfiler services
        /// </summary>
        public bool MiniProfilerEnabled { get; set; } = false;
    }
}