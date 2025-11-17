using Nop.Core.Configuration;

namespace Nop.Core.Domain.Catalog;

/// <summary>
/// GSPR settings
/// </summary>
public partial class GsprSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether GSPR is enabled
    /// </summary>
    public bool Enabled { get; set; }
}