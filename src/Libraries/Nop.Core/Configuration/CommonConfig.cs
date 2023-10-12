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
        public bool DisplayFullErrorStack { get; protected set; } = false;

        /// <summary>
        /// Gets or sets path to database with user agent strings
        /// </summary>
        public string UserAgentStringsPath { get; protected set; } = "~/App_Data/browscap.xml";

        /// <summary>
        /// Gets or sets path to database with crawler only user agent strings
        /// </summary>
        public string CrawlerOnlyUserAgentStringsPath { get; protected set; } = "~/App_Data/browscap.crawlersonly.xml";

        /// <summary>
        /// Gets or sets path to additional database with crawler only user agent strings
        /// </summary>
        public string CrawlerOnlyAdditionalUserAgentStringsPath { get; protected set; } = "~/App_Data/additional.crawlers.xml";

        /// <summary>
        /// Gets or sets a value indicating whether to store TempData in the session state. By default the cookie-based TempData provider is used to store TempData in cookies.
        /// </summary>
        public bool UseSessionStateTempDataProvider { get; protected set; } = false;

        /// <summary>
        /// The length of time, in milliseconds, before the running schedule task times out. Set null to use default value
        /// </summary>
        public int? ScheduleTaskRunTimeout { get; protected set; } = null;

        /// <summary>
        /// Gets or sets a value of "Cache-Control" header value for static content (in seconds)
        /// </summary>
        public string StaticFilesCacheControl { get; protected set; } = "public,max-age=31536000";

        /// <summary>
        /// Get or set the blacklist of static file extension for plugin directories
        /// </summary>
        public string PluginStaticFileExtensionsBlacklist { get; protected set; } = "";

        /// <summary>
        /// Get or set a value indicating whether to serve files that don't have a recognized content-type
        /// </summary>
        public bool ServeUnknownFileTypes { get; protected set; } = false;

        /// <summary>
        /// Get or set a value indicating whether to use Autofac IoC container
        ///
        /// If value is set to false then the default .Net IoC container will be use
        /// </summary>
        public bool UseAutofac { get; set; } = true;
    }
}