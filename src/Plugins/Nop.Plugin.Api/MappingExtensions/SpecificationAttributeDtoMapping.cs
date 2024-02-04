using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTO.SpecificationAttributes;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class SpecificationAttributeDtoMappings
    {
        public static ProductSpecificationAttributeDto ToDto(this ProductSpecificationAttribute productSpecificationAttribute)
        {
            return productSpecificationAttribute.MapTo<ProductSpecificationAttribute, ProductSpecificationAttributeDto>();
        }

        public static SpecificationAttributeDto ToDto(this SpecificationAttribute specificationAttribute)
        {
            return specificationAttribute.MapTo<SpecificationAttribute, SpecificationAttributeDto>();
        }

        public static SpecificationAttributeOptionDto ToDto(this SpecificationAttributeOption specificationAttributeOption)
        {
            return specificationAttributeOption.MapTo<SpecificationAttributeOption, SpecificationAttributeOptionDto>();
        }
    }
}
