using Nop.Core.Domain.Tax;
using Nop.Plugin.Api.DTOs.Taxes;
using Nop.Plugin.Api.AutoMapper;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class TaxCategoryDtoMappings
    {
        public static TaxCategoryDto ToDto(this TaxCategory taxCategory)
        {
            return taxCategory.MapTo<TaxCategory, TaxCategoryDto>();
        }
    }
}
