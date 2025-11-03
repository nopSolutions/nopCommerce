using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTO.Categories;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class CategoryDtoMappings
    {
        public static CategoryDto ToDto(this Category category)
        {
            return category.MapTo<Category, CategoryDto>();
        }

        public static Category ToEntity(this CategoryDto categoryDto)
        {
            return categoryDto.MapTo<CategoryDto, Category>();
        }
    }
}
