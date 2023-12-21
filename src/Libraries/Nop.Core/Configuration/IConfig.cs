using Newtonsoft.Json;

namespace Nop.Core.Configuration;

/// <summary>
/// Represents a configuration from app settings
/// </summary>
public partial interface IConfig
{
    /// <summary>
    /// Gets a section name to load configuration
    /// </summary>
    [JsonIgnore]
    string Name => GetType().Name;

    /// <summary>
    /// Gets an order of configuration
    /// </summary>
    /// <returns>Order</returns>
    public int GetOrder() => 1;
}