using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.Jotform;

/// <summary>
/// Represents plugin settings
/// </summary>
public class JotformSettings : ISettings
{
    #region Properties

    /// <summary>
    /// Gets or sets the Jotform script
    /// </summary>
    public string EmbedCode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Jotform AI chatbot is enabled
    /// </summary>
    public bool Enabled { get; set; }

    #endregion
}