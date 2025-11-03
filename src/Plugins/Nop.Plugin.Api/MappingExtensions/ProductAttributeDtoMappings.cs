using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTO.ProductAttributes;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ProductAttributeDtoMappings
    {
        public static ProductAttributeDto ToDto(this ProductAttribute productAttribute)
        {
            return productAttribute.MapTo<ProductAttribute, ProductAttributeDto>();
        }
    }
}
