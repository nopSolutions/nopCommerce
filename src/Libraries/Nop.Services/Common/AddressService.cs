using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Services.Directory;

namespace Nop.Services.Common
{
    /// <summary>
    /// Address service
    /// </summary>
    public partial class AddressService : IAddressService
    {
        #region Fields

        protected AddressSettings AddressSettings { get; }
        protected IAddressAttributeParser AddressAttributeParser { get; }
        protected IAddressAttributeService AddressAttributeService { get; }
        protected ICountryService CountryService { get; }
        protected IRepository<Address> AddressRepository { get; }
        protected IStateProvinceService StateProvinceService { get; }

        #endregion

        #region Ctor

        public AddressService(AddressSettings addressSettings,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            ICountryService countryService,
            IRepository<Address> addressRepository,
            IStateProvinceService stateProvinceService)
        {
            AddressSettings = addressSettings;
            AddressAttributeParser = addressAttributeParser;
            AddressAttributeService = addressAttributeService;
            CountryService = countryService;
            AddressRepository = addressRepository;
            StateProvinceService = stateProvinceService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes an address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteAddressAsync(Address address)
        {
            await AddressRepository.DeleteAsync(address);
        }

        /// <summary>
        /// Gets total number of addresses by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of addresses
        /// </returns>
        public virtual async Task<int> GetAddressTotalByCountryIdAsync(int countryId)
        {
            if (countryId == 0)
                return 0;

            var query = from a in AddressRepository.Table
                        where a.CountryId == countryId
                        select a;

            return await query.CountAsync();
        }

        /// <summary>
        /// Gets total number of addresses by state/province identifier
        /// </summary>
        /// <param name="stateProvinceId">State/province identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of addresses
        /// </returns>
        public virtual async Task<int> GetAddressTotalByStateProvinceIdAsync(int stateProvinceId)
        {
            if (stateProvinceId == 0)
                return 0;

            var query = from a in AddressRepository.Table
                        where a.StateProvinceId == stateProvinceId
                        select a;

            return await query.CountAsync();
        }

        /// <summary>
        /// Gets an address by address identifier
        /// </summary>
        /// <param name="addressId">Address identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address
        /// </returns>
        public virtual async Task<Address> GetAddressByIdAsync(int addressId)
        {
            return await AddressRepository.GetByIdAsync(addressId,
                cache => cache.PrepareKeyForShortTermCache(NopEntityCacheDefaults<Address>.ByIdCacheKey, addressId));
        }

        /// <summary>
        /// Inserts an address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertAddressAsync(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            address.CreatedOnUtc = DateTime.UtcNow;

            //some validation
            if (address.CountryId == 0)
                address.CountryId = null;
            if (address.StateProvinceId == 0)
                address.StateProvinceId = null;

            await AddressRepository.InsertAsync(address);
        }

        /// <summary>
        /// Updates the address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateAddressAsync(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            //some validation
            if (address.CountryId == 0)
                address.CountryId = null;
            if (address.StateProvinceId == 0)
                address.StateProvinceId = null;

            await AddressRepository.UpdateAsync(address);
        }

        /// <summary>
        /// Gets a value indicating whether address is valid (can be saved)
        /// </summary>
        /// <param name="address">Address to validate</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsAddressValidAsync(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            if (string.IsNullOrWhiteSpace(address.FirstName))
                return false;

            if (string.IsNullOrWhiteSpace(address.LastName))
                return false;

            if (string.IsNullOrWhiteSpace(address.Email))
                return false;

            if (AddressSettings.CompanyEnabled &&
                AddressSettings.CompanyRequired &&
                string.IsNullOrWhiteSpace(address.Company))
                return false;

            if (AddressSettings.StreetAddressEnabled &&
                AddressSettings.StreetAddressRequired &&
                string.IsNullOrWhiteSpace(address.Address1))
                return false;

            if (AddressSettings.StreetAddress2Enabled &&
                AddressSettings.StreetAddress2Required &&
                string.IsNullOrWhiteSpace(address.Address2))
                return false;

            if (AddressSettings.ZipPostalCodeEnabled &&
                AddressSettings.ZipPostalCodeRequired &&
                string.IsNullOrWhiteSpace(address.ZipPostalCode))
                return false;

            if (AddressSettings.CountryEnabled)
            {
                var country = await CountryService.GetCountryByAddressAsync(address);
                if (country == null)
                    return false;

                if (AddressSettings.StateProvinceEnabled)
                {
                    var states = await StateProvinceService.GetStateProvincesByCountryIdAsync(country.Id);
                    if (states.Any())
                    {
                        if (address.StateProvinceId == null || address.StateProvinceId.Value == 0)
                            return false;

                        var state = states.FirstOrDefault(x => x.Id == address.StateProvinceId.Value);
                        if (state == null)
                            return false;
                    }
                }
            }

