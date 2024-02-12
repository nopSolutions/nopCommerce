using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Plugin.Api.DTO;
using Nop.Plugin.Api.DTOs.StateProvinces;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Services.Customers;
using Nop.Services.Directory;

namespace Nop.Plugin.Api.Services
{
	public class AddressApiService : IAddressApiService
	{
        private readonly IStaticCacheManager _cacheManager;
		private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<CustomerAddressMapping> _customerAddressMappingRepository;

        public AddressApiService(
            IRepository<Address> addressRepository,
            IRepository<CustomerAddressMapping> customerAddressMappingRepository,
            IStaticCacheManager staticCacheManager,
            ICountryService countryService,
            IStateProvinceService stateProvinceService)
        {
            _addressRepository = addressRepository;
            _customerAddressMappingRepository = customerAddressMappingRepository;
            _cacheManager = staticCacheManager;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
        }

        /// <summary>
        /// Gets a list of addresses mapped to customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public async Task<IList<AddressDto>> GetAddressesByCustomerIdAsync(int customerId)
        {
            var query = from address in _addressRepository.Table
                        join cam in _customerAddressMappingRepository.Table on address.Id equals cam.AddressId
                        where cam.CustomerId == customerId
                        select address;

            var key = _cacheManager.PrepareKey(NopCustomerServicesDefaults.CustomerAddressesCacheKey, customerId);

            var addresses = await _cacheManager.GetAsync(key, async () => await query.ToListAsync());
            return addresses.Select(a => a.ToDto()).ToList();
        }

		/// <summary>
		/// Gets a address mapped to customer
		/// </summary>
		/// <param name="customerId">Customer identifier</param>
		/// <param name="addressId">Address identifier</param>
		/// <returns>
		/// A task that represents the asynchronous operation
		/// The task result contains the result
		/// </returns>
		public async Task<AddressDto> GetCustomerAddressAsync(int customerId, int addressId)
        {
            var query = from address in _addressRepository.Table
                        join cam in _customerAddressMappingRepository.Table on address.Id equals cam.AddressId
                        where cam.CustomerId == customerId && address.Id == addressId
                        select address;

            var key = _cacheManager.PrepareKey(NopCustomerServicesDefaults.CustomerAddressCacheKey, customerId, addressId);

            var addressEntity = await _cacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
            return addressEntity?.ToDto();
        }

		public async Task<IList<CountryDto>> GetAllCountriesAsync(bool mustAllowBilling = false, bool mustAllowShipping = false)
		{
            IEnumerable<Country> countries = await _countryService.GetAllCountriesAsync();
            if (mustAllowBilling)
                countries = countries.Where(c => c.AllowsBilling);
            if (mustAllowShipping)
                countries = countries.Where(c => c.AllowsShipping);
            return countries.Select(c => c.ToDto()).ToList();
        }

        public async Task<CountryDto> GetCountryByIdAsync(int id)
        {
            var country = await _countryService.GetCountryByIdAsync(id);
            return country?.ToDto();
        }

        public async Task<IList<StateProvinceDto>> GetAllStateProvinceAsync()
        {
            IEnumerable<StateProvince> stateProvinces = await _stateProvinceService.GetStateProvincesAsync();
            return stateProvinces.Select(c => c.ToDto()).ToList();
        }

        public async Task<StateProvinceDto> GetStateProvinceByIdAsync(int id)
        {
            var province = await _stateProvinceService.GetStateProvinceByIdAsync(id);
            return province?.ToDto();
        }


        public async Task<AddressDto> GetAddressByIdAsync(int addressId)
		{
            var query = from address in _addressRepository.Table
                        where address.Id == addressId
                        select address;
            var addressEntity = await query.FirstOrDefaultAsync();
            return addressEntity?.ToDto();
        }
	}
}
