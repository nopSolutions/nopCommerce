using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Services.Localization;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Specification attribute service
    /// </summary>
    public partial class SpecificationAttributeService : ISpecificationAttributeService
    {
        #region Constants
        private const string SPECIFICATIONATTRIBUTE_BY_ID_KEY = "Nop.specificationattributes.id-{0}";
        private const string SPECIFICATIONATTRIBUTEOPTION_BY_ID_KEY = "Nop.specificationattributeoptions.id-{0}";
        private const string PRODUCTSPECIFICATIONATTRIBUTE_ALLBYPRODUCTID_KEY = "Nop.productspecificationattribute.allbyproductid-{0}-{1}-{2}";
        private const string SPECIFICATIONATTRIBUTE_PATTERN_KEY = "Nop.specificationattributes.";
        private const string SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY = "Nop.specificationattributeoptions.";
        private const string PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY = "Nop.productspecificationattribute.";
        #endregion

        #region Fields
        
        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;
        private readonly IRepository<ProductSpecificationAttribute> _productSpecificationAttributeRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductVariant> _productVariantRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IProductService _productService;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="specificationAttributeRepository">Specification attribute repository</param>
        /// <param name="specificationAttributeOptionRepository">Specification attribute option repository</param>
        /// <param name="productSpecificationAttributeRepository">Product specification attribute repository</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="productVariantRepository">Product variant repository</param>
        /// <param name="productService">Product service</param>
        /// <param name="eventPublisher"></param>
        public SpecificationAttributeService(ICacheManager cacheManager,
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository,
            IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository,
            IRepository<Product> productRepository, 
             IRepository<ProductVariant> productVariantRepository,
            IProductService productService,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _specificationAttributeRepository = specificationAttributeRepository;
            _specificationAttributeOptionRepository = specificationAttributeOptionRepository;
            _productSpecificationAttributeRepository = productSpecificationAttributeRepository;
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _productService = productService;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        #region Specification attribute

        /// <summary>
        /// Gets a specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute</returns>
        public virtual SpecificationAttribute GetSpecificationAttributeById(int specificationAttributeId)
        {
            if (specificationAttributeId == 0)
                return null;

            string key = string.Format(SPECIFICATIONATTRIBUTE_BY_ID_KEY, specificationAttributeId);
            return _cacheManager.Get(key, () =>
            {
                var sa = _specificationAttributeRepository.GetById(specificationAttributeId);
                return sa;
            });
        }

        /// <summary>
        /// Gets specification attributes
        /// </summary>
        /// <returns>Specification attributes</returns>
        public virtual IList<SpecificationAttribute> GetSpecificationAttributes()
        {
            var query = from sa in _specificationAttributeRepository.Table
                        orderby sa.DisplayOrder
                        select sa;
            var specificationAttributes = query.ToList();
            return specificationAttributes;
        }

        /// <summary>
        /// Deletes a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual void DeleteSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException("specificationAttribute");

            _specificationAttributeRepository.Delete(specificationAttribute);

            _eventPublisher.EntityDeleted(specificationAttribute);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        /// <summary>
        /// Inserts a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual void InsertSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException("specificationAttribute");

            _specificationAttributeRepository.Insert(specificationAttribute);

            _eventPublisher.EntityInserted(specificationAttribute);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual void UpdateSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException("specificationAttribute");

            _specificationAttributeRepository.Update(specificationAttribute);

            _eventPublisher.EntityUpdated(specificationAttribute);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        #endregion

        #region Specification attribute option

        /// <summary>
        /// Gets a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier</param>
        /// <returns>Specification attribute option</returns>
        public virtual SpecificationAttributeOption GetSpecificationAttributeOptionById(int specificationAttributeOptionId)
        {
            if (specificationAttributeOptionId == 0)
                return null;

            string key = string.Format(SPECIFICATIONATTRIBUTEOPTION_BY_ID_KEY, specificationAttributeOptionId);
            return _cacheManager.Get(key, () =>
            {
                var sao = _specificationAttributeOptionRepository.GetById(specificationAttributeOptionId);
                return sao;
            });
        }

        /// <summary>
        /// Gets a specification attribute option by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute option</returns>
        public virtual IList<SpecificationAttributeOption> GetSpecificationAttributeOptionsBySpecificationAttribute(int specificationAttributeId)
        {
            var query = from sao in _specificationAttributeOptionRepository.Table
                        orderby sao.DisplayOrder
                        where sao.SpecificationAttributeId == specificationAttributeId
                        select sao;
            var specificationAttributeOptions = query.ToList();
            return specificationAttributeOptions;
        }

        /// <summary>
        /// Deletes a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual void DeleteSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            _specificationAttributeOptionRepository.Delete(specificationAttributeOption);

            _eventPublisher.EntityDeleted(specificationAttributeOption);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        /// <summary>
        /// Inserts a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual void InsertSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            _specificationAttributeOptionRepository.Insert(specificationAttributeOption);

            _eventPublisher.EntityInserted(specificationAttributeOption);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual void UpdateSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            _specificationAttributeOptionRepository.Update(specificationAttributeOption);

            _eventPublisher.EntityUpdated(specificationAttributeOption);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        #endregion

        #region Product specification attribute

        /// <summary>
        /// Deletes a product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute</param>
        public virtual void DeleteProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException("productSpecificationAttribute");

            _productSpecificationAttributeRepository.Delete(productSpecificationAttribute);

            _eventPublisher.EntityDeleted(productSpecificationAttribute);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        /// <summary>
        /// Gets a product specification attribute mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product specification attribute mapping collection</returns>
        public virtual IList<ProductSpecificationAttribute> GetProductSpecificationAttributesByProductId(int productId)
        {
            return GetProductSpecificationAttributesByProductId(productId, null, null);
        }

        /// <summary>
        /// Gets a product specification attribute mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 0 to load attributes with AllowFiltering set to true, null to load all attributes</param>
        /// <param name="showOnProductPage">0 to load attributes with ShowOnProductPage set to false, 0 to load attributes with ShowOnProductPage set to true, null to load all attributes</param>
        /// <returns>Product specification attribute mapping collection</returns>
        public virtual IList<ProductSpecificationAttribute> GetProductSpecificationAttributesByProductId(int productId, 
            bool? allowFiltering, bool? showOnProductPage)
        {
            string allowFilteringCacheStr = "null";
            if (allowFiltering.HasValue)
                allowFilteringCacheStr = allowFiltering.ToString();
            string showOnProductPageCacheStr = "null";
            if (showOnProductPage.HasValue)
                showOnProductPageCacheStr = showOnProductPage.ToString();
            string key = string.Format(PRODUCTSPECIFICATIONATTRIBUTE_ALLBYPRODUCTID_KEY, productId, allowFilteringCacheStr, showOnProductPageCacheStr);
            
            return _cacheManager.Get(key, () =>
            {
                var query = (IQueryable<ProductSpecificationAttribute>)_productSpecificationAttributeRepository.Table;
                query = query.Where(psa => psa.ProductId == productId);
                if (allowFiltering.HasValue)
                    query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
                if (showOnProductPage.HasValue)
                    query = query.Where(psa => psa.ShowOnProductPage == showOnProductPage.Value);
                query = query.OrderBy(psa => psa.DisplayOrder);

                var productSpecificationAttributes = query.ToList();
                return productSpecificationAttributes;
            });
        }

        /// <summary>
        /// Gets a product specification attribute mapping 
        /// </summary>
        /// <param name="productSpecificationAttributeId">Product specification attribute mapping identifier</param>
        /// <returns>Product specification attribute mapping</returns>
        public virtual ProductSpecificationAttribute GetProductSpecificationAttributeById(int productSpecificationAttributeId)
        {
            if (productSpecificationAttributeId == 0)
                return null;
            
            var productSpecificationAttribute = _productSpecificationAttributeRepository.GetById(productSpecificationAttributeId);
            return productSpecificationAttribute;
        }

        /// <summary>
        /// Inserts a product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
        public virtual void InsertProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException("productSpecificationAttribute");

            _productSpecificationAttributeRepository.Insert(productSpecificationAttribute);

            _eventPublisher.EntityInserted(productSpecificationAttribute);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
        public virtual void UpdateProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException("productSpecificationAttribute");

            _productSpecificationAttributeRepository.Update(productSpecificationAttribute);

            _eventPublisher.EntityUpdated(productSpecificationAttribute);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        #endregion

        #region Specification attribute option filter
        
        /// <summary>
        /// Gets a filtered product specification attribute mapping collection by category id
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="workContext">Work context</param>
        /// <returns>Product specification attribute mapping collection</returns>
        public virtual IList<SpecificationAttributeOptionFilter> GetSpecificationAttributeOptionFilter(int categoryId, IWorkContext workContext)
        {
            if (categoryId == 0)
                throw new ArgumentException("Category identifier could not be null", "categoryId");


            //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact. 
            //That's why we pass the date value
            var nowUtc = DateTime.UtcNow;

            var query = from p in _productRepository.Table
                        from pv in p.ProductVariants.DefaultIfEmpty()
                        from pc in p.ProductCategories.Where(pc => pc.CategoryId == categoryId)
                        join psa in _productSpecificationAttributeRepository.Table on p.Id equals psa.ProductId
                        join sao in _specificationAttributeOptionRepository.Table on psa.SpecificationAttributeOptionId equals sao.Id
                        join sa in _specificationAttributeRepository.Table on sao.SpecificationAttributeId equals sa.Id
                        where p.Published && pv.Published && !p.Deleted && psa.AllowFiltering &&
                        (!pv.AvailableStartDateTimeUtc.HasValue || pv.AvailableStartDateTimeUtc.Value < nowUtc) &&
                        (!pv.AvailableEndDateTimeUtc.HasValue || pv.AvailableEndDateTimeUtc.Value > nowUtc)
                        select new {sa, sao};
            
            //only distinct attributes (group by ID)
            query = from x in query
                    group x by x.sao.Id into xGroup
                    orderby xGroup.Key
                    select xGroup.FirstOrDefault();

            var result = new List<SpecificationAttributeOptionFilter>();
            var items = query.ToList();
            foreach (var item in items)
                result.Add(new SpecificationAttributeOptionFilter()
                {
                    SpecificationAttributeId = item.sa.Id,
                    SpecificationAttributeName = item.sa.GetLocalized(sa => sa.Name, workContext.WorkingLanguage.Id),
                    DisplayOrder = item.sa.DisplayOrder,
                    SpecificationAttributeOptionId = item.sao.Id,
                    SpecificationAttributeOptionName = item.sao.GetLocalized(sao => sao.Name, workContext.WorkingLanguage.Id)
                });
            result = result.OrderBy(saof => saof.DisplayOrder)
                .ThenBy(saof => saof.SpecificationAttributeName)
                .ThenBy(saof => saof.SpecificationAttributeOptionName).ToList();


            //old method
            //var products = _productService.SearchProducts(categoryId, 0, null, null, null, 0, 0,
            //    null, false, workContext.WorkingLanguage.Id, null, ProductSortingEnum.Position,
            //    0, int.MaxValue, false);
            //foreach (var product in products)
            //{
            //    foreach (var psa in product.ProductSpecificationAttributes.Where(psa => psa.AllowFiltering))
            //    {
            //        var specificationAttributeOptionId = psa.SpecificationAttributeOption.Id;
            //        if (result.Find(saof => saof.SpecificationAttributeOptionId == specificationAttributeOptionId) == null)
            //        {
            //            result.Add(new SpecificationAttributeOptionFilter()
            //                {
            //                    SpecificationAttributeId = psa.SpecificationAttributeOption.SpecificationAttribute.Id,
            //                    SpecificationAttributeName = psa.SpecificationAttributeOption.SpecificationAttribute.GetLocalized(sa => sa.Name, workContext.WorkingLanguage.Id),
            //                    DisplayOrder = psa.SpecificationAttributeOption.SpecificationAttribute.DisplayOrder,
            //                    SpecificationAttributeOptionId = specificationAttributeOptionId,
            //                    SpecificationAttributeOptionName = psa.SpecificationAttributeOption.GetLocalized(sao => sao.Name, workContext.WorkingLanguage.Id)
            //                });
            //        }
            //    }
            //}
            //result = result.OrderBy(saof => saof.DisplayOrder)
            //    .ThenBy(saof => saof.SpecificationAttributeName)
            //    .ThenBy(saof => saof.SpecificationAttributeOptionName).ToList();
            return result;
        }

        #endregion

        #endregion
    }
}