            if (AddressSettings.CountyEnabled &&
                AddressSettings.CountyRequired &&
                string.IsNullOrWhiteSpace(address.County))
                return false;

            if (AddressSettings.CityEnabled &&
                AddressSettings.CityRequired &&
                string.IsNullOrWhiteSpace(address.City))
                return false;

            if (AddressSettings.PhoneEnabled &&
                AddressSettings.PhoneRequired &&
                string.IsNullOrWhiteSpace(address.PhoneNumber))
                return false;

            if (AddressSettings.FaxEnabled &&
                AddressSettings.FaxRequired &&
                string.IsNullOrWhiteSpace(address.FaxNumber))
                return false;

            var requiredAttributes = (await AddressAttributeService.GetAllAddressAttributesAsync()).Where(x => x.IsRequired);

            foreach (var requiredAttribute in requiredAttributes)
            {
                var value = AddressAttributeParser.ParseValues(address.CustomAttributes, requiredAttribute.Id);

                if (!value.Any() || string.IsNullOrEmpty(value[0]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Find an address
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <param name="phoneNumber">Phone number</param>
        /// <param name="email">Email</param>
        /// <param name="faxNumber">Fax number</param>
        /// <param name="company">Company</param>
        /// <param name="address1">Address 1</param>
        /// <param name="address2">Address 2</param>
        /// <param name="city">City</param>
        /// <param name="county">County</param>
        /// <param name="stateProvinceId">State/province identifier</param>
        /// <param name="zipPostalCode">Zip postal code</param>
        /// <param name="countryId">Country identifier</param>
        /// <param name="customAttributes">Custom address attributes (XML format)</param>
        /// <returns>Address</returns>
        public virtual Address FindAddress(List<Address> source, string firstName, string lastName, string phoneNumber, string email,
            string faxNumber, string company, string address1, string address2, string city, string county, int? stateProvinceId,
            string zipPostalCode, int? countryId, string customAttributes)
        {
            return source.Find(a => ((string.IsNullOrEmpty(a.FirstName) && string.IsNullOrEmpty(firstName)) || a.FirstName == firstName) &&
            ((string.IsNullOrEmpty(a.LastName) && string.IsNullOrEmpty(lastName)) || a.LastName == lastName) &&
            ((string.IsNullOrEmpty(a.PhoneNumber) && string.IsNullOrEmpty(phoneNumber)) || a.PhoneNumber == phoneNumber) &&
            ((string.IsNullOrEmpty(a.Email) && string.IsNullOrEmpty(email)) || a.Email == email) &&
            ((string.IsNullOrEmpty(a.FaxNumber) && string.IsNullOrEmpty(faxNumber)) || a.FaxNumber == faxNumber) &&
            ((string.IsNullOrEmpty(a.Company) && string.IsNullOrEmpty(company)) || a.Company == company) &&
            ((string.IsNullOrEmpty(a.Address1) && string.IsNullOrEmpty(address1)) || a.Address1 == address1) &&
            ((string.IsNullOrEmpty(a.Address2) && string.IsNullOrEmpty(address2)) || a.Address2 == address2) &&
            ((string.IsNullOrEmpty(a.City) && string.IsNullOrEmpty(city)) || a.City == city) &&
            ((string.IsNullOrEmpty(a.County) && string.IsNullOrEmpty(county)) || a.County == county) &&
            ((a.StateProvinceId == null && (stateProvinceId == null || stateProvinceId == 0)) || (a.StateProvinceId != null && a.StateProvinceId == stateProvinceId)) &&
            ((string.IsNullOrEmpty(a.ZipPostalCode) && string.IsNullOrEmpty(zipPostalCode)) || a.ZipPostalCode == zipPostalCode) &&
            ((a.CountryId == null && countryId == null) || (a.CountryId != null && a.CountryId == countryId)) &&
            //actually we should parse custom address attribute (in case if "Display order" is changed) and then compare
            //bu we simplify this process and simply compare their values in XML
            ((string.IsNullOrEmpty(a.CustomAttributes) && string.IsNullOrEmpty(customAttributes)) || a.CustomAttributes == customAttributes));
        }

        /// <summary>
        /// Clone address
        /// </summary>
        /// <returns>A deep copy of address</returns>
        public virtual Address CloneAddress(Address address)
        {
            var addr = new Address
            {
                FirstName = address.FirstName,
                LastName = address.LastName,
                Email = address.Email,
                Company = address.Company,
                CountryId = address.CountryId,
                StateProvinceId = address.StateProvinceId,
                County = address.County,
                City = address.City,
                Address1 = address.Address1,
                Address2 = address.Address2,
                ZipPostalCode = address.ZipPostalCode,
                PhoneNumber = address.PhoneNumber,
                FaxNumber = address.FaxNumber,
                CustomAttributes = address.CustomAttributes,
                CreatedOnUtc = address.CreatedOnUtc
            };

            return addr;
        }

        #endregion
    }
}