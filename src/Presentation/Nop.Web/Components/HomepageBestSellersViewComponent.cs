using System.Linq;
using System.Threading.Tasks;
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

namespace Nop.Web.Components;

public partial class HomepageBestSellersViewComponent : NopViewComponent
{
    private readonly CatalogSettings _catalogSettings;
    private readonly IAclService _aclService;
    private readonly IOrderReportService _orderReportService;
    private readonly IProductModelFactory _productModelFactory;
    private readonly IProductService _productService;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IStoreContext _storeContext;
    private readonly IStoreMappingService _storeMappingService;

    public HomepageBestSellersViewComponent(
        CatalogSettings catalogSettings,
        IAclService aclService,
        IOrderReportService orderReportService,
        IProductModelFactory productModelFactory,
        IProductService productService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService)
    {
        _catalogSettings = catalogSettings;
        _aclService = aclService;
        _orderReportService = orderReportService;
        _productModelFactory = productModelFactory;
        _productService = productService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int? productThumbPictureSize)
    {
        if (!_catalogSettings.ShowBestsellersOnHomepage || _catalogSettings.NumberOfBestsellersOnHomepage <= 0)
            return Content(string.Empty);

        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
            NopModelCacheDefaults.HomepageBestsellersIdsKey, 
            store);

        // Get bestseller IDs from cache or database
        var report = await _staticCacheManager.GetAsync(cacheKey, async () => 
            await (await _orderReportService.BestSellersReportAsync(
                storeId: store.Id,
                pageSize: _catalogSettings.NumberOfBestsellersOnHomepage))
            .ToListAsync());

        if (report == null || !report.Any())
            return Content(string.Empty);

        // Get product IDs and filter out any invalid ones
        var productIds = report
            .Where(x => x?.ProductId > 0)
            .Select(x => x.ProductId)
            .Distinct()
            .ToArray();

        if (!productIds.Any())
            return Content(string.Empty);

        // Get products with a single database call
        var products = await _productService.GetProductsByIdsAsync(productIds);
        
        // Apply filters in memory
        var availableProducts = new List<Product>();
        foreach (var product in products)
        {
            if (product == null)
                continue;

            var isAvailable = _productService.ProductIsAvailable(product);
            var isAuthorized = await _aclService.AuthorizeAsync(product);
            var isMapped = await _storeMappingService.AuthorizeAsync(product);

            if (isAvailable && isAuthorized && isMapped)
            {
                availableProducts.Add(product);
            }
        }

        if (!availableProducts.Any())
            return Content(string.Empty);

        // Prepare the model
        var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(
            availableProducts, 
            true, 
            true, 
            productThumbPictureSize))
            .ToList();

        return View(model);
    }
}