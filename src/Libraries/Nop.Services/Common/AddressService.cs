using System;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Services.Directory;
using Nop.Services.Events;

namespace Nop.Services.Common
{
    /// <summary>
    /// Address service
    /// </summary>
    public partial class AddressService : IAddressService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : address ID
        /// </remarks>
        private const string ADDRESSES_BY_ID_KEY = "Nop.address.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string ADDRESSES_PATTERN_KEY = "Nop.address.";

        #endregion

        #region Fields

        private readonly IRepository<Address> _addressRepository;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IEventPublisher _eventPublisher;
        private readonly AddressSettings _addressSettings;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="addressRepository">Address repository</param>
        /// <param name="countryService">Country service</param>
        /// <param name="stateProvinceService">State/province service</param>
        /// <param name="addressAttributeService">Address attribute service</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="addressSettings">Address settings</param>
        public AddressService(ICacheManager cacheManager,
            IRepository<Address> addressRepository,
            ICountryService countryService, 
            IStateProvinceService stateProvinceService,
            IAddressAttributeService addressAttributeService,
            IEventPublisher eventPublisher, 
            AddressSettings addressSettings)
        {
            this._cacheManager = cacheManager;
            this._addressRepository = addressRepository;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._addressAttributeService = addressAttributeService;
            this._eventPublisher = eventPublisher;
            this._addressSettings = addressSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes an address
        /// </summary>
        /// <param name="address">Address</param>
        public virtual void DeleteAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            _addressRepository.Delete(address);

            //cache
            _cacheManager.RemoveByPattern(ADDRESSES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(address);
        }

        /// <summary>
        /// Gets total number of addresses by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Number of addresses</returns>
        public virtual int GetAddressTotalByCountryId(int countryId)
        {
            if (countryId == 0)
                return 0;

            var query = from a in _addressRepository.Table
                        where a.CountryId == countryId
                        select a;
            return query.Count();
        }

        /// <summary>
        /// Gets total number of addresses by state/province identifier
        /// </summary>
        /// <param name="stateProvinceId">State/province identifier</param>
        /// <returns>Number of addresses</returns>
        public virtual int GetAddressTotalByStateProvinceId(int stateProvinceId)
        {
            if (stateProvinceId == 0)
                return 0;

            var query = from a in _addressRepository.Table
                        where a.StateProvinceId == stateProvinceId
                        select a;
            return query.Count();
        }

        /// <summary>
        /// Gets an address by address identifier
        /// </summary>
        /// <param name="addressId">Address identifier</param>
        /// <returns>Address</returns>
        public virtual Address GetAddressById(int addressId)
        {
            if (addressId == 0)
                return null;

            string key = string.Format(ADDRESSES_BY_ID_KEY, addressId);
            return _cacheManager.Get(key, () => _addressRepository.GetById(addressId));
        }

        /// <summary>
        /// Inserts an address
        /// </summary>
        /// <param name="address">Address</param>
        public virtual void InsertAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            
            address.CreatedOnUtc = DateTime.UtcNow;

            //some validation
            if (address.CountryId == 0)
                address.CountryId = null;
            if (address.StateProvinceId == 0)
                address.StateProvinceId = null;

            _addressRepository.Insert(address);

            //cache
            _cacheManager.RemoveByPattern(ADDRESSES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(address);
        }

        /// <summary>
        /// Updates the address
        /// </summary>
        /// <param name="address">Address</param>
        public virtual void UpdateAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            //some validation
            if (address.CountryId == 0)
                address.CountryId = null;
            if (address.StateProvinceId == 0)
                address.StateProvinceId = null;

            _addressRepository.Update(address);

            //cache
            _cacheManager.RemoveByPattern(ADDRESSES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(address);
        }
        
        /// <summary>
        /// Gets a value indicating whether address is valid (can be saved)
        /// </summary>
        /// <param name="address">Address to validate</param>
        /// <returns>Result</returns>
        public virtual bool IsAddressValid(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            if (String.IsNullOrWhiteSpace(address.FirstName))
                return false;

            if (String.IsNullOrWhiteSpace(address.LastName))
                return false;

            if (String.IsNullOrWhiteSpace(address.Email))
                return false;

            if (_addressSettings.CompanyEnabled &&
                _addressSettings.CompanyRequired &&
                String.IsNullOrWhiteSpace(address.Company))
                return false;

            if (_addressSettings.StreetAddressEnabled &&
                _addressSettings.StreetAddressRequired &&
                String.IsNullOrWhiteSpace(address.Address1))
                return false;

            if (_addressSettings.StreetAddress2Enabled &&
                _addressSettings.StreetAddress2Required &&
                String.IsNullOrWhiteSpace(address.Address2))
                return false;

            if (_addressSettings.ZipPostalCodeEnabled &&
                _addressSettings.ZipPostalCodeRequired &&
                String.IsNullOrWhiteSpace(address.ZipPostalCode))
                return false;


            if (_addressSettings.CountryEnabled)
            {
                if (address.CountryId == null || address.CountryId.Value == 0)
                    return false;

                var country = _countryService.GetCountryById(address.CountryId.Value);
                if (country == null)
                    return false;

                if (_addressSettings.StateProvinceEnabled)
                {
                    var states = _stateProvinceService.GetStateProvincesByCountryId(country.Id);
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

            if (_addressSettings.CityEnabled &&
                _addressSettings.CityRequired &&
                String.IsNullOrWhiteSpace(address.City))
                return false;

            if (_addressSettings.PhoneEnabled &&
                _addressSettings.PhoneRequired &&
                String.IsNullOrWhiteSpace(address.PhoneNumber))
                return false;

            if (_addressSettings.FaxEnabled &&
                _addressSettings.FaxRequired &&
                String.IsNullOrWhiteSpace(address.FaxNumber))
                return false;

            var attributes = _addressAttributeService.GetAllAddressAttributes();
            if (attributes.Any(x => x.IsRequired))
                return false;

            return true;
        }
        
        #endregion
    }
}