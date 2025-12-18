using Nop.Core.Configuration;

namespace Nop.Plugin.Theme.KungFu;

public class ThemeKungFuSettings : ISettings
{
    public bool SyncAutomatically { get; set; }

    public DateTime? LastSyncedOnUtc { get; set; }

    public string LastSyncedVersion { get; set; }

    /// <summary>
    /// Gets or sets the Azure OpenAI endpoint URL
    /// </summary>
    public string AzureOpenAIEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the Azure OpenAI API key
    /// </summary>
    public string AzureOpenAIKey { get; set; }

    /// <summary>
    /// Gets or sets the Azure OpenAI deployment name
    /// </summary>
    public string AzureOpenAIDeploymentName { get; set; }

    /// <summary>
    /// Gets or sets whether to send AI sage messages after order payment
    /// </summary>
    public bool EnableAISageMessages { get; set; }
}
