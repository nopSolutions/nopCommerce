using System.Collections.Generic;
using System.Linq;
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
        public virtual void DeleteProductTemplate(ProductTemplate productTemplate)
        {
            _productTemplateRepository.Delete(productTemplate);
        }

        /// <summary>
        /// Gets all product templates
        /// </summary>
        /// <returns>Product templates</returns>
        public virtual IList<ProductTemplate> GetAllProductTemplates()
        {
            var templates = _productTemplateRepository.GetAll(query =>
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
        public virtual ProductTemplate GetProductTemplateById(int productTemplateId)
        {
            return _productTemplateRepository.GetById(productTemplateId, cache => default);
        }

        /// <summary>
        /// Inserts product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        public virtual void InsertProductTemplate(ProductTemplate productTemplate)
        {
            _productTemplateRepository.Insert(productTemplate);
        }

        /// <summary>
        /// Updates the product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        public virtual void UpdateProductTemplate(ProductTemplate productTemplate)
        {
            _productTemplateRepository.Update(productTemplate);
        }

        #endregion
    }
}