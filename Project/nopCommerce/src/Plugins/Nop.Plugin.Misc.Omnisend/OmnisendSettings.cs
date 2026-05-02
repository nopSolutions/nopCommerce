using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.Omnisend;

/// <summary>
/// Represents plugin settings
/// </summary>
public class OmnisendSettings : ISettings
{
    /// <summary>
    /// Gets or sets the API key
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use tracking
    /// </summary>
    public bool UseTracking { get; set; }

    #region Advanced

    /// <summary>
    /// Gets or sets the BrandId
    /// </summary>
    public string BrandId { get; set; }

    /// <summary>
    /// Gets or sets the tracking script
    /// </summary>
    public string TrackingScript { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to log request/response details for debug purposes
    /// </summary>
    public bool LogRequests { get; set; }

    /// <summary>
    /// Gets or sets a period (in seconds) before the request times out
    /// </summary>
    public int? RequestTimeout { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to log request errors
    /// </summary>
    public bool LogRequestErrors { get; set; }

    /// <summary>
    /// Gets or sets a page size to synchronize
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the batches Ids
    /// </summary>
    public List<string> BatchesIds { get; set; } = new();

    /// <summary>
    /// Gets or sets the product script
    /// </summary> 
    public string ProductScript { get; set; }

    /// <summary>
    /// Gets or sets the identify contact script
    /// </summary> 
    public string IdentifyContactScript { get; set; }

    #endregion
}