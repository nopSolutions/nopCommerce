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
    /// <returns>Country name</returns>
    string LookupCountryIsoCode(string ipAddress);

    /// <summary>
    /// Get country name
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <returns>Country name</returns>
    string LookupCountryName(string ipAddress);
}