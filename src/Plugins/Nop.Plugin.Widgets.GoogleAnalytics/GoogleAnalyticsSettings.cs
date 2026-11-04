using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.GoogleAnalytics;

public class GoogleAnalyticsSettings : ISettings
{
    /// <summary>
    /// Gets or sets the Google Analytics ID
    /// </summary>
    public string GoogleId { get; set; }

    /// <summary>
    /// Gets or sets the Google Analytics API secret
    /// </summary>
    public string ApiSecret { get; set; }

    /// <summary>
    /// Gets or sets the Google Analytics tracking script code
    /// </summary>
    public string TrackingScript { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to enable ecommerce tracking
    /// </summary>
    public bool EnableEcommerce { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use sandbox mode for testing purpose
    /// </summary>
    public bool UseSandbox { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include tax in the tracking data
    /// </summary>
    public bool IncludingTax { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include customer identifier to script
    /// </summary>
    public bool IncludeCustomerId { get; set; }
}