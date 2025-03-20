﻿namespace Nop.Core.Configuration;

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
    /// Get or set a value indicating whether to serve files that don't have a recognized content-type
    /// </summary>
    public bool ServeUnknownFileTypes { get; protected set; } = false;

    /// <summary>
    /// Get or set a value indicating whether to use Autofac IoC container
    ///
    /// If value is set to false then the default .Net IoC container will be use
    /// </summary>
    public bool UseAutofac { get; set; } = true;

    /// <summary>
    /// Maximum number of permit counters that can be allowed in a window (1 minute).
    /// Must be set to a value > 0 by the time these options are passed to the constructor of <see cref="FixedWindowRateLimiter"/>.
    /// If set to 0 than limitation is off
    /// </summary>
    public int PermitLimit { get; set; } = 0;

    /// <summary>
    /// Maximum cumulative permit count of queued acquisition requests.
    /// Must be set to a value >= 0 by the time these options are passed to the constructor of <see cref="FixedWindowRateLimiter"/>.
    /// If set to 0 than Queue is off
    /// </summary>
    public int QueueCount { get; set; } = 0;

    /// <summary>
    /// Default status code to set on the response when a request is rejected.
    /// </summary>
    public int RejectionStatusCode { get; set; } = 503;
}