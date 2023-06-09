using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;

namespace Nop.Services.Directory
{
    /// <summary>
    /// Country service interface
    /// </summary>
    public partial interface ICountryService
    {
        /// <summary>
        /// Deletes a country
        /// </summary>
        /// <param name="country">Country</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteCountryAsync(Country country);

        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="languageId">Language identifier. It's used to sort countries by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the countries
        /// </returns>
        Task<IList<Country>> GetAllCountriesAsync(int languageId = 0, bool showHidden = false);

        /// <summary>
        /// Gets all countries that allow billing
        /// </summary>
        /// <param name="languageId">Language identifier. It's used to sort countries by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the countries
        /// </returns>
        Task<IList<Country>> GetAllCountriesForBillingAsync(int languageId = 0, bool showHidden = false);

        /// <summary>
        /// Gets all countries that allow shipping
        /// </summary>
        /// <param name="languageId">Language identifier. It's used to sort countries by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the countries
        /// </returns>
        Task<IList<Country>> GetAllCountriesForShippingAsync(int languageId = 0, bool showHidden = false);

        /// <summary>
        /// Gets a country by address 
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the country
        /// </returns>
        Task<Country> GetCountryByAddressAsync(Address address);

        /// <summary>
        /// Gets a country 
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the country
        /// </returns>
        Task<Country> GetCountryByIdAsync(int countryId);

        /// <summary>
        /// Get countries by identifiers
        /// </summary>
        /// <param name="countryIds">Country identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the countries
        /// </returns>
        Task<IList<Country>> GetCountriesByIdsAsync(int[] countryIds);

        /// <summary>
        /// Gets a country by two letter ISO code
        /// </summary>
        /// <param name="twoLetterIsoCode">Country two letter ISO code</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the country
        /// </returns>
        Task<Country> GetCountryByTwoLetterIsoCodeAsync(string twoLetterIsoCode);

        /// <summary>
        /// Gets a country by three letter ISO code
        /// </summary>
        /// <param name="threeLetterIsoCode">Country three letter ISO code</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the country
        /// </returns>
        Task<Country> GetCountryByThreeLetterIsoCodeAsync(string threeLetterIsoCode);

        /// <summary>
        /// Inserts a country
        /// </summary>
        /// <param name="country">Country</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertCountryAsync(Country country);

        /// <summary>
        /// Updates the country
        /// </summary>
        /// <param name="country">Country</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateCountryAsync(Country country);
    }
}