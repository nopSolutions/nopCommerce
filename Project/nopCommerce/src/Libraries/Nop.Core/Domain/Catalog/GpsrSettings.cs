using Nop.Core.Configuration;

namespace Nop.Core.Domain.Catalog;

/// <summary>
/// GPSR settings
/// </summary>
public partial class GpsrSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether GPSR is enabled
    /// </summary>
    public bool Enabled { get; set; }
}