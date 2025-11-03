using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.Infrastructure;
using Nop.Services.Stores;
using System.Threading.Tasks;
using Nop.Services.Catalog;

namespace Nop.Plugin.Api.Services
{
    public class CategoryApiService : ICategoryApiService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<ProductCategory> _productCategoryMappingRepository;
        private readonly IStoreMappingService _storeMappingService;
		private readonly ICategoryService _categoryService;

		public CategoryApiService(
            IRepository<Category> categoryRepository,
            IRepository<ProductCategory> productCategoryMappingRepository,
            IStoreMappingService storeMappingService,
            ICategoryService categoryService)
        {
            _categoryRepository = categoryRepository;
            _productCategoryMappingRepository = productCategoryMappingRepository;
            _storeMappingService = storeMappingService;
			this._categoryService = categoryService;
		}

        public IList<Category> GetCategories(
            IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            int limit = Constants.Configurations.DefaultLimit, int page = Constants.Configurations.DefaultPageValue,
            int sinceId = Constants.Configurations.DefaultSinceId,
            int? productId = null,
            bool? publishedStatus = null, int? parentCategoryId = null)
        {
            var query = GetCategoriesQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax, publishedStatus, productId, ids, parentCategoryId);


            if (sinceId > 0)
            {
                query = query.Where(c => c.Id > sinceId);
            }

            return new ApiList<Category>(query, page - 1, limit);
        }

        public Category GetCategoryById(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var category = _categoryRepository.Table.FirstOrDefault(cat => cat.Id == id && !cat.Deleted);

            return category;
        }

        public async Task<int> GetCategoriesCountAsync(
            DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            bool? publishedStatus = null, int? productId = null, int? parentCategoryId = null)
        {
            var query = GetCategoriesQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax, publishedStatus, productId, ids: null, parentCategoryId);

            return await query.WhereAwait(async c => await _storeMappingService.AuthorizeAsync(c)).CountAsync();
        }

        public async Task<IDictionary<int, IList<Category>>> GetProductCategories(IList<int> productIds)
        {
            var productCategories = await _categoryService.GetProductCategoryIdsAsync(productIds.ToArray());
            return productCategories.ToDictionary(prodCat => prodCat.Key, prodCat => prodCat.Value.Select(catId => GetCategoryById(catId)).ToList() as IList<Category>);
        }

        private IQueryable<Category> GetCategoriesQuery(
            DateTime? createdAtMin, DateTime? createdAtMax, DateTime? updatedAtMin, DateTime? updatedAtMax,
            bool? publishedStatus, int? productId, IList<int> ids, int? parentCategoryId)
        {
            var query = _categoryRepository.Table;

            if (ids != null && ids.Count > 0)
            {
                query = query.Where(c => ids.Contains(c.Id));
            }

            if (parentCategoryId != null)
            {
                query = query.Where(c => c.ParentCategoryId == parentCategoryId.Value);
            }

            if (publishedStatus != null)
            {
                query = query.Where(c => c.Published == publishedStatus.Value);
            }

            query = query.Where(c => !c.Deleted);

            if (createdAtMin != null)
            {
                query = query.Where(c => c.CreatedOnUtc > createdAtMin.Value);
            }

            if (createdAtMax != null)
            {
                query = query.Where(c => c.CreatedOnUtc < createdAtMax.Value);
            }

            if (updatedAtMin != null)
            {
                query = query.Where(c => c.UpdatedOnUtc > updatedAtMin.Value);
            }

            if (updatedAtMax != null)
            {
                query = query.Where(c => c.UpdatedOnUtc < updatedAtMax.Value);
            }

            if (productId != null)
            {
                var categoryMappingsForProduct = from productCategoryMapping in _productCategoryMappingRepository.Table
                                                 where productCategoryMapping.ProductId == productId
                                                 select productCategoryMapping;

                query = from category in query
                        join productCategoryMapping in categoryMappingsForProduct on category.Id equals productCategoryMapping.CategoryId
                        select category;
            }

            query = query.OrderBy(category => category.Id);

            return query;
        }
	}
}
