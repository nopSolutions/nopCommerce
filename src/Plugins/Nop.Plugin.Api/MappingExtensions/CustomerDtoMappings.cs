using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTO.Customers;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class CustomerDtoMappings
    {
        public static CustomerDto ToDto(this Customer customer)
        {
            return customer.MapTo<Customer, CustomerDto>();
        }
    }
}
