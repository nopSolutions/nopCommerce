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
    public class HomepageBestSellersViewComponent : NopViewComponent
    {
        private readonly CatalogSettings _catalogSettings;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IOrderReportService _orderReportService;
        private readonly IStaticCacheManager _cacheManager;

        public HomepageBestSellersViewComponent(CatalogSettings catalogSettings,
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

        public IViewComponentResult Invoke(int? productThumbPictureSize)
        {
            if (!_catalogSettings.ShowBestsellersOnHomepage || _catalogSettings.NumberOfBestsellersOnHomepage == 0)
                return Content("");

            //load and cache report
            var report = _cacheManager.Get(string.Format(ModelCacheEventConsumer.HOMEPAGE_BESTSELLERS_IDS_KEY, _storeContext.CurrentStore.Id),
                () => _orderReportService.BestSellersReport(
                        storeId: _storeContext.CurrentStore.Id,
                        pageSize: _catalogSettings.NumberOfBestsellersOnHomepage)
                    .ToList());

            //load products
            var products = _productService.GetProductsByIds(report.Select(x => x.ProductId).ToArray());
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            if (!products.Any())
                return Content("");

            //prepare model
            var model = _productModelFactory.PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();
            return View(model);
        }
    }
}
