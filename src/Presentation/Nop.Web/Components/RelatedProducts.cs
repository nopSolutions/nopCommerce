using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Infrastructure.Cache;

namespace Nop.Web.Components
{
    public class RelatedProductsViewComponent : NopViewComponent
    {
        private readonly IAclService _aclService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;

        public RelatedProductsViewComponent(IAclService aclService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService)
        {
            _aclService = aclService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _cacheManager = cacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
        }

        public IViewComponentResult Invoke(int productId, int? productThumbPictureSize)
        {
            //load and cache report
            var productIds = _cacheManager.Get(string.Format(NopModelCacheDefaults.ProductsRelatedIdsKey, productId, _storeContext.CurrentStore.Id),
                () => _productService.GetRelatedProductsByProductId1(productId).Select(x => x.ProductId2).ToArray());

            //load products
            var products = _productService.GetProductsByIds(productIds);
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();
            //visible individually
            products = products.Where(p => p.VisibleIndividually).ToList();

            if (!products.Any())
                return Content("");

            var model = _productModelFactory.PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();
            return View(model);
        }
    }
}