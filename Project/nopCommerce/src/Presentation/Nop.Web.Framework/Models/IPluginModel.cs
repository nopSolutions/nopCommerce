
namespace Nop.Web.Framework.Models;

/// <summary>
/// Represents a plugin model
/// </summary>
public partial interface IPluginModel
{
    /// <summary>
    /// Gets or sets a value indicating whether a plugin is active
    /// </summary>
    bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets a friendly name
    /// </summary>
    string FriendlyName { get; set; }

    /// <summary>
    /// Gets or sets a system name
    /// </summary>
    string SystemName { get; set; }

    /// <summary>
    /// Gets or sets a display order
    /// </summary>
    int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets a configuration URL
    /// </summary>
    string ConfigurationUrl { get; set; }

    /// <summary>
    /// Gets or sets a logo URL
    /// </summary>
    string LogoUrl { get; set; }
}