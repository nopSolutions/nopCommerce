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

        private readonly IRepository<ProductTemplate> _productTemplateRepository;

        #endregion

        #region Ctor

        public ProductTemplateService(IRepository<ProductTemplate> productTemplateRepository)
        {
            _productTemplateRepository = productTemplateRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        public virtual async Task DeleteProductTemplateAsync(ProductTemplate productTemplate)
        {
            await _productTemplateRepository.DeleteAsync(productTemplate);
        }

        /// <summary>
        /// Gets all product templates
        /// </summary>
        /// <returns>Product templates</returns>
        public virtual async Task<IList<ProductTemplate>> GetAllProductTemplatesAsync()
        {
            var templates = await _productTemplateRepository.GetAllAsync(query =>
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
        /// <returns>Product template</returns>
        public virtual async Task<ProductTemplate> GetProductTemplateByIdAsync(int productTemplateId)
        {
            return await _productTemplateRepository.GetByIdAsync(productTemplateId, cache => default);
        }

        /// <summary>
        /// Inserts product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        public virtual async Task InsertProductTemplateAsync(ProductTemplate productTemplate)
        {
            await _productTemplateRepository.InsertAsync(productTemplate);
        }

        /// <summary>
        /// Updates the product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        public virtual async Task UpdateProductTemplateAsync(ProductTemplate productTemplate)
        {
            await _productTemplateRepository.UpdateAsync(productTemplate);
        }

        #endregion
    }
}