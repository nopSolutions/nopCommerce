
using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;


namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product tag service
    /// </summary>
    public partial class ProductTagService : IProductTagService
    {
        #region Fields

        private readonly IRepository<ProductTag> _productTagRepository;

        #endregion

        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="productTagRepository">Product tag repository</param>
        public ProductTagService(IRepository<ProductTag> productTagRepository)
        {
            this._productTagRepository = productTagRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public virtual void DeleteProductTag(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException("productTag");

            _productTagRepository.Delete(productTag);
        }

        /// <summary>
        /// Gets all product tags
        /// </summary>
        /// <returns>Product tags</returns>
        public virtual IList<ProductTag> GetAllProductTags()
        {
            var query = from pt in _productTagRepository.Table
                        orderby pt.ProductCount descending
                        select pt;
            var productTags = query.ToList();
            return productTags;
        }

        /// <summary>
        /// Gets product tag
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>Product tag</returns>
        public virtual ProductTag GetProductById(int productTagId)
        {
            if (productTagId == 0)
                return null;

            var productTag = _productTagRepository.GetById(productTagId);
            return productTag;
        }

        /// <summary>
        /// Gets product tag by name
        /// </summary>
        /// <param name="name">Product tag name</param>
        /// <returns>Product tag</returns>
        public virtual ProductTag GetProductTagByName(string name)
        {
            var query = from pt in _productTagRepository.Table
                        where pt.Name == name
                        select pt;

            var productTag = query.FirstOrDefault();
            return productTag;
        }

        /// <summary>
        /// Inserts a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public virtual void InsertProductTag(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException("productTag");

            _productTagRepository.Insert(productTag);
        }

        /// <summary>
        /// Updates the product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public virtual void UpdateProductTag(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException("productTag");

            _productTagRepository.Update(productTag);
        }

        /// <summary>
        /// Updates the product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public virtual void UpdateProductTagTotals(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException("productTag");

            int newTotal = productTag.Products.Where(p => !p.Deleted).Count();
            if (newTotal > 0)
            {
                productTag.ProductCount = newTotal;
                UpdateProductTag(productTag);
            }
            else
            {
                DeleteProductTag(productTag);
            }
        }
        
        #endregion
    }
}
