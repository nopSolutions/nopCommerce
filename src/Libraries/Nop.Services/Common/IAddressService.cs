

using Nop.Core.Domain.Common;

namespace Nop.Services.Common
{
    /// <summary>
    /// Address service interface
    /// </summary>
    public partial interface IAddressService
    {
        /// <summary>
        /// Deletes an address
        /// </summary>
        /// <param name="address">Address</param>
        void DeleteAddress(Address address);

        /// <summary>
        /// Gets total number of addresses by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Number of addresses</returns>
        int GetAddressTotalByCountryId(int countryId);

        /// <summary>
        /// Gets total number of addresses by state/province identifier
        /// </summary>
        /// <param name="stateProvinceId">State/province identifier</param>
        /// <returns>Number of addresses</returns>
        int GetAddressTotalByStateProvinceId(int stateProvinceId);

        /// <summary>
        /// Gets an address by address identifier
        /// </summary>
        /// <param name="addressId">Address identifier</param>
        /// <returns>Address</returns>
        Address GetAddressById(int addressId);

        /// <summary>
        /// Inserts an address
        /// </summary>
        /// <param name="address">Address</param>
        void InsertAddress(Address address);

        /// <summary>
        /// Updates the address
        /// </summary>
        /// <param name="address">Address</param>
        void UpdateAddress(Address address);

        /// <summary>
        /// Gets a value indicating whether address is valid (can be saved)
        /// </summary>
        /// <param name="address">Address to validate</param>
        /// <returns>Result</returns>
        bool IsAddressValid(Address address);
    }
}