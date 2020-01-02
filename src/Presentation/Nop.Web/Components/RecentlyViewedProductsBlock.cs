using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Components
{
    public class RecentlyViewedProductsBlockViewComponent : NopViewComponent
    {
        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly IStoreMappingService _storeMappingService;

        public RecentlyViewedProductsBlockViewComponent(CatalogSettings catalogSettings,
            IAclService aclService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IRecentlyViewedProductsService recentlyViewedProductsService,
            IStoreMappingService storeMappingService)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _recentlyViewedProductsService = recentlyViewedProductsService;
            _storeMappingService = storeMappingService;
        }

        public IViewComponentResult Invoke(int? productThumbPictureSize, bool? preparePriceModel)
        {
            if (!_catalogSettings.RecentlyViewedProductsEnabled)
                return Content("");

            var preparePictureModel = productThumbPictureSize.HasValue;
            var products = _recentlyViewedProductsService.GetRecentlyViewedProducts(_catalogSettings.RecentlyViewedProductsNumber);

            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();

            if (!products.Any())
                return Content("");

            //prepare model
            var model = new List<ProductOverviewModel>();
            model.AddRange(_productModelFactory.PrepareProductOverviewModels(products,
                preparePriceModel.GetValueOrDefault(),
                preparePictureModel,
                productThumbPictureSize));

            return View(model);
        }
    }
}