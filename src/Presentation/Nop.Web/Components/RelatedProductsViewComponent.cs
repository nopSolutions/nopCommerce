using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

/// <summary>
/// View component for displaying related products
/// </summary>
public partial class RelatedProductsViewComponent : NopViewComponent
{
    #region Fields

    private readonly IAclService _aclService;
    private readonly IProductModelFactory _productModelFactory;
    private readonly IProductService _productService;
    private readonly IStoreMappingService _storeMappingService;
    private readonly ILogger<RelatedProductsViewComponent> _logger;
    private readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public RelatedProductsViewComponent(
        IAclService aclService,
        IProductModelFactory productModelFactory,
        IProductService productService,
        IStoreMappingService storeMappingService,
        ILogger<RelatedProductsViewComponent> logger,
        IWorkContext workContext)
    {
        _aclService = aclService ?? throw new ArgumentNullException(nameof(aclService));
        _productModelFactory = productModelFactory ?? throw new ArgumentNullException(nameof(productModelFactory));
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _storeMappingService = storeMappingService ?? throw new ArgumentNullException(nameof(storeMappingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _workContext = workContext ?? throw new ArgumentNullException(nameof(workContext));
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke the widget view component
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="productThumbPictureSize">Picture size for thumbnails</param>
    /// <returns>View component result</returns>
    public async Task<IViewComponentResult> InvokeAsync(int productId, int? productThumbPictureSize = null)
    {
        try
        {
            if (productId <= 0)
                return Content(string.Empty);

            // Load related product IDs in a single query
            var relatedProductIds = (await _productService.GetRelatedProductsByProductId1Async(productId))
                .Select(x => x.ProductId2)
                .ToArray();

            if (relatedProductIds.Length == 0)
                return Content(string.Empty);

            // Get current store and customer
            var currentStore = await _workContext.GetCurrentStoreAsync();
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();

            // Load products with all necessary data in optimized way
            var products = await _productService.GetProductsByIdsAsync(relatedProductIds, 
                cacheResults: true, 
                includeDeleted: false);

            if (products?.Any() != true)
                return Content(string.Empty);

            // Filter products in memory after loading
            var filteredProducts = await products
                .WhereAwait(async p => 
                    p.Published &&
                    p.VisibleIndividually &&
                    await _productService.ProductIsAvailableAsync(p, currentStore.Id) &&
                    await _aclService.AuthorizeAsync(p, currentCustomer) &&
                    await _storeMappingService.AuthorizeAsync(p, currentStore.Id))
                .ToListAsync();

            if (!filteredProducts.Any())
                return Content(string.Empty);

            // Prepare the model
            var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(
                filteredProducts,
                preparePriceModel: true,
                preparePictureModel: true,
                productThumbPictureSize: productThumbPictureSize))
                .ToList();

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while preparing related products for product ID {ProductId}", productId);
            return Content(string.Empty);
        }
    }

    #endregion
}