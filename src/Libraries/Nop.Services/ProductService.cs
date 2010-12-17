//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Data;
using Nop.Core;

namespace Nop.Services
{
    /// <summary>
    /// Product service
    /// </summary>
    public partial class ProductService : IProductService
    {
        #region Constants
        private const string PRODUCTS_BY_ID_KEY = "Nop.product.id-{0}";
        private const string PRODUCTVARIANTS_ALL_KEY = "Nop.productvariant.all-{0}-{1}";
        private const string PRODUCTVARIANTS_BY_ID_KEY = "Nop.productvariant.id-{0}";
        private const string PRODUCTS_PATTERN_KEY = "Nop.product.";
        private const string PRODUCTVARIANTS_PATTERN_KEY = "Nop.productvariant.";
        #endregion

        #region Fields

        private readonly IRepository<Product> _productRespository;
        private readonly IRepository<LocalizedProduct> _localizedProductRespository;
        private readonly IRepository<ProductVariant> _productVariantRespository;
        private readonly IRepository<LocalizedProductVariant> _localizedProductVariantRespository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="productRespository">Product repository</param>
        /// <param name="localizedProductRespository">Localized product repository</param>
        /// <param name="productVariantRespository">Product variant repository</param>
        /// <param name="localizedProductVariantRespository">Localized product variant repository</param>
        public ProductService(ICacheManager cacheManager,
            IRepository<Product> productRespository,
            IRepository<LocalizedProduct> localizedProductRespository,
            IRepository<ProductVariant> productVariantRespository,
            IRepository<LocalizedProductVariant> localizedProductVariantRespository)
        {
            this._cacheManager = cacheManager;
            this._productRespository = productRespository;
            this._localizedProductRespository = localizedProductRespository;
            this._productVariantRespository = productVariantRespository;
            this._localizedProductVariantRespository = localizedProductVariantRespository;
        }

        #endregion

        #region Methods

        #region Products

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="product">Product</param>
        public void DeleteProduct(Product product)
        {
            if (product == null)
                return;

            product.Deleted = true;
            //delete product
            UpdateProduct(product);

            //delete product variants
            foreach (var productVariant in product.ProductVariants)
                DeleteProductVariant(productVariant);
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>Product collection</returns>
        public List<Product> GetAllProducts()
        {
            //TODO: use bool showHidden = NopContext.Current.IsAdmin;
            bool showHidden = true;
            return GetAllProducts(showHidden);
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product collection</returns>
        public List<Product> GetAllProducts(bool showHidden)
        {
            var query = from p in _productRespository.Table
                        orderby p.Name
                        where (showHidden || p.Published) &&
                        !p.Deleted
                        select p;
            var products = query.ToList();
            return products;
        }

        /// <summary>
        /// Gets all products displayed on the home page
        /// </summary>
        /// <returns>Product collection</returns>
        public List<Product> GetAllProductsDisplayedOnHomePage()
        {
            //TODO: use bool showHidden = NopContext.Current.IsAdmin;
            bool showHidden = true;

            var query = from p in _productRespository.Table
                        orderby p.Name
                        where (showHidden || p.Published) &&
                        !p.Deleted &&
                        p.ShowOnHomePage
                        select p;
            var products = query.ToList();
            return products;
        }
        
        /// <summary>
        /// Gets product
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product</returns>
        public Product GetProductById(int productId)
        {
            if (productId == 0)
                return null;

            string key = string.Format(PRODUCTS_BY_ID_KEY, productId);
            object obj2 = _cacheManager.Get(key);
            if (obj2 != null)
            {
                return (Product)obj2;
            }

            var product = _productRespository.GetById(productId);

                _cacheManager.Add(key, product);
            
            return product;
        }
        
        /// <summary>
        /// Inserts a product
        /// </summary>
        /// <param name="product">Product</param>
        public void InsertProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            _productRespository.Insert(product);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the product
        /// </summary>
        /// <param name="product">Product</param>
        public void UpdateProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            _productRespository.Update(product);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
        }

        /// <summary>
        /// Gets localized product by id
        /// </summary>
        /// <param name="localizedProductId">Localized product identifier</param>
        /// <returns>Product content</returns>
        public LocalizedProduct GetLocalizedProductById(int localizedProductId)
        {
            if (localizedProductId == 0)
                return null;

            var productLocalized = _localizedProductRespository.GetById(localizedProductId);
            return productLocalized;
        }

        /// <summary>
        /// Gets localized product by product id
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product content</returns>
        public List<LocalizedProduct> GetLocalizedProductByProductId(int productId)
        {
            if (productId == 0)
                return new List<LocalizedProduct>();
            
            var query = from pl in _localizedProductRespository.Table
                        where pl.ProductId == productId
                        select pl;
            var content = query.ToList();
            return content;
        }

        /// <summary>
        /// Gets localized product by product id and language id
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Product content</returns>
        public LocalizedProduct GetLocalizedProductByProductIdAndLanguageId(int productId, int languageId)
        {
            if (productId == 0 || languageId == 0)
                return null;

            var query = from pl in _localizedProductRespository.Table
                        orderby pl.Id
                        where pl.ProductId == productId &&
                        pl.LanguageId == languageId
                        select pl;
            var productLocalized = query.FirstOrDefault();
            return productLocalized;
        }

        /// <summary>
        /// Inserts a localized product
        /// </summary>
        /// <param name="localizedProduct">Product content</param>
        public void InsertLocalizedProduct(LocalizedProduct localizedProduct)
        {
            if (localizedProduct == null)
                throw new ArgumentNullException("localizedProduct");

            _localizedProductRespository.Insert(localizedProduct);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
        }

        /// <summary>
        /// Update a localized product
        /// </summary>
        /// <param name="localizedProduct">Product content</param>
        public void UpdateLocalizedProduct(LocalizedProduct localizedProduct)
        {
            if (localizedProduct == null)
                throw new ArgumentNullException("localizedProduct");

            bool allFieldsAreEmpty = string.IsNullOrEmpty(localizedProduct.Name) &&
                                     string.IsNullOrEmpty(localizedProduct.ShortDescription) &&
                                     string.IsNullOrEmpty(localizedProduct.FullDescription) &&
                                     string.IsNullOrEmpty(localizedProduct.MetaKeywords) &&
                                     string.IsNullOrEmpty(localizedProduct.MetaDescription) &&
                                     string.IsNullOrEmpty(localizedProduct.MetaTitle) &&
                                     string.IsNullOrEmpty(localizedProduct.SeName);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                _localizedProductRespository.Delete(localizedProduct);
            }
            else
            {
                _localizedProductRespository.Update(localizedProduct);
            }

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
        }
        
