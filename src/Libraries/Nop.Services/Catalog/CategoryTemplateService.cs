using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Category template service
    /// </summary>
    public partial class CategoryTemplateService : ICategoryTemplateService
    {
        #region Constants
        private const string CATEGORYTEMPLATES_BY_ID_KEY = "Nop.categorytemplate.id-{0}";
        private const string CATEGORYTEMPLATES_ALL_KEY = "Nop.categorytemplate.all";
        private const string CATEGORYTEMPLATES_PATTERN_KEY = "Nop.categorytemplate.";

        #endregion

        #region Fields

        private readonly IRepository<CategoryTemplate> _categoryTemplateRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="categoryTemplateRepository">Category template repository</param>
        /// <param name="eventPublisher"></param>
        public CategoryTemplateService(ICacheManager cacheManager,
            IRepository<CategoryTemplate> categoryTemplateRepository, IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _categoryTemplateRepository = categoryTemplateRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        public virtual void DeleteCategoryTemplate(CategoryTemplate categoryTemplate)
        {
            if (categoryTemplate == null)
                throw new ArgumentNullException("categoryTemplate");

            _categoryTemplateRepository.Delete(categoryTemplate);

            _cacheManager.RemoveByPattern(CATEGORYTEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(categoryTemplate);
        }

        /// <summary>
        /// Gets all category templates
        /// </summary>
        /// <returns>Category templates</returns>
        public virtual IList<CategoryTemplate> GetAllCategoryTemplates()
        {
            string key = CATEGORYTEMPLATES_ALL_KEY;
            return _cacheManager.Get(key, () =>
            {
                var query = from pt in _categoryTemplateRepository.Table
                            orderby pt.DisplayOrder
                            select pt;

                var templates = query.ToList();
                return templates;
            });
        }
 
        /// <summary>
        /// Gets a category template
        /// </summary>
        /// <param name="categoryTemplateId">Category template identifier</param>
        /// <returns>Category template</returns>
        public virtual CategoryTemplate GetCategoryTemplateById(int categoryTemplateId)
        {
            if (categoryTemplateId == 0)
                return null;

            string key = string.Format(CATEGORYTEMPLATES_BY_ID_KEY, categoryTemplateId);
            return _cacheManager.Get(key, () =>
            {
                var template = _categoryTemplateRepository.GetById(categoryTemplateId);
                return template;
            });
        }

        /// <summary>
        /// Inserts category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        public virtual void InsertCategoryTemplate(CategoryTemplate categoryTemplate)
        {
            if (categoryTemplate == null)
                throw new ArgumentNullException("categoryTemplate");

            _categoryTemplateRepository.Insert(categoryTemplate);

            //cache
            _cacheManager.RemoveByPattern(CATEGORYTEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(categoryTemplate);
        }

        /// <summary>
        /// Updates the category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        public virtual void UpdateCategoryTemplate(CategoryTemplate categoryTemplate)
        {
            if (categoryTemplate == null)
                throw new ArgumentNullException("categoryTemplate");

            _categoryTemplateRepository.Update(categoryTemplate);

            //cache
            _cacheManager.RemoveByPattern(CATEGORYTEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(categoryTemplate);
        }
        
        #endregion
    }
}
