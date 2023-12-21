using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Plugin.Misc.AbcPromos.Models;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using Nop.Services.Seo;
using Nop.Services.Logging;
using Nop.Services.Catalog;
using System.Collections.Generic;
using Nop.Plugin.Widgets.AbcPromos;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Services.Localization;
using System;
using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Misc.AbcPromos.Controllers
{
    public class CustomCatalogController : BasePublicController
    {
        private readonly IAbcPromoService _abcPromoService;
        private readonly ICategoryService _categoryService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IUrlRecordService _urlRecordService;

        private readonly IProductModelFactory _productModelFactory;
        private readonly ICatalogModelFactory _catalogModelFactory;

        private readonly ILogger _logger;

        private readonly AbcPromosSettings _settings;
        private readonly CatalogSettings _catalogSettings;

        public CustomCatalogController(
            IAbcPromoService abcPromoService,
            ICategoryService categoryService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IUrlRecordService urlRecordService,
            IProductModelFactory productModelFactory,
            ICatalogModelFactory categoryModelFactory,
            ILogger logger,
            AbcPromosSettings settings,
            CatalogSettings catalogSettings,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _abcPromoService = abcPromoService;
            _categoryService = categoryService;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _urlRecordService = urlRecordService;
            _productModelFactory = productModelFactory;
            _catalogModelFactory = categoryModelFactory;
            _logger = logger;
            _settings = settings;
            _catalogSettings = catalogSettings;
        }

        public async Task<IActionResult> Promo(string promoSlug, CatalogProductsCommand command)
        {
            // Set to high to low by default for gifts under
            if (promoSlug.Contains("gifts-under"))
            {
                command.OrderBy = 11;
            }

            var urlRecord = await _urlRecordService.GetBySlugAsync(promoSlug);
            if (urlRecord == null) return InvokeHttp404();

            var promo = await _abcPromoService.GetPromoByIdAsync(urlRecord.EntityId);
            if (promo == null) return InvokeHttp404();

            var shouldDisplay = _settings.IncludeExpiredPromosOnRebatesPromosPage ?
                promo.IsExpired() || promo.IsActive() :
                promo.IsActive();
            if (!shouldDisplay) return InvokeHttp404();

            var promoProducts = await _abcPromoService.GetPublishedProductsByPromoIdAsync(promo.Id);

            // if a category is provided, filter by it
            var filterCategory = await GetFilterCategoryAsync();
            if (filterCategory != null) {
                var filterCategoryIds = (await _categoryService.GetChildCategoryIdsAsync(filterCategory.Id)).ToList();
                filterCategoryIds.Add(filterCategory.Id);
                var categoryFilteredProducts = new List<Product>();
                foreach (var product in promoProducts)
                {
                    var pcs = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id);
                    var pcsCategoryIds = pcs.Select(pc => pc.CategoryId);
                    var pcCategoryIdsWithParents = new List<int>();

                    if (pcsCategoryIds.Intersect(filterCategoryIds).Any())
                    {
                        categoryFilteredProducts.Add(product);
                    }
                }
                
                promoProducts = categoryFilteredProducts;
            }

            promoProducts = SortPromoProducts(promoProducts, command);

            var filteredPromoProducts = promoProducts.Skip(command.PageIndex * 20).Take(20).ToList();

            var model = new PromoListingModel
            {
                Name = promo.ManufacturerId != null ?
                            $"{(await _manufacturerService.GetManufacturerByIdAsync(promo.ManufacturerId.Value)).Name} - {promo.Description}" :
                            promo.Description,
                Products = (await _productModelFactory.PrepareProductOverviewModelsAsync(filteredPromoProducts)).ToList(),
                BannerImageUrl = await promo.GetPromoBannerUrlAsync(),
                PromoFormPopup = promo.GetPopupCommand()
            };

            var pagedList = new PagedList<Product>(
                filteredPromoProducts,
                command.PageIndex,
                20,
                promoProducts.Count
            );
            model.LoadPagedList(pagedList);

            // using duplicate sorting - it would be good to link this to NOP code but it's pretty complex
            await PrepareSortingOptionsAsync(model, command);

            return View("~/Plugins/Widgets.AbcPromos/Views/PromoListing.cshtml", model);
        }

        private async Task<Category> GetFilterCategoryAsync()
        {
            var categorySlug = Request.Query["category"].FirstOrDefault();
            if (categorySlug == null) { return null; }

            var urlRecord = await _urlRecordService.GetBySlugAsync(categorySlug);
            if (urlRecord == null || urlRecord.EntityName != "Category") { return null; }

            return await _categoryService.GetCategoryByIdAsync(urlRecord.EntityId);
        }

        private List<Product> SortPromoProducts(
            IList<Product> promoProducts,
            CatalogProductsCommand command
        )
        {
            if (command.OrderBy == 11)
            {
                return promoProducts.OrderByDescending(p => p.Price).ToList();
            }

            return promoProducts.OrderBy(p => p.Price).ToList();
        }

        private async Task PrepareSortingOptionsAsync(PromoListingModel model, CatalogProductsCommand command)
        {
            //set the order by position by default
            model.OrderBy = command.OrderBy;
            command.OrderBy = (int)ProductSortingEnum.Position;

            //ensure that product sorting is enabled
            if (!_catalogSettings.AllowProductSorting)
                return;

            //get active sorting options
            var activeSortingOptionsIds = Enum.GetValues(typeof(ProductSortingEnum)).Cast<int>()
                .Except(_catalogSettings.ProductSortingEnumDisabled).ToList();
            if (!activeSortingOptionsIds.Any())
                return;

            //order sorting options
            var orderedActiveSortingOptions = activeSortingOptionsIds
                .Select(id => new { Id = id, Order = _catalogSettings.ProductSortingEnumDisplayOrder.TryGetValue(id, out var order) ? order : id })
                .OrderBy(option => option.Order).ToList();

            model.AllowProductSorting = true;
            command.OrderBy = model.OrderBy ?? orderedActiveSortingOptions.FirstOrDefault().Id;

            //prepare available model sorting options
            foreach (var option in orderedActiveSortingOptions)
            {
                model.AvailableSortOptions.Add(new SelectListItem
                {
                    Text = await _localizationService.GetLocalizedEnumAsync((ProductSortingEnum)option.Id),
                    Value = option.Id.ToString(),
                    Selected = option.Id == command.OrderBy
                });
            }

            // Promo specific - only using price options
            model.AvailableSortOptions =
                model.AvailableSortOptions.Where(aso => aso.Text.Contains("Price:")).ToList();
        }
    }
}