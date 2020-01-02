using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTOs.ProductManufacturerMappings;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ProductManufacturerMappingDtoMappings
    {
        public static ProductManufacturerMappingsDto ToDto(this ProductManufacturer mapping)
        {
            return mapping.MapTo<ProductManufacturer, ProductManufacturerMappingsDto>();
        }
    }
}