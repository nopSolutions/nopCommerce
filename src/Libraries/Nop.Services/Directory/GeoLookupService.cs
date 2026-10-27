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

    protected static readonly object _locker = new();
    protected static DatabaseReader _databaseReader;
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
    /// Gets a cached DatabaseReader for the GeoLite2-Country database
    /// </summary>
    /// <returns>The cached DatabaseReader instance for the GeoLite2-Country.mmdb file.</returns>
    protected virtual DatabaseReader GetDatabaseReader()
    {
        if (_databaseReader != null)
            return _databaseReader;

        //prevent multi loading data
        lock (_locker)
        {
            var databasePath = _fileProvider.MapPath("~/App_Data/GeoLite2-Country.mmdb");

            //data can be loaded while we waited
            if (_databaseReader != null)
                return _databaseReader;

            _databaseReader = new DatabaseReader(databasePath);
            
            return _databaseReader;
        }
    }

    /// <summary>
    /// Get information
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the country information
    /// </returns>
    protected virtual async Task<CountryResponse> GetInformationAsync(string ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddress))
            return null;

        try
        {
            var reader = GetDatabaseReader();
            var omni = reader.Country(ipAddress);

            return omni;
        }
        catch (GeoIP2Exception)
        {
            //address is not found
            //do not throw exceptions
            return null;
        }
        catch (Exception exc)
        {
            //do not throw exceptions
            await _logger.WarningAsync("Cannot load MaxMind record", exc);
            return null;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get country ISO code
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the country name
    /// </returns>
    public virtual async Task<string> LookupCountryIsoCodeAsync(string ipAddress)
    {
        var response = await GetInformationAsync(ipAddress);

        return response?.Country?.IsoCode ?? string.Empty;
    }

    /// <summary>
    /// Get country name
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the country name
    /// </returns>
    public virtual async Task<string> LookupCountryNameAsync(string ipAddress)
    {
        var response = await GetInformationAsync(ipAddress);

        return response?.Country?.Name ?? string.Empty;
    }

    #endregion
}