using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Misc.Custom.Factories
{
    public class CustomCatalogModelFactory : CatalogModelFactory
    {
        private readonly CatalogSettings _catalogSettings;
        private readonly IStoreContext _storeContext;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IManufacturerService _manufacturerService;


        public CustomCatalogModelFactory(BlogSettings blogSettings,
            CatalogSettings catalogSettings, 
            DisplayDefaultMenuItemSettings displayDefaultMenuItemSettings,
            ForumSettings forumSettings,
            ICategoryService categoryService,
            ICategoryTemplateService categoryTemplateService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IManufacturerTemplateService manufacturerTemplateService,
            INopUrlHelper nopUrlHelper,
            IPictureService pictureService, IProductModelFactory productModelFactory,
            IProductService productService, IProductTagService productTagService,
            ISearchTermService searchTermService,
            ISpecificationAttributeService specificationAttributeService,
            IStaticCacheManager staticCacheManager, IStoreContext storeContext,
            ITopicService topicService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService, IWebHelper webHelper,
            IWorkContext workContext, MediaSettings mediaSettings,
            VendorSettings vendorSettings) : base(blogSettings, catalogSettings, displayDefaultMenuItemSettings, forumSettings, categoryService, categoryTemplateService, currencyService, customerService, eventPublisher, httpContextAccessor, localizationService, manufacturerService, manufacturerTemplateService, nopUrlHelper, pictureService, productModelFactory, productService, productTagService, searchTermService, specificationAttributeService, staticCacheManager, storeContext, topicService, urlRecordService, vendorService, webHelper, workContext, mediaSettings, vendorSettings)
        {
            _catalogSettings = catalogSettings;
            _storeContext = storeContext;
            _categoryService = categoryService;
            _productService =productService;

            _specificationAttributeService = specificationAttributeService;
            _manufacturerService = manufacturerService;
        }

        public async override Task<CatalogProductsModel> PrepareCategoryProductsModelAsync(Category category, CatalogProductsCommand command)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new CatalogProductsModel
            {
                UseAjaxLoading = _catalogSettings.UseAjaxCatalogProductsLoading
            };

            var currentStore = await _storeContext.GetCurrentStoreAsync();

            //sorting
            await PrepareSortingOptionsAsync(model, command);
            //view mode
            await PrepareViewModesAsync(model, command);
            //page size
            await PreparePageSizeOptionsAsync(model, command, category.AllowCustomersToSelectPageSize,
                category.PageSizeOptions, category.PageSize);

            var categoryIds = new List<int> { category.Id };

            //include subcategories
            if (_catalogSettings.ShowProductsFromSubcategories)
                categoryIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(category.Id, currentStore.Id));

            //price range
            PriceRangeModel selectedPriceRange = null;
            if (_catalogSettings.EnablePriceRangeFiltering && category.PriceRangeFiltering)
            {
                selectedPriceRange = await GetConvertedPriceRangeAsync(command);

                PriceRangeModel availablePriceRange = null;
                if (!category.ManuallyPriceRange)
                {
                    async Task<decimal?> getProductPriceAsync(ProductSortingEnum orderBy)
                    {
                        var products = await _productService.SearchProductsAsync(0, 1,
                            categoryIds: categoryIds,
                            storeId: currentStore.Id,
                            visibleIndividuallyOnly: true,
                            excludeFeaturedProducts: !_catalogSettings.IgnoreFeaturedProducts && !_catalogSettings.IncludeFeaturedProductsInNormalLists,
                            orderBy: orderBy);

                        return products?.FirstOrDefault()?.Price ?? 0;
                    }

                    availablePriceRange = new PriceRangeModel
                    {
                        From = await getProductPriceAsync(ProductSortingEnum.PriceAsc),
                        To = await getProductPriceAsync(ProductSortingEnum.PriceDesc)
                    };
                }
                else
                {
                    availablePriceRange = new PriceRangeModel
                    {
                        From = category.PriceFrom,
                        To = category.PriceTo
                    };
                }

                model.PriceRangeFilter = await PreparePriceRangeFilterAsync(selectedPriceRange, availablePriceRange);
            }

            //filterable options
            var filterableOptions = await _specificationAttributeService
                .GetFiltrableSpecificationAttributeOptionsByCategoryIdAsync(category.Id);

            model.SpecificationFilter = await PrepareSpecificationFilterModel(command.SpecificationOptionIds, filterableOptions);

            //filterable manufacturers
            if (_catalogSettings.EnableManufacturerFiltering)
            {
                var manufacturers = await _manufacturerService.GetManufacturersByCategoryIdAsync(category.Id);

                model.ManufacturerFilter = await PrepareManufacturerFilterModel(command.ManufacturerIds, manufacturers);
            }

            var filteredSpecs = command.SpecificationOptionIds is null ? null : filterableOptions.Where(fo => command.SpecificationOptionIds.Contains(fo.Id)).ToList();

            //products
            var products = await _productService.SearchProductsAsync(
                command.PageNumber - 1,
                command.PageSize,
                categoryIds: categoryIds,
                storeId: currentStore.Id,
                visibleIndividuallyOnly: true,
                excludeFeaturedProducts: !_catalogSettings.IgnoreFeaturedProducts && !_catalogSettings.IncludeFeaturedProductsInNormalLists,
                priceMin: selectedPriceRange?.From,
                priceMax: selectedPriceRange?.To,
                manufacturerIds: command.ManufacturerIds,
                filteredSpecOptions: filteredSpecs,
                orderBy: (ProductSortingEnum)command.OrderBy);

            var isFiltering = filterableOptions.Any() || selectedPriceRange?.From is not null;
            //await PrepareCatalogProductsAsync(model, products, isFiltering);

            //customization start
            if (!category.Name.Contains("Pricing", StringComparison.OrdinalIgnoreCase))
            {
                var productsCustomSimple = await GetCustomProducts(products, command);
                //await PrepareCatalogProductsCustomAsync(model, productsCustomSimple, isFiltering);
                await PrepareCatalogProductsAsync(model, products, isFiltering);
            }
            else
                await PrepareCatalogProductsAsync(model, products, isFiltering);

            //customization end

            return model;
        }
    }
}
