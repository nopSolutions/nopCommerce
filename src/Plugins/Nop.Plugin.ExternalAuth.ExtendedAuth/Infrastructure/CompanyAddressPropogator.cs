using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using iTextSharp.text;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Companies;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tasks;
using Nop.Services.Common;
using Nop.Services.Companies;
using Nop.Services.Customers;
using Nop.Services.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Nop.Plugin.ExternalAuth.ExtendedAuth.Infrastructure
{
    public record CompanyAddresses
    {
        public IList<Address> Addresses { get; set; }
    }
    
    public class CompanyAddressPropogator : IScheduleTask
    {
        public const string CompanyAddressPropogatorTaskName = "Company Address Propogator";
        public const string CompanyAddressPropogatorTask = "Nop.Plugin.ExternalAuth.ExtendedAuth.Infrastructure.CompanyAddressPropogator";
        private const string CompanyAddressesKey = "Addresses";
        
        private readonly IGenericAttributeService _genericAttribute;
        private readonly ICompanyService _companyService;
        private readonly IAddressService _addressService;
        private readonly ICustomerService _customerService;

        public async Task ExecuteAsync()
        {
            var companies = await _companyService.GetAllCompaniesAsync();
            foreach (var company in companies)
            {
                var companyAddressesSetting = await _genericAttribute.GetAttributeAsync<string>(company,
                    CompanyAddressesKey,
                    defaultValue: string.Empty);
                
                var companyAddresses =
                    JsonSerializer.Deserialize<CompanyAddresses>(
                        companyAddressesSetting, new JsonSerializerOptions() {IgnoreNullValues = true});
                if(companyAddresses == null || !companyAddresses.Addresses.Any())
                    continue;
                
                var customers = await _companyService.GetCompanyCustomersByCompanyIdAsync(company.Id);
                foreach (var customer in customers)
                {
                    var customerAddresses =
                        (await _customerService.GetCustomerAddressesByCustomerIdAsync(customer.CustomerId))
                        .Select(x => _addressService.GetAddressByIdAsync(x.AddressId).Result)
                        .ToList();

                    var missingCompanyAddresses = companyAddresses.Addresses.Where(companyAddr =>
                        !customerAddresses.Any(customerAddr =>
                            string.Equals(customerAddr.Address1, companyAddr.Address1) &&
                            string.Equals(customerAddr.Address2, companyAddr.Address2)));

                    foreach (var missingCompanyAddress in missingCompanyAddresses)
                    {
                        await _addressService.InsertAddressAsync(missingCompanyAddress);
                        var customerEntity = await _customerService.GetCustomerByIdAsync(customer.CustomerId);
                        await _customerService.InsertCustomerAddressAsync(customerEntity, missingCompanyAddress);
                        missingCompanyAddress.Id = 0;
                    }
                }
            }
        }

        public CompanyAddressPropogator(IGenericAttributeService genericAttribute, ICompanyService companyService, IAddressService addressService, ICustomerService customerService)
        {
            _genericAttribute = genericAttribute;
            _companyService = companyService;
            _addressService = addressService;
            _customerService = customerService;
        }
    }
}