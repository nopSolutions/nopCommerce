<<<<<<< HEAD
namespace Nop.Services.Directory
{
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
=======
namespace Nop.Services.Directory
{
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
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}