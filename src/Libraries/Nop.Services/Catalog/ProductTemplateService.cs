using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product template service
    /// </summary>
    public partial class ProductTemplateService : IProductTemplateService
    {
        #region Constants
        private const string PRODUCTTEMPLATES_BY_ID_KEY = "Nop.producttemplate.id-{0}";
        private const string PRODUCTTEMPLATES_ALL_KEY = "Nop.producttemplate.all";
        private const string PRODUCTTEMPLATES_PATTERN_KEY = "Nop.producttemplate.";

        #endregion

        #region Fields

        private readonly IRepository<ProductTemplate> _productTemplateRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="productTemplateRepository">Product template repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ProductTemplateService(ICacheManager cacheManager,
            IRepository<ProductTemplate> productTemplateRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _productTemplateRepository = productTemplateRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        public virtual void DeleteProductTemplate(ProductTemplate productTemplate)
        {
            if (productTemplate == null)
                throw new ArgumentNullException("productTemplate");

            _productTemplateRepository.Delete(productTemplate);

            _cacheManager.RemoveByPattern(PRODUCTTEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productTemplate);
        }

        /// <summary>
        /// Gets all product templates
        /// </summary>
        /// <returns>Product templates</returns>
        public virtual IList<ProductTemplate> GetAllProductTemplates()
        {
            string key = PRODUCTTEMPLATES_ALL_KEY;
            return _cacheManager.Get(key, () =>
            {
                var query = from pt in _productTemplateRepository.Table
                            orderby pt.DisplayOrder
                            select pt;

                var templates = query.ToList();
                return templates;
            });
        }
 
        /// <summary>
        /// Gets a product template
        /// </summary>
        /// <param name="productTemplateId">Product template identifier</param>
        /// <returns>Product template</returns>
        public virtual ProductTemplate GetProductTemplateById(int productTemplateId)
        {
            if (productTemplateId == 0)
                return null;

            string key = string.Format(PRODUCTTEMPLATES_BY_ID_KEY, productTemplateId);
            return _cacheManager.Get(key, () =>
            {
                var template = _productTemplateRepository.GetById(productTemplateId);
                return template;
            });
        }

        /// <summary>
        /// Inserts product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        public virtual void InsertProductTemplate(ProductTemplate productTemplate)
        {
            if (productTemplate == null)
                throw new ArgumentNullException("productTemplate");

            _productTemplateRepository.Insert(productTemplate);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTTEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productTemplate);
        }

        /// <summary>
        /// Updates the product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        public virtual void UpdateProductTemplate(ProductTemplate productTemplate)
        {
            if (productTemplate == null)
                throw new ArgumentNullException("productTemplate");

            _productTemplateRepository.Update(productTemplate);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTTEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productTemplate);
        }
        
        #endregion
    }
}
