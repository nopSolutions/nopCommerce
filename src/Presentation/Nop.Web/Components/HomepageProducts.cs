using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class HomepageProductsViewComponent : NopViewComponent
    {
        private readonly IAclService _aclService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IStoreMappingService _storeMappingService;

        public HomepageProductsViewComponent(IAclService aclService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IStoreMappingService storeMappingService)
        {
            _aclService = aclService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _storeMappingService = storeMappingService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? productThumbPictureSize)
        {
            var products = await _productService.GetAllProductsDisplayedOnHomepageAsync();
            //ACL and store mapping
            products = products.Where(p => _aclService.AuthorizeAsync(p).Result && _storeMappingService.AuthorizeAsync(p).Result).ToList();
            //availability dates
            products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();

            products = products.Where(p => p.VisibleIndividually).ToList();

            if (!products.Any())
                return Content("");

            var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, true, true, productThumbPictureSize)).ToList();

            return View(model);
        }
    }
}