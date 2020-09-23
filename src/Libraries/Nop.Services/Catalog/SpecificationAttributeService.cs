using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Specification attribute service
    /// </summary>
    public partial class SpecificationAttributeService : ISpecificationAttributeService
    {
        #region Fields

        private readonly ICacheKeyService _cacheKeyService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductSpecificationAttribute> _productSpecificationAttributeRepository;
        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;

        #endregion

        #region Ctor

        public SpecificationAttributeService(ICacheKeyService cacheKeyService,
            IEventPublisher eventPublisher,
            IRepository<Product> productRepository,
            IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository,
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository)
        {
            _cacheKeyService = cacheKeyService;
            _eventPublisher = eventPublisher;
            _productRepository = productRepository;
            _productSpecificationAttributeRepository = productSpecificationAttributeRepository;
            _specificationAttributeRepository = specificationAttributeRepository;
            _specificationAttributeOptionRepository = specificationAttributeOptionRepository;
        }

        #endregion

        #region Methods

        #region Specification attribute

        /// <summary>
        /// Gets a specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute</returns>
        public virtual async Task<SpecificationAttribute> GetSpecificationAttributeById(int specificationAttributeId)
        {
            if (specificationAttributeId == 0)
                return null;

            return await _specificationAttributeRepository.ToCachedGetById(specificationAttributeId);
        }

        /// <summary>
        /// Gets specification attributes
        /// </summary>
        /// <param name="specificationAttributeIds">The specification attribute identifiers</param>
        /// <returns>Specification attributes</returns>
        public virtual async Task<IList<SpecificationAttribute>> GetSpecificationAttributeByIds(int[] specificationAttributeIds)
        {
            if (specificationAttributeIds == null || specificationAttributeIds.Length == 0)
                return new List<SpecificationAttribute>();

            var query = from p in _specificationAttributeRepository.Table
                        where specificationAttributeIds.Contains(p.Id)
                        select p;

            return await query.ToListAsync();
        }

        /// <summary>
        /// Gets specification attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Specification attributes</returns>
        public virtual async Task<IPagedList<SpecificationAttribute>> GetSpecificationAttributes(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from sa in _specificationAttributeRepository.Table
                        orderby sa.DisplayOrder, sa.Id
                        select sa;

            return await query.ToPagedList(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets specification attributes that have options
        /// </summary>
        /// <returns>Specification attributes that have available options</returns>
        public virtual async Task<IList<SpecificationAttribute>> GetSpecificationAttributesWithOptions()
        {
            var query = from sa in _specificationAttributeRepository.Table
                        where _specificationAttributeOptionRepository.Table.Any(o => o.SpecificationAttributeId == sa.Id)
                        orderby sa.DisplayOrder, sa.Id
                        select sa;

            return await query.ToCachedList(_cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.SpecAttributesWithOptionsCacheKey));
        }

        /// <summary>
        /// Deletes a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual async Task DeleteSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException(nameof(specificationAttribute));

            await _specificationAttributeRepository.Delete(specificationAttribute);
            
            //event notification
            await _eventPublisher.EntityDeleted(specificationAttribute);
        }

        /// <summary>
        /// Deletes specifications attributes
        /// </summary>
        /// <param name="specificationAttributes">Specification attributes</param>
        public virtual async Task DeleteSpecificationAttributes(IList<SpecificationAttribute> specificationAttributes)
        {
            if (specificationAttributes == null)
                throw new ArgumentNullException(nameof(specificationAttributes));

            foreach (var specificationAttribute in specificationAttributes) 
                await DeleteSpecificationAttribute(specificationAttribute);
        }

        /// <summary>
        /// Inserts a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual async Task InsertSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException(nameof(specificationAttribute));

            await _specificationAttributeRepository.Insert(specificationAttribute);
            
            //event notification
            await _eventPublisher.EntityInserted(specificationAttribute);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual async Task UpdateSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException(nameof(specificationAttribute));

            await _specificationAttributeRepository.Update(specificationAttribute);
            
            //event notification
            await _eventPublisher.EntityUpdated(specificationAttribute);
        }

        #endregion

        #region Specification attribute option

        /// <summary>
        /// Gets a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier</param>
        /// <returns>Specification attribute option</returns>
        public virtual async Task<SpecificationAttributeOption> GetSpecificationAttributeOptionById(int specificationAttributeOptionId)
        {
            if (specificationAttributeOptionId == 0)
                return null;

            return await _specificationAttributeOptionRepository.ToCachedGetById(specificationAttributeOptionId);
        }

        /// <summary>
        /// Get specification attribute options by identifiers
        /// </summary>
        /// <param name="specificationAttributeOptionIds">Identifiers</param>
        /// <returns>Specification attribute options</returns>
        public virtual async Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByIds(int[] specificationAttributeOptionIds)
        {
            if (specificationAttributeOptionIds == null || specificationAttributeOptionIds.Length == 0)
                return new List<SpecificationAttributeOption>();

            var query = from sao in _specificationAttributeOptionRepository.Table
                        where specificationAttributeOptionIds.Contains(sao.Id)
                        select sao;
            var specificationAttributeOptions = await query.ToListAsync();
            //sort by passed identifiers
            var sortedSpecificationAttributeOptions = new List<SpecificationAttributeOption>();
            foreach (var id in specificationAttributeOptionIds)
            {
                var sao = specificationAttributeOptions.Find(x => x.Id == id);
                if (sao != null)
                    sortedSpecificationAttributeOptions.Add(sao);
            }

            return sortedSpecificationAttributeOptions;
        }

        /// <summary>
        /// Gets a specification attribute option by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute option</returns>
        public virtual async Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsBySpecificationAttribute(int specificationAttributeId)
        {
            var query = from sao in _specificationAttributeOptionRepository.Table
                        orderby sao.DisplayOrder, sao.Id
                        where sao.SpecificationAttributeId == specificationAttributeId
                        select sao;

            var specificationAttributeOptions = await query.ToCachedList(_cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.SpecAttributesOptionsCacheKey, specificationAttributeId));

            return specificationAttributeOptions;
        }

        /// <summary>
        /// Deletes a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual async Task DeleteSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException(nameof(specificationAttributeOption));

            await _specificationAttributeOptionRepository.Delete(specificationAttributeOption);

            //event notification
            await _eventPublisher.EntityDeleted(specificationAttributeOption);
        }

        /// <summary>
        /// Inserts a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual async Task InsertSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException(nameof(specificationAttributeOption));

            await _specificationAttributeOptionRepository.Insert(specificationAttributeOption);
            
            //event notification
            await _eventPublisher.EntityInserted(specificationAttributeOption);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual async Task UpdateSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException(nameof(specificationAttributeOption));

            await _specificationAttributeOptionRepository.Update(specificationAttributeOption);
            
            //event notification
            await _eventPublisher.EntityUpdated(specificationAttributeOption);
        }

        /// <summary>
        /// Returns a list of IDs of not existing specification attribute options
        /// </summary>
        /// <param name="attributeOptionIds">The IDs of the attribute options to check</param>
        /// <returns>List of IDs not existing specification attribute options</returns>
        public virtual async Task<int[]> GetNotExistingSpecificationAttributeOptions(int[] attributeOptionIds)
        {
            if (attributeOptionIds == null)
                throw new ArgumentNullException(nameof(attributeOptionIds));

            var query = _specificationAttributeOptionRepository.Table;
            var queryFilter = attributeOptionIds.Distinct().ToArray();
            var filter = await query.Select(a => a.Id).Where(m => queryFilter.Contains(m)).ToListAsync();
            return queryFilter.Except(filter).ToArray();
        }

        #endregion

        #region Product specification attribute

        /// <summary>
        /// Deletes a product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute</param>
        public virtual async Task DeleteProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException(nameof(productSpecificationAttribute));

            await _productSpecificationAttributeRepository.Delete(productSpecificationAttribute);
            
            //event notification
            await _eventPublisher.EntityDeleted(productSpecificationAttribute);
        }

        /// <summary>
        /// Gets a product specification attribute mapping collection
        /// </summary>
        /// <param name="productId">Product identifier; 0 to load all records</param>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier; 0 to load all records</param>
        /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 1 to load attributes with AllowFiltering set to true, null to load all attributes</param>
        /// <param name="showOnProductPage">0 to load attributes with ShowOnProductPage set to false, 1 to load attributes with ShowOnProductPage set to true, null to load all attributes</param>
        /// <returns>Product specification attribute mapping collection</returns>
        public virtual async Task<IList<ProductSpecificationAttribute>> GetProductSpecificationAttributes(int productId = 0,
            int specificationAttributeOptionId = 0, bool? allowFiltering = null, bool? showOnProductPage = null)
        {
            var allowFilteringCacheStr = allowFiltering.HasValue ? allowFiltering.ToString() : "null";
            var showOnProductPageCacheStr = showOnProductPage.HasValue ? showOnProductPage.ToString() : "null";

            var key = _cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductSpecificationAttributeAllByProductIdCacheKey, 
                productId, specificationAttributeOptionId, allowFilteringCacheStr, showOnProductPageCacheStr);

            var query = _productSpecificationAttributeRepository.Table;
            if (productId > 0)
                query = query.Where(psa => psa.ProductId == productId);
            if (specificationAttributeOptionId > 0)
                query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);
            if (allowFiltering.HasValue)
                query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
            if (showOnProductPage.HasValue)
                query = query.Where(psa => psa.ShowOnProductPage == showOnProductPage.Value);
            query = query.OrderBy(psa => psa.DisplayOrder).ThenBy(psa => psa.Id);

            var productSpecificationAttributes = await query.ToCachedList(key);

            return productSpecificationAttributes;
        }

        /// <summary>
        /// Gets a product specification attribute mapping 
        /// </summary>
        /// <param name="productSpecificationAttributeId">Product specification attribute mapping identifier</param>
        /// <returns>Product specification attribute mapping</returns>
        public virtual async Task<ProductSpecificationAttribute> GetProductSpecificationAttributeById(int productSpecificationAttributeId)
        {
            if (productSpecificationAttributeId == 0)
                return null;

            return await _productSpecificationAttributeRepository.GetById(productSpecificationAttributeId);
        }

        /// <summary>
        /// Inserts a product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
        public virtual async Task InsertProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException(nameof(productSpecificationAttribute));

            await _productSpecificationAttributeRepository.Insert(productSpecificationAttribute);
            
            //event notification
            await _eventPublisher.EntityInserted(productSpecificationAttribute);
        }

        /// <summary>
        /// Updates the product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
        public virtual async Task UpdateProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException(nameof(productSpecificationAttribute));

            await _productSpecificationAttributeRepository.Update(productSpecificationAttribute);
            
            //event notification
            await _eventPublisher.EntityUpdated(productSpecificationAttribute);
        }

        /// <summary>
        /// Gets a count of product specification attribute mapping records
        /// </summary>
        /// <param name="productId">Product identifier; 0 to load all records</param>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier; 0 to load all records</param>
        /// <returns>Count</returns>
        public virtual async Task<int> GetProductSpecificationAttributeCount(int productId = 0, int specificationAttributeOptionId = 0)
        {
            var query = _productSpecificationAttributeRepository.Table;
            if (productId > 0)
                query = query.Where(psa => psa.ProductId == productId);
            if (specificationAttributeOptionId > 0)
                query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);

            return await query.CountAsync();
        }

        /// <summary>
        /// Get mapped products for specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Products</returns>
        public virtual async Task<IPagedList<Product>> GetProductsBySpecificationAttributeId(int specificationAttributeId, int pageIndex, int pageSize)
        {
            var query = from product in _productRepository.Table
                join psa in _productSpecificationAttributeRepository.Table on product.Id equals psa.ProductId
                join spao in _specificationAttributeOptionRepository.Table on psa.SpecificationAttributeOptionId equals spao.Id 
                where spao.SpecificationAttributeId == specificationAttributeId
                orderby product.Name
                select product;

            return await query.ToPagedList(pageIndex, pageSize);
        }

        #endregion

        #endregion
    }
}
