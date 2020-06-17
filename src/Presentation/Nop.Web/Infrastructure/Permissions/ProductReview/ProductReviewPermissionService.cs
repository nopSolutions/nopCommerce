using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Web.Infrastructure.Cache;

namespace Nop.Web.Infrastructure.Permissions.ProductReview
{
    public class ProductReviewPermissionService : IProductReviewPermissionService
    {
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly CatalogSettings _catalogSettings;

        public ProductReviewPermissionService(ICustomerService customerService, IProductService productService,
            IStaticCacheManager staticCacheManager, CatalogSettings catalogSettings, ICacheKeyService cacheKeyService)
        {
            _customerService = customerService;
            _productService = productService;
            _staticCacheManager = staticCacheManager;
            _catalogSettings = catalogSettings;
            _cacheKeyService = cacheKeyService;
        }

        public bool CanAdd(Customer customer, Product product, Store store)
        {
            var customerProductReviewId = _cacheKeyService.PrepareKeyForDefaultCache(
                NopModelCacheDefaults.ProductReviewsCurrentCustomerModelKey, product.Id, customer.Id,
                store.Id);

            return !_catalogSettings.OneReviewPerProductFromCustomer || _staticCacheManager.Get(customerProductReviewId,
                () => _productService.GetAllProductReviews(customerId: customer.Id, productId: product.Id,
                    storeId: store.Id, pageSize: 1).TotalCount) == 0;
        }
    }
}