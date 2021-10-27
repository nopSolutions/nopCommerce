using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        protected IRepository<CategoryTemplate> CategoryTemplateRepository { get; }

        #endregion

        #region Ctor

        public CategoryTemplateService(IRepository<CategoryTemplate> categoryTemplateRepository)
        {
            CategoryTemplateRepository = categoryTemplateRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCategoryTemplateAsync(CategoryTemplate categoryTemplate)
        {
            await CategoryTemplateRepository.DeleteAsync(categoryTemplate);
        }

        /// <summary>
        /// Gets all category templates
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category templates
        /// </returns>
        public virtual async Task<IList<CategoryTemplate>> GetAllCategoryTemplatesAsync()
        {
            var templates = await CategoryTemplateRepository.GetAllAsync(query =>
            {
                return from pt in query
                    orderby pt.DisplayOrder, pt.Id
                    select pt;
            }, cache => default);

            return templates;
        }

        /// <summary>
        /// Gets a category template
        /// </summary>
        /// <param name="categoryTemplateId">Category template identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category template
        /// </returns>
        public virtual async Task<CategoryTemplate> GetCategoryTemplateByIdAsync(int categoryTemplateId)
        {
            return await CategoryTemplateRepository.GetByIdAsync(categoryTemplateId, cache => default);
        }

        /// <summary>
        /// Inserts category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCategoryTemplateAsync(CategoryTemplate categoryTemplate)
        {
            await CategoryTemplateRepository.InsertAsync(categoryTemplate);
        }

        /// <summary>
        /// Updates the category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCategoryTemplateAsync(CategoryTemplate categoryTemplate)
        {
            await CategoryTemplateRepository.UpdateAsync(categoryTemplate);
        }

        #endregion
    }
}