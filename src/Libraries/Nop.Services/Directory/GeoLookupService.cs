//This product includes GeoLite2 data created by MaxMind, available from http://www.maxmind.com

using System;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using MaxMind.GeoIP2.Responses;
using Nop.Core;
using Nop.Services.Logging;

namespace Nop.Services.Directory
{
    /// <summary>
    /// GEO lookup service
    /// </summary>
    public partial class GeoLookupService : IGeoLookupService
    {
        #region Fields

        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public GeoLookupService(ILogger logger)
        {
            this._logger = logger;
        }

        #endregion

        #region Utilities

        protected virtual CountryResponse GetInformation(string ipAddress)
        {
            if (String.IsNullOrEmpty(ipAddress))
                return null;

            try
            {
                //This product includes GeoLite2 data created by MaxMind, available from http://www.maxmind.com
                var databasePath = CommonHelper.MapPath("~/App_Data/GeoLite2-Country.mmdb");
                var reader = new DatabaseReader(databasePath);
                var omni = reader.Country(ipAddress);
                return omni;
                //more info: http://maxmind.github.io/GeoIP2-dotnet/
                //more info: https://github.com/maxmind/GeoIP2-dotnet
                //more info: http://dev.maxmind.com/geoip/geoip2/geolite2/
                //Console.WriteLine(omni.Country.IsoCode); // 'US'
                //Console.WriteLine(omni.Country.Name); // 'United States'
                //Console.WriteLine(omni.Country.Names["zh-CN"]); // '美国'
                //Console.WriteLine(omni.MostSpecificSubdivision.Name); // 'Minnesota'
                //Console.WriteLine(omni.MostSpecificSubdivision.IsoCode); // 'MN'
                //Console.WriteLine(omni.City.Name); // 'Minneapolis'
                //Console.WriteLine(omni.Postal.Code); // '55455'
                //Console.WriteLine(omni.Location.Latitude); // 44.9733
                //Console.WriteLine(omni.Location.Longitude); // -93.2323
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
        /// Get country name
        /// </summary>
        /// <param name="ipAddress">IP address</param>
        /// <returns>Country name</returns>
        public virtual string LookupCountryIsoCode(string ipAddress)
        {
            var response = GetInformation(ipAddress);
            if (response != null && response.Country != null)
                return response.Country.IsoCode;

            return "";
        }

        /// <summary>
        /// Get country name
        /// </summary>
        /// <param name="ipAddress">IP address</param>
        /// <returns>Country name</returns>
        public virtual string LookupCountryName(string ipAddress)
        {
            var response = GetInformation(ipAddress);
            if (response != null && response.Country != null)
                return response.Country.Name;

            return "";
        }

        #endregion
    }
}