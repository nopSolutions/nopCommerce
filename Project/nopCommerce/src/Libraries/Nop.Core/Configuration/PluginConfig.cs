namespace Nop.Core.Configuration;

/// <summary>
/// Represents plugin configuration parameters
/// </summary>
public partial class PluginConfig : IConfig
{
    /// <summary>
    /// Gets or sets a value indicating whether to load an assembly into the load-from context, bypassing some security checks.
    /// </summary>
    public bool UseUnsafeLoadAssembly { get; set; } = true;
}