using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTO.Manufacturers;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ManufacturerDtoMappings
    {
        public static ManufacturerDto ToDto(this Manufacturer manufacturer)
        {
            return manufacturer.MapTo<Manufacturer, ManufacturerDto>();
        }
    }
}
