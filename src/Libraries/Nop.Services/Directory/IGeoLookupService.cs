namespace Nop.Services.Directory;

/// <summary>
/// GEO lookup service
/// </summary>
public partial interface IGeoLookupService
{
    /// <summary>
    /// Get country ISO code
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the country name
    /// </returns>
    Task<string> LookupCountryIsoCodeAsync(string ipAddress);

    /// <summary>
    /// Get country name
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the country name
    /// </returns>
    Task<string> LookupCountryNameAsync(string ipAddress);
}