        #endregion

        #region Product variants
        
        /// <summary>
        /// Get low stock product variants
        /// </summary>
        /// <returns>Result</returns>
        public List<ProductVariant> GetLowStockProductVariants()
        {
            var query = from pv in _productVariantRespository.Table
                        orderby pv.MinStockQuantity
                        where !pv.Deleted &&
                        pv.MinStockQuantity >= pv.StockQuantity
                        select pv;
            var productVariants = query.ToList();
            return productVariants;
        }
        
        /// <summary>
        /// Gets a product variant
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>Product variant</returns>
        public ProductVariant GetProductVariantById(int productVariantId)
        {
            if (productVariantId == 0)
                return null;

            string key = string.Format(PRODUCTVARIANTS_BY_ID_KEY, productVariantId);
            object obj2 = _cacheManager.Get(key);
            if (obj2 != null)
            {
                return (ProductVariant) obj2;
            }

            var productVariant = _productVariantRespository.GetById(productVariantId);

            _cacheManager.Add(key, productVariant);

            return productVariant;
        }

        /// <summary>
        /// Gets a product variant by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Product variant</returns>
        public ProductVariant GetProductVariantBySku(string sku)
        {
            if (String.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            var query = from pv in _productVariantRespository.Table
                        orderby pv.DisplayOrder, pv.Id
                        where !pv.Deleted &&
                        pv.Sku == sku
                        select pv;
            var productVariant = query.FirstOrDefault();
            return productVariant;
        }
        
        /// <summary>
        /// Inserts a product variant
        /// </summary>
        /// <param name="productVariant">The product variant</param>
        public void InsertProductVariant(ProductVariant productVariant)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            _productVariantRespository.Insert(productVariant);
            
            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the product variant
        /// </summary>
        /// <param name="productVariant">The product variant</param>
        public void UpdateProductVariant(ProductVariant productVariant)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            _productVariantRespository.Update(productVariant);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
        }

        /// <summary>
        /// Gets product variants by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>Product variant collection</returns>
        public List<ProductVariant> GetProductVariantsByProductId(int productId)
        {
            //TODO: use bool showHidden = NopContext.Current.IsAdmin;
            bool showHidden = true;
            return GetProductVariantsByProductId(productId, showHidden);
        }
        
