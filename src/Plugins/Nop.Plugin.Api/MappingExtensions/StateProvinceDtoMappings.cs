using Nop.Core.Domain.Directory;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTO;
using Nop.Plugin.Api.DTOs.StateProvinces;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class StateProvinceDtoMappings
    {
        public static StateProvinceDto ToDto(this StateProvince address)
        {
            return address.MapTo<StateProvince, StateProvinceDto>();
        }

        public static StateProvince ToEntity(this StateProvinceDto address)
        {
            return address.MapTo<StateProvinceDto, StateProvince>();
        }
    }
}
