using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Api.DTO;
using Nop.Plugin.Api.DTOs.StateProvinces;

namespace Nop.Plugin.Api.Services
{
	public interface IAddressApiService
	{
		Task<IList<AddressDto>> GetAddressesByCustomerIdAsync(int customerId);
		Task<AddressDto> GetCustomerAddressAsync(int customerId, int addressId);
		Task<IList<CountryDto>> GetAllCountriesAsync(bool mustAllowBilling = false, bool mustAllowShipping = false);

		Task<CountryDto> GetCountryByIdAsync(int id);

		Task<IList<StateProvinceDto>> GetAllStateProvinceAsync();
		Task<StateProvinceDto> GetStateProvinceByIdAsync(int id);
		Task<AddressDto> GetAddressByIdAsync(int addressId);
	}
}
