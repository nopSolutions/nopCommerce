using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Specification attribute service
    /// </summary>
    public partial class SpecificationAttributeService : ISpecificationAttributeService
    {
        #region Fields

        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductSpecificationAttribute> _productSpecificationAttributeRepository;
        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;
        private readonly IRepository<SpecificationAttributeGroup> _specificationAttributeGroupRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public SpecificationAttributeService(IRepository<Product> productRepository,
            IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository,
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository,
            IRepository<SpecificationAttributeGroup> specificationAttributeGroupRepository,
            IStaticCacheManager staticCacheManager)
        {
            _productRepository = productRepository;
            _productSpecificationAttributeRepository = productSpecificationAttributeRepository;
            _specificationAttributeRepository = specificationAttributeRepository;
            _specificationAttributeOptionRepository = specificationAttributeOptionRepository;
            _specificationAttributeGroupRepository = specificationAttributeGroupRepository;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        #region Specification attribute group

        /// <summary>
        /// Gets a specification attribute group
        /// </summary>
        /// <param name="specificationAttributeGroupId">The specification attribute group identifier</param>
        /// <returns>Specification attribute group</returns>
        public virtual async Task<SpecificationAttributeGroup> GetSpecificationAttributeGroupById(int specificationAttributeGroupId)
        {
            return await _specificationAttributeGroupRepository.GetById(specificationAttributeGroupId, cache => default);
        }

        /// <summary>
        /// Gets specification attribute groups
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Specification attribute groups</returns>
        public virtual async Task<IPagedList<SpecificationAttributeGroup>> GetSpecificationAttributeGroups(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from sag in _specificationAttributeGroupRepository.Table
                        orderby sag.DisplayOrder, sag.Id
                        select sag;

            return await query.ToPagedList(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets product specification attribute groups
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Specification attribute groups</returns>
        public virtual async Task<IList<SpecificationAttributeGroup>> GetProductSpecificationAttributeGroups(int productId)
        {
            var productAttributesForGroupQuery =
                from sa in _specificationAttributeRepository.Table
                join sao in _specificationAttributeOptionRepository.Table
                    on sa.Id equals sao.SpecificationAttributeId
                join psa in _productSpecificationAttributeRepository.Table
                    on sao.Id equals psa.SpecificationAttributeOptionId
                where psa.ProductId == productId && psa.ShowOnProductPage
                select sa.SpecificationAttributeGroupId;

            var availableGroupsQuery =
                from sag in _specificationAttributeGroupRepository.Table
                where productAttributesForGroupQuery.Any(groupId => groupId == sag.Id)
                select sag;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.SpecificationAttributeGroupByProductCacheKey, productId);

            return await _staticCacheManager.Get(key, async () => await availableGroupsQuery.ToAsyncEnumerable().ToListAsync());
        }

        /// <summary>
        /// Deletes a specification attribute group
        /// </summary>
        /// <param name="specificationAttributeGroup">The specification attribute group</param>
        public virtual async Task DeleteSpecificationAttributeGroup(SpecificationAttributeGroup specificationAttributeGroup)
        {
            await _specificationAttributeGroupRepository.Delete(specificationAttributeGroup);
        }

        /// <summary>
        /// Inserts a specification attribute group
        /// </summary>
        /// <param name="specificationAttributeGroup">The specification attribute group</param>
        public virtual async Task InsertSpecificationAttributeGroup(SpecificationAttributeGroup specificationAttributeGroup)
        {
            await _specificationAttributeGroupRepository.Insert(specificationAttributeGroup);
        }

        /// <summary>
        /// Updates the specification attribute group
        /// </summary>
        /// <param name="specificationAttributeGroup">The specification attribute group</param>
        public virtual async Task UpdateSpecificationAttributeGroup(SpecificationAttributeGroup specificationAttributeGroup)
        {
            await _specificationAttributeGroupRepository.Update(specificationAttributeGroup);
        }

        #endregion

        #region Specification attribute

        /// <summary>
        /// Gets a specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute</returns>
        public virtual async Task<SpecificationAttribute> GetSpecificationAttributeById(int specificationAttributeId)
        {
            return await _specificationAttributeRepository.GetById(specificationAttributeId, cache => default);
        }

        /// <summary>
        /// Gets specification attributes
        /// </summary>
        /// <param name="specificationAttributeIds">The specification attribute identifiers</param>
        /// <returns>Specification attributes</returns>
        public virtual async Task<IList<SpecificationAttribute>> GetSpecificationAttributeByIds(int[] specificationAttributeIds)
        {
            return await _specificationAttributeRepository.GetByIds(specificationAttributeIds);
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

            return await _staticCacheManager.Get(_staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.SpecificationAttributesWithOptionsCacheKey), async () => await query.ToAsyncEnumerable().ToListAsync());
        }

        /// <summary>
        /// Gets specification attributes by group identifier
        /// </summary>
        /// <param name="specificationAttributeGroupId">The specification attribute group identifier</param>
        /// <returns>Specification attributes</returns>
        public virtual async Task<IList<SpecificationAttribute>> GetSpecificationAttributesByGroupId(int? specificationAttributeGroupId = null)
        {
            var query = _specificationAttributeRepository.Table;
            if (!specificationAttributeGroupId.HasValue || specificationAttributeGroupId > 0)
                query = query.Where(sa => sa.SpecificationAttributeGroupId == specificationAttributeGroupId);

            query = query.OrderBy(sa => sa.DisplayOrder).ThenBy(sa => sa.Id);

            return await query.ToAsyncEnumerable().ToListAsync();
        }

        /// <summary>
        /// Deletes a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual async Task DeleteSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            await _specificationAttributeRepository.Delete(specificationAttribute);
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
            await _specificationAttributeRepository.Insert(specificationAttribute);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual async Task UpdateSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            await _specificationAttributeRepository.Update(specificationAttribute);
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
            return await _specificationAttributeOptionRepository.GetById(specificationAttributeOptionId, cache => default);
        }

        /// <summary>
        /// Get specification attribute options by identifiers
        /// </summary>
        /// <param name="specificationAttributeOptionIds">Identifiers</param>
        /// <returns>Specification attribute options</returns>
        public virtual async Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByIds(int[] specificationAttributeOptionIds)
        {
            return await _specificationAttributeOptionRepository.GetByIds(specificationAttributeOptionIds);
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

            var specificationAttributeOptions = await _staticCacheManager.Get(_staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.SpecificationAttributeOptionsCacheKey, specificationAttributeId), async () => await query.ToAsyncEnumerable().ToListAsync());

            return specificationAttributeOptions;
        }

        /// <summary>
        /// Deletes a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual async Task DeleteSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            await _specificationAttributeOptionRepository.Delete(specificationAttributeOption);
        }

        /// <summary>
        /// Inserts a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual async Task InsertSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            await _specificationAttributeOptionRepository.Insert(specificationAttributeOption);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual async Task UpdateSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            await _specificationAttributeOptionRepository.Update(specificationAttributeOption);
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
            var filter = await query.Select(a => a.Id)
                .Where(m => queryFilter.Contains(m))
                .ToAsyncEnumerable()
                .ToListAsync();
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
            await _productSpecificationAttributeRepository.Delete(productSpecificationAttribute);
        }

        /// <summary>
        /// Gets a product specification attribute mapping collection
        /// </summary>
        /// <param name="productId">Product identifier; 0 to load all records</param>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier; 0 to load all records</param>
        /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 1 to load attributes with AllowFiltering set to true, null to load all attributes</param>
        /// <param name="showOnProductPage">0 to load attributes with ShowOnProductPage set to false, 1 to load attributes with ShowOnProductPage set to true, null to load all attributes</param>
        /// <param name="specificationAttributeGroupId">Specification attribute group identifier; 0 to load all records; null to load attributes without group</param>
        /// <returns>Product specification attribute mapping collection</returns>
        public virtual async Task<IList<ProductSpecificationAttribute>> GetProductSpecificationAttributes(int productId = 0,
            int specificationAttributeOptionId = 0, bool? allowFiltering = null, bool? showOnProductPage = null, int? specificationAttributeGroupId = 0)
        {
            var allowFilteringCacheStr = allowFiltering.HasValue ? allowFiltering.ToString() : "null";
            var showOnProductPageCacheStr = showOnProductPage.HasValue ? showOnProductPage.ToString() : "null";
            var specificationAttributeGroupIdCacheStr = specificationAttributeGroupId.HasValue ? specificationAttributeGroupId.ToString() : "null";

            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductSpecificationAttributeByProductCacheKey,
                productId, specificationAttributeOptionId, allowFilteringCacheStr, showOnProductPageCacheStr, specificationAttributeGroupIdCacheStr);

            var query = _productSpecificationAttributeRepository.Table;
            if (productId > 0)
                query = query.Where(psa => psa.ProductId == productId);
            if (specificationAttributeOptionId > 0)
                query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);
            if (allowFiltering.HasValue)
                query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
            if (!specificationAttributeGroupId.HasValue || specificationAttributeGroupId > 0)
            {
                query = from psa in query
                        join sao in _specificationAttributeOptionRepository.Table
                            on psa.SpecificationAttributeOptionId equals sao.Id
                        join sa in _specificationAttributeRepository.Table
                            on sao.SpecificationAttributeId equals sa.Id
                        where sa.SpecificationAttributeGroupId == specificationAttributeGroupId
                        select psa;
            }
            if (showOnProductPage.HasValue)
                query = query.Where(psa => psa.ShowOnProductPage == showOnProductPage.Value);
            query = query.OrderBy(psa => psa.DisplayOrder).ThenBy(psa => psa.Id);

            var productSpecificationAttributes = await _staticCacheManager.Get(key, async () => await query.ToAsyncEnumerable().ToListAsync());

            return productSpecificationAttributes;
        }

        /// <summary>
        /// Gets a product specification attribute mapping 
        /// </summary>
        /// <param name="productSpecificationAttributeId">Product specification attribute mapping identifier</param>
        /// <returns>Product specification attribute mapping</returns>
        public virtual async Task<ProductSpecificationAttribute> GetProductSpecificationAttributeById(int productSpecificationAttributeId)
        {
            return await _productSpecificationAttributeRepository.GetById(productSpecificationAttributeId);
        }

        /// <summary>
        /// Inserts a product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
        public virtual async Task InsertProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            await _productSpecificationAttributeRepository.Insert(productSpecificationAttribute);
        }

        /// <summary>
        /// Updates the product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
        public virtual async Task UpdateProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            await _productSpecificationAttributeRepository.Update(productSpecificationAttribute);
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

            return await query.ToAsyncEnumerable().CountAsync();
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
