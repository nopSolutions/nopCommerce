using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Category template service
    /// </summary>
    public partial class CategoryTemplateService : ICategoryTemplateService
    {
        #region Fields

        private readonly IRepository<CategoryTemplate> _categoryTemplateRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public CategoryTemplateService(IRepository<CategoryTemplate> categoryTemplateRepository,
            IStaticCacheManager staticCacheManager)
        {
            _categoryTemplateRepository = categoryTemplateRepository;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        public virtual void DeleteCategoryTemplate(CategoryTemplate categoryTemplate)
        {
            _categoryTemplateRepository.Delete(categoryTemplate);
        }

        /// <summary>
        /// Gets all category templates
        /// </summary>
        /// <returns>Category templates</returns>
        public virtual IList<CategoryTemplate> GetAllCategoryTemplates()
        {
            var templates = _categoryTemplateRepository.GetAll(query =>
            {
                return from pt in query
                    orderby pt.DisplayOrder, pt.Id
                    select pt;
            }, _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoryTemplatesAllCacheKey));

            return templates;
        }

        /// <summary>
        /// Gets a category template
        /// </summary>
        /// <param name="categoryTemplateId">Category template identifier</param>
        /// <returns>Category template</returns>
        public virtual CategoryTemplate GetCategoryTemplateById(int categoryTemplateId)
        {
            return _categoryTemplateRepository.GetById(categoryTemplateId);
        }

        /// <summary>
        /// Inserts category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        public virtual void InsertCategoryTemplate(CategoryTemplate categoryTemplate)
        {
            _categoryTemplateRepository.Insert(categoryTemplate);
        }

        /// <summary>
        /// Updates the category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        public virtual void UpdateCategoryTemplate(CategoryTemplate categoryTemplate)
        {
            _categoryTemplateRepository.Update(categoryTemplate);
        }

        #endregion
    }
}