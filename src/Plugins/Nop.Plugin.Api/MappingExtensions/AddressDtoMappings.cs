using Nop.Core.Domain.Common;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTOs;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class AddressDtoMappings
    {
        public static AddressDto ToDto(this Address address)
        {
            return address.MapTo<Address, AddressDto>();
        }

        public static Address ToEntity(this AddressDto addressDto)
        {
            return addressDto.MapTo<AddressDto, Address>();
        }
    }
}