using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Infrastructure.Cache;

namespace Nop.Web.Components
{
    public class ProductsAlsoPurchasedViewComponent : NopViewComponent
    {
        private readonly CatalogSettings _catalogSettings;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IOrderReportService _orderReportService;
        private readonly IStaticCacheManager _cacheManager;

        public ProductsAlsoPurchasedViewComponent(CatalogSettings catalogSettings,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IStoreContext storeContext,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IOrderReportService orderReportService,
            IStaticCacheManager cacheManager)
        {
            this._catalogSettings = catalogSettings;
            this._productModelFactory = productModelFactory;
            this._productService = productService;
            this._storeContext = storeContext;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._orderReportService = orderReportService;
            this._cacheManager = cacheManager;
        }

        public IViewComponentResult Invoke(int productId, int? productThumbPictureSize)
        {
            if (!_catalogSettings.ProductsAlsoPurchasedEnabled)
                return Content("");

            //load and cache report
            var productIds = _cacheManager.Get(string.Format(ModelCacheEventConsumer.PRODUCTS_ALSO_PURCHASED_IDS_KEY, productId, _storeContext.CurrentStore.Id),
                () => _orderReportService.GetAlsoPurchasedProductsIds(_storeContext.CurrentStore.Id, productId, _catalogSettings.ProductsAlsoPurchasedNumber)
            );

            //load products
            var products = _productService.GetProductsByIds(productIds);
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            if (!products.Any())
                return Content("");

            var model = _productModelFactory.PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();
            return View(model);
        }
    }
}
