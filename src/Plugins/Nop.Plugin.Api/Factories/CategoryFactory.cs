using System;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;

namespace Nop.Plugin.Api.Factories
{
    public class CategoryFactory : IFactory<Category>
    {
        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly CatalogSettings _catalogSettings;

        public CategoryFactory(ICategoryTemplateService categoryTemplateService, CatalogSettings catalogSettings)
        {
            _categoryTemplateService = categoryTemplateService;
            _catalogSettings = catalogSettings;
        }

        public Category Initialize()
        {
            // TODO: cache the default entity.
            var defaultCategory = new Category();

            // Set the first template as the default one.
            var firstTemplate = _categoryTemplateService.GetAllCategoryTemplates().FirstOrDefault();

            if (firstTemplate != null)
            {
                defaultCategory.CategoryTemplateId = firstTemplate.Id;
            }
            
            //default values
            defaultCategory.PageSize = _catalogSettings.DefaultCategoryPageSize;
            defaultCategory.PageSizeOptions = _catalogSettings.DefaultCategoryPageSizeOptions;
            defaultCategory.Published = true;
            defaultCategory.IncludeInTopMenu = true;
            defaultCategory.AllowCustomersToSelectPageSize = true;

            defaultCategory.CreatedOnUtc = DateTime.UtcNow;
            defaultCategory.UpdatedOnUtc = DateTime.UtcNow;

            return defaultCategory;
        }
    }
}