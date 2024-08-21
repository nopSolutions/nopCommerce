//This product includes GeoLite2 data created by MaxMind, available from http://www.maxmind.com
//more info: http://maxmind.github.io/GeoIP2-dotnet/
//more info: https://github.com/maxmind/GeoIP2-dotnet
//more info: http://dev.maxmind.com/geoip/geoip2/geolite2/

using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using MaxMind.GeoIP2.Responses;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;

namespace Nop.Services.Directory;

/// <summary>
/// GEO lookup service
/// </summary>
public partial class GeoLookupService : IGeoLookupService
{
    #region Fields

    protected readonly ILogger _logger;
    protected readonly INopFileProvider _fileProvider;

    #endregion

    #region Ctor

    public GeoLookupService(ILogger logger,
        INopFileProvider fileProvider)
    {
        _logger = logger;
        _fileProvider = fileProvider;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get information
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <returns>Information</returns>
    protected virtual CountryResponse GetInformation(string ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddress))
            return null;

        try
        {
            //This product includes GeoLite2 data created by MaxMind, available from http://www.maxmind.com
            var databasePath = _fileProvider.MapPath("~/App_Data/GeoLite2-Country.mmdb");
            var reader = new DatabaseReader(databasePath);
            var omni = reader.Country(ipAddress);

            return omni;
                
        }
        //catch (AddressNotFoundException exc)
        catch (GeoIP2Exception)
        {
            //address is not found
            //do not throw exceptions
            return null;
        }
        catch (Exception exc)
        {
            //do not throw exceptions
            _logger.Warning("Cannot load MaxMind record", exc);
            return null;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get country ISO code
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <returns>Country name</returns>
    public virtual string LookupCountryIsoCode(string ipAddress)
    {
        var response = GetInformation(ipAddress);
        if (response?.Country != null)
            return response.Country.IsoCode;

        return string.Empty;
    }

    /// <summary>
    /// Get country name
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <returns>Country name</returns>
    public virtual string LookupCountryName(string ipAddress)
    {
        var response = GetInformation(ipAddress);
        if (response?.Country != null)
            return response.Country.Name;

        return string.Empty;
    }

    #endregion
}