        /// <summary>
        /// Gets product variants by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product variant collection</returns>
        public List<ProductVariant> GetProductVariantsByProductId(int productId, bool showHidden)
        {
            string key = string.Format(PRODUCTVARIANTS_ALL_KEY, showHidden, productId);
            object obj2 = _cacheManager.Get(key);
            if (obj2 != null)
            {
                return (List<ProductVariant>) obj2;
            }

            var query = (IQueryable<ProductVariant>) _productVariantRespository.Table;
            if (!showHidden)
            {
                query = query.Where(pv => pv.Published);
            }
            if (!showHidden)
            {
                query =
                    query.Where(
                        pv => !pv.AvailableStartDateTimeUtc.HasValue || pv.AvailableStartDateTimeUtc <= DateTime.UtcNow);
                query =
                    query.Where(
                        pv => !pv.AvailableEndDateTimeUtc.HasValue || pv.AvailableEndDateTimeUtc >= DateTime.UtcNow);
            }
            query = query.Where(pv => !pv.Deleted);
            query = query.Where(pv => pv.ProductId == productId);
            query = query.OrderBy(pv => pv.DisplayOrder);

            var productVariants = query.ToList();

            //cache
            _cacheManager.Add(key, productVariants);

            return productVariants;
        }

        /// <summary>
        /// Gets localized product variant by id
        /// </summary>
        /// <param name="localizedProductVariantId">Localized product variant identifier</param>
        /// <returns>Product variant content</returns>
        public LocalizedProductVariant GetLocalizedProductVariantById(int localizedProductVariantId)
        {
            if (localizedProductVariantId == 0)
                return null;

            var productVariantLocalized = _localizedProductVariantRespository.GetById(localizedProductVariantId);
            return productVariantLocalized;
        }

        /// <summary>
        /// Gets localized product variant by product variant id
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>Product variant content</returns>
        public List<LocalizedProductVariant> GetLocalizedProductVariantByProductVariantId(int productVariantId)
        {
            if (productVariantId == 0)
                return new List<LocalizedProductVariant>();
            
            var query = from pvl in _localizedProductVariantRespository.Table
                        where pvl.ProductVariantId == productVariantId
                        select pvl;
            var content = query.ToList();
            return content;
        }

        /// <summary>
        /// Gets localized product variant by product variant id and language id
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Product variant content</returns>
        public LocalizedProductVariant GetLocalizedProductVariantByProductVariantIdAndLanguageId(int productVariantId, int languageId)
        {
            if (productVariantId == 0 || languageId == 0)
                return null;
            
            var query = from pvl in _localizedProductVariantRespository.Table
                        orderby pvl.Id
                        where pvl.ProductVariantId == productVariantId &&
                        pvl.LanguageId == languageId
                        select pvl;
            var productVariantLocalized = query.FirstOrDefault();
            return productVariantLocalized;
        }

        /// <summary>
        /// Inserts a localized product variant
        /// </summary>
        /// <param name="localizedProductVariant">Localized product variant</param>
        public void InsertLocalizedProductVariant(LocalizedProductVariant localizedProductVariant)
        {
            if (localizedProductVariant == null)
                throw new ArgumentNullException("localizedProductVariant");

            _localizedProductVariantRespository.Insert(localizedProductVariant);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
        }

        /// <summary>
        /// Update a localized product variant
        /// </summary>
        /// <param name="localizedProductVariant">Localized product variant</param>
        public void UpdateLocalizedProductVariant(LocalizedProductVariant localizedProductVariant)
        {
            if (localizedProductVariant == null)
                throw new ArgumentNullException("localizedProductVariant");

            bool allFieldsAreEmpty = string.IsNullOrEmpty(localizedProductVariant.Name) &&
                                     string.IsNullOrEmpty(localizedProductVariant.Description);
            
            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                _localizedProductVariantRespository.Delete(localizedProductVariant);
            }
            else
            {
                _localizedProductVariantRespository.Update(localizedProductVariant);
            }

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
        }

        /// <summary>
        /// Delete a product variant
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        public void DeleteProductVariant(ProductVariant productVariant)
        {
            if (productVariant == null)
                return;

            productVariant.Deleted = true;
            UpdateProductVariant(productVariant);
        }

        #endregion

        #endregion
    }
}
