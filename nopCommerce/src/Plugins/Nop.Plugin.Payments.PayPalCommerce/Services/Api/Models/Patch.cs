using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the patch object to apply partial updates to resources
/// </summary>
public class Patch<TValue> where TValue : class
{
    #region Properties

    /// <summary>
    /// Gets or sets the operation.
    /// </summary>
    [JsonProperty(PropertyName = "op")]
    public string Op { get; set; }

    /// <summary>
    /// Gets or sets the [JSON Pointer](https://tools.ietf.org/html/rfc6901) to the target document location at which to complete the operation.
    /// </summary>
    [JsonProperty(PropertyName = "path")]
    public string Path { get; set; }

    /// <summary>
    /// Gets or sets the value to apply. The `remove`, `copy`, and `move` operations do not require a value.
    /// </summary>
    [JsonProperty(PropertyName = "value")]
    public TValue Value { get; set; }

    /// <summary>
    /// Gets or sets the [JSON Pointer](https://tools.ietf.org/html/rfc6901) to the target document location from which to `move` the value.
    /// </summary>
    [JsonProperty(PropertyName = "from")]
    public string From { get; set; }

    #endregion
}