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
        private const string TIERPRICES_ALLBYPRODUCTVARIANTID_KEY = "Nop.tierprice.allbyproductvariantid-{0}";
        private const string PRODUCTS_PATTERN_KEY = "Nop.product.";
        private const string PRODUCTVARIANTS_PATTERN_KEY = "Nop.productvariant.";
        private const string TIERPRICES_PATTERN_KEY = "Nop.tierprice.";
        #endregion

        #region Fields

        private readonly IWorkingContext _context;
        private readonly ILocalizedEntityService _leService;
        private readonly IRepository<Product> _productRespository;
        private readonly IRepository<ProductCategory> _productCategoryRespository;
        private readonly IRepository<ProductManufacturer> _productManufacturerRespository;
        private readonly IRepository<ProductVariant> _productVariantRespository;
        private readonly IRepository<RelatedProduct> _relatedProductRespository;
        private readonly IRepository<CrossSellProduct> _crossSellProductRespository;
        private readonly IRepository<TierPrice> _tierPriceRespository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Working context</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="leService">Localized entity service</param>
        /// <param name="productRespository">Product repository</param>
        /// <param name="productCategoryRespository">Product category repository</param>
        /// <param name="productManufacturerRespository">Product manufacturer repository</param>
        /// <param name="productVariantRespository">Product variant repository</param>
        /// <param name="relatedProductRespository">Related product repository</param>
        /// <param name="crossSellProductRespository">Cross-sell product repository</param>
        /// <param name="tierPriceRespository">Tier price repository</param>
        public ProductService(IWorkingContext context, 
            ICacheManager cacheManager,
            ILocalizedEntityService leService,
            IRepository<Product> productRespository,
            IRepository<ProductCategory> productCategoryRespository,
            IRepository<ProductManufacturer> productManufacturerRespository,
            IRepository<ProductVariant> productVariantRespository,
            IRepository<RelatedProduct> relatedProductRespository,
            IRepository<CrossSellProduct> crossSellProductRespository,
            IRepository<TierPrice> tierPriceRespository)
        {
            this._context = context;
            this._cacheManager = cacheManager;
            this._leService = leService;
            this._productRespository = productRespository;
            this._productCategoryRespository = productCategoryRespository;
            this._productManufacturerRespository = productManufacturerRespository;
            this._productVariantRespository = productVariantRespository;
            this._relatedProductRespository = relatedProductRespository;
            this._crossSellProductRespository = crossSellProductRespository;
            this._tierPriceRespository = tierPriceRespository;
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
            bool showHidden = _context.IsAdmin;
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
            bool showHidden = _context.IsAdmin;

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
            return _cacheManager.Get(key, () =>
            {
                var product = _productRespository.GetById(productId);
                return product;
            });
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
            _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
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
            _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
        }
         
        /// <summary>
        /// Search products
        /// </summary>
        /// <param name="categoryId">Category identifier; 0 to load all recordss</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all records</param>
        /// <param name="featuredProducts">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers). 0 to load featured products only, 1 to load not featured products only, null to load all products</param>
        /// <param name="priceMin">Minimum price; null to load all records</param>
        /// <param name="priceMax">Maximum price; null to load all records</param>
        /// <param name="relatedToProductId">Filter by related product; 0 to load all records</param>
        /// <param name="productTagId">Product tag identifier; 0 to load all records</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search in descriptions</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="filteredSpecs">Filtered product specification identifiers</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Product collection</returns>
        public PagedList<Product> SearchProducts(int categoryId, int manufacturerId, bool? featuredProducts,
            decimal? priceMin, decimal? priceMax,
            int relatedToProductId, int productTagId,
            string keywords, bool searchDescriptions, int languageId,
            List<int> filteredSpecs, ProductSortingEnum orderBy,
            int pageIndex, int pageSize)
        {
            bool showHidden = _context.IsAdmin;

            //UNDONE: filter by product specs
            //string commaSeparatedSpecIds = string.Empty;
            //if (filteredSpecs != null)
            //{
            //    filteredSpecs.Sort();
            //    for (int i = 0; i < filteredSpecs.Count; i++)
            //    {
            //        commaSeparatedSpecIds += filteredSpecs[i].ToString();
            //        if (i != filteredSpecs.Count - 1)
            //        {
            //            commaSeparatedSpecIds += ",";
            //        }
            //    }
            //}

            bool searchKeywords = String.IsNullOrWhiteSpace(keywords);

            var query1 = from p in _productRespository.Table
                         from pcm in _productCategoryRespository.Table
                         .Where(pcm => p.Id == pcm.ProductId).DefaultIfEmpty()
                         from pmm in _productManufacturerRespository.Table
                         .Where(pmm => p.Id == pmm.ProductId).DefaultIfEmpty()
                         from rp in _relatedProductRespository.Table
                         .Where(rp => p.Id == rp.ProductId2).DefaultIfEmpty()
                         from pv in _productVariantRespository.Table
                         .Where(pv => p.Id == pv.ProductId).DefaultIfEmpty()
                         //UNDONE: search in localized properties
                         //from pvl in _context.ProductVariantLocalized
                         //.Where(pvl => pv.ProductVariantId == pvl.ProductVariantId && pvl.LanguageId == languageId).DefaultIfEmpty()
                         //from pl in _context.ProductLocalized
                         //.Where(pl => p.ProductId == pl.ProductId && pl.LanguageId == languageId).DefaultIfEmpty()
                         //UNDONE search product tags
                         where
                         (categoryId == 0 || (pcm.CategoryId == categoryId && (!featuredProducts.HasValue || pcm.IsFeaturedProduct == featuredProducts.Value))) &&
                         (manufacturerId == 0 || (pmm.ManufacturerId == manufacturerId && (!featuredProducts.HasValue || pmm.IsFeaturedProduct == featuredProducts.Value))) &&
                         (relatedToProductId == 0 || rp.ProductId1 == relatedToProductId) &&
                         (showHidden || p.Published) &&
                         (!p.Deleted) &&
                         (showHidden || pv.Published) &&
                         (showHidden || !pv.Deleted) &&
                         (!priceMin.HasValue || priceMin.Value == 0 || pv.Price > priceMin.Value) &&
                         (!priceMax.HasValue || priceMax.Value == int.MaxValue || pv.Price < priceMax.Value) &&
                         (!searchKeywords ||
                         //search standard content
                         (p.Name.Contains(keywords)
                         || pv.Name.Contains(keywords)
                         || pv.Sku.Contains(keywords)
                         || (searchDescriptions && p.ShortDescription.Contains(keywords))
                         || (searchDescriptions && p.FullDescription.Contains(keywords))
                         || (searchDescriptions && pv.Description.Contains(keywords))
                         //search language content
                         //|| pl.Name.Contains(keywords)
                         //|| pvl.Name.Contains(keywords)
                         //|| (searchDescriptions && pl.ShortDescription.Contains(keywords))
                         //|| (searchDescriptions && pl.FullDescription.Contains(keywords))
                         //|| (searchDescriptions && pvl.Description.Contains(keywords))
                         )
                         )
                         select p.Id;
            //UNDONE sort by ProductSortingEnum orderBy
            var query = from p in _productRespository.Table
                        where query1.Contains(p.Id)
                        orderby p.CreatedOnUtc descending
                        select p;

            var products = new PagedList<Product>(query, pageIndex, pageSize);
            return products;
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
            return _cacheManager.Get(key, () =>
            {
                var pv = _productVariantRespository.GetById(productVariantId);
                return pv;
            });
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
            _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
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
            _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets product variants by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>Product variant collection</returns>
        public List<ProductVariant> GetProductVariantsByProductId(int productId)
        {
            bool showHidden = _context.IsAdmin;
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
            return _cacheManager.Get(key, () =>
                                              {
                                                  var query = (IQueryable<ProductVariant>) _productVariantRespository.Table;
                                                  if (!showHidden)
                                                  {
                                                      query = query.Where(pv => pv.Published);
                                                  }
                                                  if (!showHidden)
                                                  {
                                                      query =
                                                          query.Where(
                                                              pv =>
                                                              !pv.AvailableStartDateTimeUtc.HasValue ||
                                                              pv.AvailableStartDateTimeUtc <= DateTime.UtcNow);
                                                      query =
                                                          query.Where(
                                                              pv =>
                                                              !pv.AvailableEndDateTimeUtc.HasValue ||
                                                              pv.AvailableEndDateTimeUtc >= DateTime.UtcNow);
                                                  }
                                                  query = query.Where(pv => !pv.Deleted);
                                                  query = query.Where(pv => pv.ProductId == productId);
                                                  query = query.OrderBy(pv => pv.DisplayOrder);

                                                  var productVariants = query.ToList();
                                                  return productVariants;
                                              });
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

        #region Related products

        /// <summary>
        /// Deletes a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        public void DeleteRelatedProduct(RelatedProduct relatedProduct)
        {
            if (relatedProduct == null)
                return;

            _relatedProductRespository.Delete(relatedProduct);
        }

        /// <summary>
        /// Gets a related product collection by product identifier
        /// </summary>
        /// <param name="productId1">The first product identifier</param>
        /// <returns>Related product collection</returns>
        public List<RelatedProduct> GetRelatedProductsByProductId1(int productId1)
        {
            bool showHidden = _context.IsAdmin;

            var query = from rp in _relatedProductRespository.Table
                        join p in _productRespository.Table on rp.ProductId2 equals p.Id
                        where rp.ProductId1 == productId1 &&
                        !p.Deleted &&
                        (showHidden || p.Published)
                        orderby rp.DisplayOrder
                        select rp;
            var relatedProducts = query.ToList();

            return relatedProducts;
        }

        /// <summary>
        /// Gets a related product
        /// </summary>
        /// <param name="relatedProductId">Related product identifer</param>
        /// <returns>Related product</returns>
        public RelatedProduct GetRelatedProductById(int relatedProductId)
        {
            if (relatedProductId == 0)
                return null;
            
            var relatedProduct = _relatedProductRespository.GetById(relatedProductId);
            return relatedProduct;
        }

        /// <summary>
        /// Inserts a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        public void InsertRelatedProduct(RelatedProduct relatedProduct)
        {
            if (relatedProduct == null)
                throw new ArgumentNullException("relatedProduct");

            _relatedProductRespository.Insert(relatedProduct);
        }

        /// <summary>
        /// Updates a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        public void UpdateRelatedProduct(RelatedProduct relatedProduct)
        {
            if (relatedProduct == null)
                throw new ArgumentNullException("relatedProduct");

            _relatedProductRespository.Update(relatedProduct);
        }

        #endregion

        #region Cross-sell products

        /// <summary>
        /// Deletes a cross-sell product
        /// </summary>
        /// <param name="crossSellProduct">Cross-sell intifer</param>
        public void DeleteCrossSellProduct(CrossSellProduct crossSellProduct)
        {
            if (crossSellProduct == null)
                return;

            _crossSellProductRespository.Delete(crossSellProduct);
        }

        /// <summary>
        /// Gets a cross-sell product collection by product identifier
        /// </summary>
        /// <param name="productId1">The first product identifier</param>
        /// <returns>Cross-sell product collection</returns>
        public List<CrossSellProduct> GetCrossSellProductsByProductId1(int productId1)
        {
            bool showHidden = _context.IsAdmin;

            var query = from csp in _crossSellProductRespository.Table
                        join p in _productRespository.Table on csp.ProductId2 equals p.Id
                        where csp.ProductId1 == productId1 &&
                        !p.Deleted &&
                        (showHidden || p.Published)
                        orderby csp.Id
                        select csp;
            var crossSellProducts = query.ToList();
            return crossSellProducts;
        }

        /// <summary>
        /// Gets a cross-sell product
        /// </summary>
        /// <param name="crossSellProductId">Cross-sell product identifer</param>
        /// <returns>Cross-sell product</returns>
        public CrossSellProduct GetCrossSellProductById(int crossSellProductId)
        {
            if (crossSellProductId == 0)
                return null;

            var crossSellProduct = _crossSellProductRespository.GetById(crossSellProductId);
            return crossSellProduct;
        }

        /// <summary>
        /// Inserts a cross-sell product
        /// </summary>
        /// <param name="crossSellProduct">Cross-sell product</param>
        public void InsertCrossSellProduct(CrossSellProduct crossSellProduct)
        {
            if (crossSellProduct == null)
                throw new ArgumentNullException("crossSellProduct");

            _crossSellProductRespository.Insert(crossSellProduct);
        }

        /// <summary>
        /// Updates a cross-sell product
        /// </summary>
        /// <param name="crossSellProduct">Cross-sell product</param>
        public void UpdateCrossSellProduct(CrossSellProduct crossSellProduct)
        {
            if (crossSellProduct == null)
                throw new ArgumentNullException("crossSellProduct");

            _crossSellProductRespository.Update(crossSellProduct);
        }

        #endregion
        
        #region Tier prices
        
        /// <summary>
        /// Deletes a tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        public void DeleteTierPrice(TierPrice tierPrice)
        {
            if (tierPrice == null)
                return;

            _tierPriceRespository.Delete(tierPrice);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets a tier price
        /// </summary>
        /// <param name="tierPriceId">Tier price identifier</param>
        /// <returns>Tier price</returns>
        public TierPrice GetTierPriceById(int tierPriceId)
        {
            if (tierPriceId == 0)
                return null;
            
            var tierPrice = _tierPriceRespository.GetById(tierPriceId);
            return tierPrice;
        }

        /// <summary>
        /// Gets tier prices by product variant identifier
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>Tier price collection</returns>
        public List<TierPrice> GetTierPricesByProductVariantId(int productVariantId)
        {
            if (productVariantId == 0)
                return new List<TierPrice>();

            string key = string.Format(TIERPRICES_ALLBYPRODUCTVARIANTID_KEY, productVariantId);
            return _cacheManager.Get(key, () =>
            {
                var query = from tp in _tierPriceRespository.Table
                            orderby tp.Quantity
                            where tp.ProductVariantId == productVariantId
                            select tp;
                var tierPrices = query.ToList();
                return tierPrices;
            });
        }

        /// <summary>
        /// Inserts a tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        public void InsertTierPrice(TierPrice tierPrice)
        {
            if (tierPrice == null)
                throw new ArgumentNullException("tierPrice");

            _tierPriceRespository.Insert(tierPrice);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        public void UpdateTierPrice(TierPrice tierPrice)
        {
            if (tierPrice == null)
                throw new ArgumentNullException("tierPrice");

            _tierPriceRespository.Update(tierPrice);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
        }

        #endregion

        #endregion
    }
}
