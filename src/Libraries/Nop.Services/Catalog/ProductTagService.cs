using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product tag service
    /// </summary>
    public partial class ProductTagService : IProductTagService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        private const string PRODUCTTAG_COUNT_KEY = "Nop.producttag.count-{0}";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTTAG_PATTERN_KEY = "Nop.producttag.";

        #endregion

        #region Fields

        private readonly IRepository<ProductTag> _productTagRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly CommonSettings _commonSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="productTagRepository">Product tag repository</param>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="commonSettings">Common settings</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="catalogSettings">Catalog settings</param>
        public ProductTagService(IRepository<ProductTag> productTagRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IDataProvider dataProvider,
            IDbContext dbContext,
            CommonSettings commonSettings,
            CatalogSettings catalogSettings,
            ICacheManager cacheManager,
            IEventPublisher eventPublisher)
        {
            this._productTagRepository = productTagRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._commonSettings = commonSettings;
            this._catalogSettings = catalogSettings;
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Nested classes

        private class ProductTagWithCount
        {
            public int ProductTagId { get; set; }
            public int ProductCount { get; set; }
        }

        #endregion
        
        #region Utilities

        /// <summary>
        /// Get product count for each of existing product tag
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Dictionary of "product tag ID : product count"</returns>
        private Dictionary<int, int> GetProductCount(int storeId)
        {
            string key = string.Format(PRODUCTTAG_COUNT_KEY, storeId);
            return _cacheManager.Get(key, () =>
            {

                if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
                {
                    //stored procedures are enabled and supported by the database. 
                    //It's much faster than the LINQ implementation below 

                    #region Use stored procedure

                    //prepare parameters
                    var pStoreId = _dataProvider.GetParameter();
                    pStoreId.ParameterName = "StoreId";
                    pStoreId.Value = storeId;
                    pStoreId.DbType = DbType.Int32;


                    //invoke stored procedure
                    var result = _dbContext.SqlQuery<ProductTagWithCount>(
                        "Exec ProductTagCountLoadAll @StoreId",
                        pStoreId);

                    var dictionary = new Dictionary<int, int>();
                    foreach (var item in result)
                        dictionary.Add(item.ProductTagId, item.ProductCount);
                    return dictionary;

                    #endregion
                }
                else
                {
                    //stored procedures aren't supported. Use LINQ
                    #region Search products
                    var query = _productTagRepository.Table.Select(pt => new
                    {
                        Id = pt.Id,
                        ProductCount = (storeId == 0 || _catalogSettings.IgnoreStoreLimitations) ?
                            pt.Products.Count(p => !p.Deleted && p.Published)
                            : (from p in pt.Products
                               join sm in _storeMappingRepository.Table
                               on new { p1 = p.Id, p2 = "Product" } equals new { p1 = sm.EntityId, p2 = sm.EntityName } into p_sm
                               from sm in p_sm.DefaultIfEmpty()
                               where (!p.LimitedToStores || storeId == sm.StoreId) && !p.Deleted && p.Published
                               select p).Count()
                    });
                    var dictionary = new Dictionary<int, int>();
                    foreach (var item in query)
                        dictionary.Add(item.Id, item.ProductCount);
                    return dictionary;

                    #endregion

                }
            });
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

            //cache
            _cacheManager.RemoveByPattern(PRODUCTTAG_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productTag);
        }

        /// <summary>
        /// Gets all product tags
        /// </summary>
        /// <returns>Product tags</returns>
        public virtual IList<ProductTag> GetAllProductTags()
        {
            var query = _productTagRepository.Table;
            var productTags = query.ToList();
            return productTags;
        }

        /// <summary>
        /// Gets product tag
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>Product tag</returns>
        public virtual ProductTag GetProductTagById(int productTagId)
        {
            if (productTagId == 0)
                return null;

            return _productTagRepository.GetById(productTagId);
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

            //cache
            _cacheManager.RemoveByPattern(PRODUCTTAG_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productTag);
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

            //cache
            _cacheManager.RemoveByPattern(PRODUCTTAG_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productTag);
        }

        /// <summary>
        /// Get number of products
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Number of products</returns>
        public virtual int GetProductCount(int productTagId, int storeId)
        {
            var dictionary = GetProductCount(storeId);
            if (dictionary.ContainsKey(productTagId))
                return dictionary[productTagId];
            
            return 0;
        }

        #endregion
    }
}
