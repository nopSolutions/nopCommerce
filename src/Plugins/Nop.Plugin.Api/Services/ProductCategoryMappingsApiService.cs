using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DataStructures;

namespace Nop.Plugin.Api.Services
{
    public class ProductCategoryMappingsApiService : IProductCategoryMappingsApiService
    {
        private readonly IRepository<ProductCategory> _productCategoryMappingsRepository;

        public ProductCategoryMappingsApiService(IRepository<ProductCategory> productCategoryMappingsRepository)
        {
            _productCategoryMappingsRepository = productCategoryMappingsRepository;
        }

        public IList<ProductCategory> GetMappings(int? productId = null, 
            int? categoryId = null, int limit = Configurations.DefaultLimit, 
            int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetMappingsQuery(productId, categoryId, sinceId);

            return new ApiList<ProductCategory>(query, page - 1, limit);
        }

        public int GetMappingsCount(int? productId = null, int? categoryId = null)
        {
            return GetMappingsQuery(productId, categoryId).Count();
        }

        public ProductCategory GetById(int id)
        {
            if (id <= 0)
                return null;

            return _productCategoryMappingsRepository.GetById(id);
        }

        private IQueryable<ProductCategory> GetMappingsQuery(int? productId = null, 
            int? categoryId = null, int sinceId = Configurations.DefaultSinceId)
        {
            var query = _productCategoryMappingsRepository.Table;

            if (productId != null)
            {
                query = query.Where(mapping => mapping.ProductId == productId);
            }

            if (categoryId != null)
            {
                query = query.Where(mapping => mapping.CategoryId == categoryId);
            }

            if (sinceId > 0)
            {
                query = query.Where(mapping => mapping.Id > sinceId);
            }

            query = query.OrderBy(mapping => mapping.Id);

            return query;
        }
    }
}