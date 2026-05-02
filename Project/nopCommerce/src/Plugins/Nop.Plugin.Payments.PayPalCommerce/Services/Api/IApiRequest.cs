namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api;

/// <summary>
/// Represents API request
/// </summary>
public interface IApiRequest
{
    /// <summary>
    /// Gets the request path
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets the request method
    /// </summary>
    public string Method { get; }
}