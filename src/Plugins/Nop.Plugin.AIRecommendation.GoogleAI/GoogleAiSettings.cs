using Nop.Core.Configuration;

namespace Nop.Plugin.AIRecommendation.GoogleAI;

/// <summary>
/// Represents plugin settings
/// </summary>
public class GoogleAiSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether the plugin is enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the Google Cloud project ID
    /// </summary>
    public string ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the Google Cloud location ID
    /// </summary>
    public string LocationId { get; set; }

    /// <summary>
    /// Gets or sets the Google Cloud catalog ID
    /// </summary>
    public string CatalogId { get; set; }

    /// <summary>
    /// Gets or sets the Google Cloud branch ID
    /// </summary>
    public string BranchId { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether to log requests to Google AI API
    /// for debugging and testing purposes
    /// </summary>
    public bool LogRequests { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether synchronization with Google AI is allowed
    /// </summary>
    public bool SyncAllowed { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether allowed to searched products by Google AI
    /// </summary>
    public bool SearchAllowed { get; set; }

    #region Advanced

    /// <summary>
    /// Gets or sets the page size to import products
    /// </summary>
    public int ImportPageSize { get; set; }

    #endregion
}