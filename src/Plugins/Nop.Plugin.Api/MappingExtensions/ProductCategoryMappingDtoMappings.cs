using Nop.Plugin.Api.AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTOs.ProductCategoryMappings;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ProductCategoryMappingDtoMappings
    {
        public static ProductCategoryMappingDto ToDto(this ProductCategory mapping)
        {
            return mapping.MapTo<ProductCategory, ProductCategoryMappingDto>();
        }
    }
}