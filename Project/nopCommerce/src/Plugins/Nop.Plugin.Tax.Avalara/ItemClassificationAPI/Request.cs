using Newtonsoft.Json;

namespace Nop.Plugin.Tax.Avalara.ItemClassificationAPI;

/// <summary>
/// Represents base request to the service
/// </summary>
public abstract class Request
{
    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public abstract string Path { get; }

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public abstract string Method { get; }
}