using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Data;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product template service
    /// </summary>
    public partial class ProductTemplateService : IProductTemplateService
    {
        #region Fields

        protected IRepository<ProductTemplate> ProductTemplateRepository { get; }

        #endregion

        #region Ctor

        public ProductTemplateService(IRepository<ProductTemplate> productTemplateRepository)
        {
            ProductTemplateRepository = productTemplateRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductTemplateAsync(ProductTemplate productTemplate)
        {
            await ProductTemplateRepository.DeleteAsync(productTemplate);
        }

        /// <summary>
        /// Gets all product templates
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product templates
        /// </returns>
        public virtual async Task<IList<ProductTemplate>> GetAllProductTemplatesAsync()
        {
            var templates = await ProductTemplateRepository.GetAllAsync(query =>
            {
                return from pt in query
                    orderby pt.DisplayOrder, pt.Id
                    select pt;
            }, cache => default);

            return templates;
        }

        /// <summary>
        /// Gets a product template
        /// </summary>
        /// <param name="productTemplateId">Product template identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product template
        /// </returns>
        public virtual async Task<ProductTemplate> GetProductTemplateByIdAsync(int productTemplateId)
        {
            return await ProductTemplateRepository.GetByIdAsync(productTemplateId, cache => default);
        }

        /// <summary>
        /// Inserts product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertProductTemplateAsync(ProductTemplate productTemplate)
        {
            await ProductTemplateRepository.InsertAsync(productTemplate);
        }

        /// <summary>
        /// Updates the product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateProductTemplateAsync(ProductTemplate productTemplate)
        {
            await ProductTemplateRepository.UpdateAsync(productTemplate);
        }

        #endregion
    }
}