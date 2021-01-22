using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
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
using Nop.Web.Framework.Events;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;

namespace Nop.Web.Factories
{
    public partial class CatalogModelFactory : ICatalogModelFactory
    {
        #region Fields

        private readonly BlogSettings _blogSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly DisplayDefaultMenuItemSettings _displayDefaultMenuItemSettings;
        private readonly ForumSettings _forumSettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ICategoryService _categoryService;
        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IManufacturerTemplateService _manufacturerTemplateService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly ISearchTermService _searchTermService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly ITopicService _topicService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorService _vendorService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public CatalogModelFactory(BlogSettings blogSettings,
            CatalogSettings catalogSettings,
            DisplayDefaultMenuItemSettings displayDefaultMenuItemSettings,
            ForumSettings forumSettings,
            IActionContextAccessor actionContextAccessor,
            ICategoryService categoryService,
            ICategoryTemplateService categoryTemplateService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IManufacturerTemplateService manufacturerTemplateService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IProductTagService productTagService,
            ISearchTermService searchTermService,
            ISpecificationAttributeService specificationAttributeService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            ITopicService topicService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            VendorSettings vendorSettings)
        {
            _blogSettings = blogSettings;
            _catalogSettings = catalogSettings;
            _displayDefaultMenuItemSettings = displayDefaultMenuItemSettings;
            _forumSettings = forumSettings;
            _actionContextAccessor = actionContextAccessor;
            _categoryService = categoryService;
            _categoryTemplateService = categoryTemplateService;
            _currencyService = currencyService;
            _customerService = customerService;
            _eventPublisher = eventPublisher;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _manufacturerTemplateService = manufacturerTemplateService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _productTagService = productTagService;
            _searchTermService = searchTermService;
            _specificationAttributeService = specificationAttributeService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _topicService = topicService;
            _urlHelperFactory = urlHelperFactory;
            _urlRecordService = urlRecordService;
            _vendorService = vendorService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        protected virtual CategorySimpleModel GetCategorySimpleModel(XElement elem)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            var model = new CategorySimpleModel
            {
                Id = int.Parse(elem.XPathSelectElement("Id").Value),
                Name = elem.XPathSelectElement("Name").Value,
                SeName = elem.XPathSelectElement("SeName").Value,

                NumberOfProducts = !string.IsNullOrEmpty(elem.XPathSelectElement("NumberOfProducts").Value)
                    ? int.Parse(elem.XPathSelectElement("NumberOfProducts").Value)
                    : (int?)null,

                IncludeInTopMenu = bool.Parse(elem.XPathSelectElement("IncludeInTopMenu").Value),
                HaveSubCategories = bool.Parse(elem.XPathSelectElement("HaveSubCategories").Value),
                Route = urlHelper.RouteUrl("Category", new { SeName = elem.XPathSelectElement("SeName").Value })
            };

            return model;
        }

        /// <summary>
        /// Prepare sorting options
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        protected virtual async Task PrepareSortingOptionsAsync(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException(nameof(pagingFilteringModel));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            //set the order by position by default
            pagingFilteringModel.OrderBy = command.OrderBy;
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

            pagingFilteringModel.AllowProductSorting = true;
            command.OrderBy = pagingFilteringModel.OrderBy ?? orderedActiveSortingOptions.FirstOrDefault().Id;

            //prepare available model sorting options
            var currentPageUrl = _webHelper.GetThisPageUrl(true);
            foreach (var option in orderedActiveSortingOptions)
            {
                pagingFilteringModel.AvailableSortOptions.Add(new SelectListItem
                {
                    Text = await _localizationService.GetLocalizedEnumAsync((ProductSortingEnum)option.Id),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "orderby", option.Id.ToString()),
                    Selected = option.Id == command.OrderBy
                });
            }
        }

        /// <summary>
        /// Prepare view modes
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        protected virtual async Task PrepareViewModesAsync(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException(nameof(pagingFilteringModel));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            pagingFilteringModel.AllowProductViewModeChanging = _catalogSettings.AllowProductViewModeChanging;

            var viewMode = !string.IsNullOrEmpty(command.ViewMode)
                ? command.ViewMode
                : _catalogSettings.DefaultViewMode;
            pagingFilteringModel.ViewMode = viewMode;
            if (pagingFilteringModel.AllowProductViewModeChanging)
            {
                var currentPageUrl = _webHelper.GetThisPageUrl(true);
                //grid
                pagingFilteringModel.AvailableViewModes.Add(new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Catalog.ViewMode.Grid"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode", "grid"),
                    Selected = viewMode == "grid"
                });
                //list
                pagingFilteringModel.AvailableViewModes.Add(new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Catalog.ViewMode.List"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode", "list"),
                    Selected = viewMode == "list"
                });
            }
        }

        /// <summary>
        /// Prepare page size options
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <param name="allowCustomersToSelectPageSize">Are customers allowed to select page size?</param>
        /// <param name="pageSizeOptions">Page size options</param>
        /// <param name="fixedPageSize">Fixed page size</param>
        protected virtual Task PreparePageSizeOptionsAsync(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command,
            bool allowCustomersToSelectPageSize, string pageSizeOptions, int fixedPageSize)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException(nameof(pagingFilteringModel));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (command.PageNumber <= 0)
                command.PageNumber = 1;

            pagingFilteringModel.AllowCustomersToSelectPageSize = false;
            if (allowCustomersToSelectPageSize && pageSizeOptions != null)
            {
                var pageSizes = pageSizeOptions.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (pageSizes.Any())
                {
                    // get the first page size entry to use as the default (category page load) or if customer enters invalid value via query string
                    if (command.PageSize <= 0 || !pageSizes.Contains(command.PageSize.ToString()))
                    {
                        if (int.TryParse(pageSizes.FirstOrDefault(), out var temp))
                        {
                            if (temp > 0)
                            {
                                command.PageSize = temp;
                            }
                        }
                    }

                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.RemoveQueryString(currentPageUrl, "pagenumber");

                    foreach (var pageSize in pageSizes)
                    {
                        if (!int.TryParse(pageSize, out var temp))
                            continue;

                        if (temp <= 0)
                            continue;

                        pagingFilteringModel.PageSizeOptions.Add(new SelectListItem
                        {
                            Text = pageSize,
                            Value = _webHelper.ModifyQueryString(sortUrl, "pagesize", pageSize),
                            Selected = pageSize.Equals(command.PageSize.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        });
                    }

                    if (pagingFilteringModel.PageSizeOptions.Any())
                    {
                        pagingFilteringModel.PageSizeOptions = pagingFilteringModel.PageSizeOptions.OrderBy(x => int.Parse(x.Text)).ToList();
                        pagingFilteringModel.AllowCustomersToSelectPageSize = true;

                        if (command.PageSize <= 0)
                        {
                            command.PageSize = int.Parse(pagingFilteringModel.PageSizeOptions.First().Text);
                        }
                    }
                }
            }
            else
            {
                //customer is not allowed to select a page size
                command.PageSize = fixedPageSize;
            }

            //ensure pge size is specified
            if (command.PageSize <= 0)
            {
                command.PageSize = fixedPageSize;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <returns>List of category (simple) models</returns>
        protected virtual async Task<List<CategorySimpleModel>> PrepareCategorySimpleModelsAsync()
        {
            //load and cache them
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoryAllModelKey,
                await _workContext.GetWorkingLanguageAsync(),
                _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()),
                await _storeContext.GetCurrentStoreAsync());

            return await _staticCacheManager.GetAsync(cacheKey, async () => await PrepareCategorySimpleModelsAsync(0));
        }

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <param name="rootCategoryId">Root category identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <returns>List of category (simple) models</returns>
        protected virtual async Task<List<CategorySimpleModel>> PrepareCategorySimpleModelsAsync(int rootCategoryId, bool loadSubCategories = true)
        {
            var result = new List<CategorySimpleModel>();

            //little hack for performance optimization
            //we know that this method is used to load top and left menu for categories.
            //it'll load all categories anyway.
            //so there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
            //so we load all categories at once (we know they are cached)
            var allCategories = await _categoryService.GetAllCategoriesAsync(storeId: (await _storeContext.GetCurrentStoreAsync()).Id);
            var categories = allCategories.Where(c => c.ParentCategoryId == rootCategoryId).OrderBy(c => c.DisplayOrder).ToList();
            foreach (var category in categories)
            {
                var categoryModel = new CategorySimpleModel
                {
                    Id = category.Id,
                    Name = await _localizationService.GetLocalizedAsync(category, x => x.Name),
                    SeName = await _urlRecordService.GetSeNameAsync(category),
                    IncludeInTopMenu = category.IncludeInTopMenu
                };

                //number of products in each category
                if (_catalogSettings.ShowCategoryProductNumber)
                {
                    var categoryIds = new List<int> { category.Id };
                    //include subcategories
                    if (_catalogSettings.ShowCategoryProductNumberIncludingSubcategories)
                        categoryIds.AddRange(
                            await _categoryService.GetChildCategoryIdsAsync(category.Id, (await _storeContext.GetCurrentStoreAsync()).Id));

                    categoryModel.NumberOfProducts =
                        await _productService.GetNumberOfProductsInCategoryAsync(categoryIds, (await _storeContext.GetCurrentStoreAsync()).Id);
                }

                if (loadSubCategories)
                {
                    var subCategories = await PrepareCategorySimpleModelsAsync(category.Id);
                    categoryModel.SubCategories.AddRange(subCategories);
                }

                categoryModel.HaveSubCategories = categoryModel.SubCategories.Count > 0 &
                    categoryModel.SubCategories.Any(x => x.IncludeInTopMenu);

                result.Add(categoryModel);
            }

            return result;
        }

        /// <summary>
        /// Prepare category (simple) xml document
        /// </summary>
        /// <returns>Xml document of category (simple) models</returns>
        protected virtual async Task<XDocument> PrepareCategoryXmlDocumentAsync()
        {
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoryXmlAllModelKey,
                await _workContext.GetWorkingLanguageAsync(),
                _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()),
                await _storeContext.GetCurrentStoreAsync());

            return await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var categories = await PrepareCategorySimpleModelsAsync();

                var xsSubmit = new XmlSerializer(typeof(List<CategorySimpleModel>));

                await using var strWriter = new StringWriter();
                using var writer = XmlWriter.Create(strWriter);
                xsSubmit.Serialize(writer, categories);
                var xml = strWriter.ToString();

                return XDocument.Parse(xml);
            });
        }

        #endregion

        #region Categories

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Category model</returns>
        public virtual async Task<CategoryModel> PrepareCategoryModelAsync(Category category, CatalogPagingFilteringModel command)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var model = new CategoryModel
            {
                Id = category.Id,
                Name = await _localizationService.GetLocalizedAsync(category, x => x.Name),
                Description = await _localizationService.GetLocalizedAsync(category, x => x.Description),
                MetaKeywords = await _localizationService.GetLocalizedAsync(category, x => x.MetaKeywords),
                MetaDescription = await _localizationService.GetLocalizedAsync(category, x => x.MetaDescription),
                MetaTitle = await _localizationService.GetLocalizedAsync(category, x => x.MetaTitle),
                SeName = await _urlRecordService.GetSeNameAsync(category),
            };

            //sorting
            await PrepareSortingOptionsAsync(model.PagingFilteringContext, command);
            //view mode
            await PrepareViewModesAsync(model.PagingFilteringContext, command);
            //page size
            await PreparePageSizeOptionsAsync(model.PagingFilteringContext, command,
                category.AllowCustomersToSelectPageSize,
                category.PageSizeOptions,
                category.PageSize);

            //price ranges
            await model.PagingFilteringContext.PriceRangeFilter.LoadPriceRangeFiltersAsync(category.PriceRanges, _webHelper, _priceFormatter);
            var selectedPriceRange = await model.PagingFilteringContext.PriceRangeFilter.GetSelectedPriceRangeAsync(_webHelper, category.PriceRanges);
            decimal? minPriceConverted = null;
            decimal? maxPriceConverted = null;
            if (selectedPriceRange != null)
            {
                if (selectedPriceRange.From.HasValue)
                    minPriceConverted = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(selectedPriceRange.From.Value, await _workContext.GetWorkingCurrencyAsync());

                if (selectedPriceRange.To.HasValue)
                    maxPriceConverted = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(selectedPriceRange.To.Value, await _workContext.GetWorkingCurrencyAsync());
            }

            //category breadcrumb
            if (_catalogSettings.CategoryBreadcrumbEnabled)
            {
                model.DisplayCategoryBreadcrumb = true;

                model.CategoryBreadcrumb = await (await _categoryService.GetCategoryBreadCrumbAsync(category)).SelectAwait(async catBr =>
                    new CategoryModel
                    {
                        Id = catBr.Id,
                        Name = await _localizationService.GetLocalizedAsync(catBr, x => x.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(catBr)
                    }).ToListAsync();
            }

            var pictureSize = _mediaSettings.CategoryThumbPictureSize;

            //subcategories
            model.SubCategories = await (await _categoryService.GetAllCategoriesByParentCategoryIdAsync(category.Id))
                .SelectAwait(async curCategory =>
                {
                    var subCatModel = new CategoryModel.SubCategoryModel
                    {
                        Id = curCategory.Id,
                        Name = await _localizationService.GetLocalizedAsync(curCategory, y => y.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(curCategory),
                        Description = await _localizationService.GetLocalizedAsync(curCategory, y => y.Description)
                    };

                    //prepare picture model
                    var categoryPictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoryPictureModelKey, curCategory,
                        pictureSize, true, await _workContext.GetWorkingLanguageAsync(), _webHelper.IsCurrentConnectionSecured(),
                        await _storeContext.GetCurrentStoreAsync());

                    subCatModel.PictureModel = await _staticCacheManager.GetAsync(categoryPictureCacheKey, async () =>
                    {
                        var picture = await _pictureService.GetPictureByIdAsync(curCategory.PictureId);
                        string fullSizeImageUrl, imageUrl;

                        (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                        (imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);

                        var pictureModel = new PictureModel
                        {
                            FullSizeImageUrl = fullSizeImageUrl,
                            ImageUrl = imageUrl,
                            Title = string.Format(await _localizationService
                                .GetResourceAsync("Media.Category.ImageLinkTitleFormat"), subCatModel.Name),
                            AlternateText = string.Format(await _localizationService
                                .GetResourceAsync("Media.Category.ImageAlternateTextFormat"), subCatModel.Name)
                        };

                        return pictureModel;
                    });

                    return subCatModel;
                }).ToListAsync();

            //featured products
            if (!_catalogSettings.IgnoreFeaturedProducts)
            {
                var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
                var featuredProducts = await _productService.GetCategoryFeaturedProductsAsync(category.Id, storeId);
                if (featuredProducts != null)
                    model.FeaturedProducts = (await _productModelFactory.PrepareProductOverviewModelsAsync(featuredProducts)).ToList();
            }

            var categoryIds = new List<int> { category.Id };

            //include subcategories
            if (_catalogSettings.ShowProductsFromSubcategories)
                categoryIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(category.Id, (await _storeContext.GetCurrentStoreAsync()).Id));

            //products
            IList<int> alreadyFilteredSpecOptionIds = await model.PagingFilteringContext.SpecificationFilter.GetAlreadyFilteredSpecOptionIdsAsync(_webHelper);
            var (products, filterableSpecificationAttributeOptionIds) = await _productService.SearchProductsAsync(true,
                categoryIds: categoryIds,
                storeId: (await _storeContext.GetCurrentStoreAsync()).Id,
                visibleIndividuallyOnly: true,
                excludeFeaturedProducts: !_catalogSettings.IgnoreFeaturedProducts && !_catalogSettings.IncludeFeaturedProductsInNormalLists,
                priceMin: minPriceConverted,
                priceMax: maxPriceConverted,
                filteredSpecs: alreadyFilteredSpecOptionIds,
                orderBy: (ProductSortingEnum)command.OrderBy,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);
            model.Products = (await _productModelFactory.PrepareProductOverviewModelsAsync(products)).ToList();

            model.PagingFilteringContext.LoadPagedList(products);

            //specs
            await model.PagingFilteringContext.SpecificationFilter.PrepareSpecsFiltersAsync(alreadyFilteredSpecOptionIds,
                filterableSpecificationAttributeOptionIds?.ToArray(), _specificationAttributeService, _localizationService, _webHelper, _workContext, _staticCacheManager);

            return model;
        }

        /// <summary>
        /// Prepare category template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>Category template view path</returns>
        public virtual async Task<string> PrepareCategoryTemplateViewPathAsync(int templateId)
        {
            var template = await _categoryTemplateService.GetCategoryTemplateByIdAsync(templateId) ??
                           (await _categoryTemplateService.GetAllCategoryTemplatesAsync()).FirstOrDefault();

            if (template == null)
                throw new Exception("No default template could be loaded");

            return template.ViewPath;
        }

        /// <summary>
        /// Prepare category navigation model
        /// </summary>
        /// <param name="currentCategoryId">Current category identifier</param>
        /// <param name="currentProductId">Current product identifier</param>
        /// <returns>Category navigation model</returns>
        public virtual async Task<CategoryNavigationModel> PrepareCategoryNavigationModelAsync(int currentCategoryId, int currentProductId)
        {
            //get active category
            var activeCategoryId = 0;
            if (currentCategoryId > 0)
            {
                //category details page
                activeCategoryId = currentCategoryId;
            }
            else if (currentProductId > 0)
            {
                //product details page
                var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(currentProductId);
                if (productCategories.Any())
                    activeCategoryId = productCategories[0].CategoryId;
            }

            var cachedCategoriesModel = await PrepareCategorySimpleModelsAsync();
            var model = new CategoryNavigationModel
            {
                CurrentCategoryId = activeCategoryId,
                Categories = cachedCategoriesModel
            };

            return model;
        }

        /// <summary>
        /// Prepare top menu model
        /// </summary>
        /// <returns>Top menu model</returns>
        public virtual async Task<TopMenuModel> PrepareTopMenuModelAsync()
        {
            var cachedCategoriesModel = new List<CategorySimpleModel>();
            //categories
            if (!_catalogSettings.UseAjaxLoadMenu)
                cachedCategoriesModel = await PrepareCategorySimpleModelsAsync();

            //top menu topics
            var topicModel = await (await _topicService.GetAllTopicsAsync((await _storeContext.GetCurrentStoreAsync()).Id, onlyIncludedInTopMenu: true))
                .SelectAwait(async t => new TopMenuModel.TopicModel
                    {
                        Id = t.Id,
                        Name = await _localizationService.GetLocalizedAsync(t, x => x.Title),
                        SeName = await _urlRecordService.GetSeNameAsync(t)
                    }).ToListAsync();

            var model = new TopMenuModel
            {
                Categories = cachedCategoriesModel,
                Topics = topicModel,
                NewProductsEnabled = _catalogSettings.NewProductsEnabled,
                BlogEnabled = _blogSettings.Enabled,
                ForumEnabled = _forumSettings.ForumsEnabled,
                DisplayHomepageMenuItem = _displayDefaultMenuItemSettings.DisplayHomepageMenuItem,
                DisplayNewProductsMenuItem = _displayDefaultMenuItemSettings.DisplayNewProductsMenuItem,
                DisplayProductSearchMenuItem = _displayDefaultMenuItemSettings.DisplayProductSearchMenuItem,
                DisplayCustomerInfoMenuItem = _displayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem,
                DisplayBlogMenuItem = _displayDefaultMenuItemSettings.DisplayBlogMenuItem,
                DisplayForumsMenuItem = _displayDefaultMenuItemSettings.DisplayForumsMenuItem,
                DisplayContactUsMenuItem = _displayDefaultMenuItemSettings.DisplayContactUsMenuItem,
                UseAjaxMenu = _catalogSettings.UseAjaxLoadMenu
            };

            return model;
        }

        /// <summary>
        /// Prepare homepage category models
        /// </summary>
        /// <returns>List of homepage category models</returns>
        public virtual async Task<List<CategoryModel>> PrepareHomepageCategoryModelsAsync()
        {
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;

            var categoriesCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoryHomepageKey,
                pictureSize,
                await _workContext.GetWorkingLanguageAsync(),
                _webHelper.IsCurrentConnectionSecured());

            var model = await _staticCacheManager.GetAsync(categoriesCacheKey, async () =>
                await (await _categoryService.GetAllCategoriesDisplayedOnHomepageAsync())
                .SelectAwait(async category =>
                    {
                        var catModel = new CategoryModel
                        {
                            Id = category.Id,
                            Name = await _localizationService.GetLocalizedAsync(category, x => x.Name),
                            Description = await _localizationService.GetLocalizedAsync(category, x => x.Description),
                            MetaKeywords = await _localizationService.GetLocalizedAsync(category, x => x.MetaKeywords),
                            MetaDescription = await _localizationService.GetLocalizedAsync(category, x => x.MetaDescription),
                            MetaTitle = await _localizationService.GetLocalizedAsync(category, x => x.MetaTitle),
                            SeName = await _urlRecordService.GetSeNameAsync(category),
                        };

                        //prepare picture model
                        var categoryPictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoryPictureModelKey,
                            category, pictureSize, true, await _workContext.GetWorkingLanguageAsync(),
                            _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());
                        catModel.PictureModel = await _staticCacheManager.GetAsync(categoryPictureCacheKey, async () =>
                        {
                            var picture = await _pictureService.GetPictureByIdAsync(category.PictureId);
                            string fullSizeImageUrl, imageUrl;

                            (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                            (imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);

                            var pictureModel = new PictureModel
                            {
                                FullSizeImageUrl = fullSizeImageUrl,
                                ImageUrl = imageUrl,
                                Title = string.Format(
                                    await _localizationService.GetResourceAsync("Media.Category.ImageLinkTitleFormat"),
                                    catModel.Name),
                                AlternateText =
                                    string.Format(
                                        await _localizationService.GetResourceAsync("Media.Category.ImageAlternateTextFormat"),
                                        catModel.Name)
                            };
                            return pictureModel;
                        });

                        return catModel;
                    }).ToListAsync());

            return model;
        }
        
        /// <summary>
        /// Prepare root categories for menu
        /// </summary>
        /// <returns>List of category (simple) models</returns>
        public virtual async Task<List<CategorySimpleModel>> PrepareRootCategoriesAsync()
        {
            var doc = await PrepareCategoryXmlDocumentAsync();

            var models = from xe in doc.Root.XPathSelectElements("CategorySimpleModel")
                         select GetCategorySimpleModel(xe);

            return models.ToList();
        }

        /// <summary>
        /// Prepare subcategories for menu
        /// </summary>
        /// <param name="id">Id of category to get subcategory</param>
        /// <returns></returns>
        public virtual async Task<List<CategorySimpleModel>> PrepareSubCategoriesAsync(int id)
        {
            var doc = await PrepareCategoryXmlDocumentAsync();

            var model = from xe in doc.Descendants("CategorySimpleModel")
                        where xe.XPathSelectElement("Id").Value == id.ToString()
                        select xe;

            var models = from xe in model.First().XPathSelectElements("SubCategories/CategorySimpleModel")
                         select GetCategorySimpleModel(xe);

            return models.ToList();
        }

        #endregion

        #region Manufacturers

        /// <summary>
        /// Prepare manufacturer model
        /// </summary>
        /// <param name="manufacturer">Manufacturer identifier</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Manufacturer model</returns>
        public virtual async Task<ManufacturerModel> PrepareManufacturerModelAsync(Manufacturer manufacturer, CatalogPagingFilteringModel command)
        {
            if (manufacturer == null)
                throw new ArgumentNullException(nameof(manufacturer));

            var model = new ManufacturerModel
            {
                Id = manufacturer.Id,
                Name = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name),
                Description = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Description),
                MetaKeywords = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaKeywords),
                MetaDescription = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaDescription),
                MetaTitle = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaTitle),
                SeName = await _urlRecordService.GetSeNameAsync(manufacturer),
            };

            //sorting
            await PrepareSortingOptionsAsync(model.PagingFilteringContext, command);
            //view mode
            await PrepareViewModesAsync(model.PagingFilteringContext, command);
            //page size
            await PreparePageSizeOptionsAsync(model.PagingFilteringContext, command,
                manufacturer.AllowCustomersToSelectPageSize,
                manufacturer.PageSizeOptions,
                manufacturer.PageSize);

            //price ranges
            await model.PagingFilteringContext.PriceRangeFilter.LoadPriceRangeFiltersAsync(manufacturer.PriceRanges, _webHelper, _priceFormatter);
            var selectedPriceRange = await model.PagingFilteringContext.PriceRangeFilter.GetSelectedPriceRangeAsync(_webHelper, manufacturer.PriceRanges);
            decimal? minPriceConverted = null;
            decimal? maxPriceConverted = null;
            if (selectedPriceRange != null)
            {
                if (selectedPriceRange.From.HasValue)
                    minPriceConverted = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(selectedPriceRange.From.Value, await _workContext.GetWorkingCurrencyAsync());

                if (selectedPriceRange.To.HasValue)
                    maxPriceConverted = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(selectedPriceRange.To.Value, await _workContext.GetWorkingCurrencyAsync());
            }

            //featured products
            if (!_catalogSettings.IgnoreFeaturedProducts)
            {
                var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
                var featuredProducts = await _productService.GetManufacturerFeaturedProductsAsync(manufacturer.Id, storeId);
                if (featuredProducts != null)
                    model.FeaturedProducts = (await _productModelFactory.PrepareProductOverviewModelsAsync(featuredProducts)).ToList();
            }

            //products
            var products = await _productService.SearchProductsAsync(command.PageNumber - 1, command.PageSize,
                manufacturerId: manufacturer.Id,
                storeId: (await _storeContext.GetCurrentStoreAsync()).Id,
                visibleIndividuallyOnly: true,
                excludeFeaturedProducts: !_catalogSettings.IgnoreFeaturedProducts && !_catalogSettings.IncludeFeaturedProductsInNormalLists,
                priceMin: minPriceConverted,
                priceMax: maxPriceConverted,
                orderBy: (ProductSortingEnum)command.OrderBy);
            model.Products = (await _productModelFactory.PrepareProductOverviewModelsAsync(products)).ToList();

            model.PagingFilteringContext.LoadPagedList(products);

            return model;
        }

        /// <summary>
        /// Prepare manufacturer template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>Manufacturer template view path</returns>
        public virtual async Task<string> PrepareManufacturerTemplateViewPathAsync(int templateId)
        {
            var template = await _manufacturerTemplateService.GetManufacturerTemplateByIdAsync(templateId) ??
                           (await _manufacturerTemplateService.GetAllManufacturerTemplatesAsync()).FirstOrDefault();

            if (template == null)
                throw new Exception("No default template could be loaded");

            return template.ViewPath;
        }

        /// <summary>
        /// Prepare manufacturer all models
        /// </summary>
        /// <returns>List of manufacturer models</returns>
        public virtual async Task<List<ManufacturerModel>> PrepareManufacturerAllModelsAsync()
        {
            var model = new List<ManufacturerModel>();
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(storeId: (await _storeContext.GetCurrentStoreAsync()).Id);
            foreach (var manufacturer in manufacturers)
            {
                var modelMan = new ManufacturerModel
                {
                    Id = manufacturer.Id,
                    Name = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name),
                    Description = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Description),
                    MetaKeywords = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaKeywords),
                    MetaDescription = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaDescription),
                    MetaTitle = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaTitle),
                    SeName = await _urlRecordService.GetSeNameAsync(manufacturer),
                };

                //prepare picture model
                var pictureSize = _mediaSettings.ManufacturerThumbPictureSize;
                var manufacturerPictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ManufacturerPictureModelKey,
                    manufacturer, pictureSize, true, await _workContext.GetWorkingLanguageAsync(),
                    _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());
                modelMan.PictureModel = await _staticCacheManager.GetAsync(manufacturerPictureCacheKey, async () =>
                {
                    var picture = await _pictureService.GetPictureByIdAsync(manufacturer.PictureId);
                    string fullSizeImageUrl, imageUrl;

                    (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                    (imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);

                    var pictureModel = new PictureModel
                    {
                        FullSizeImageUrl = fullSizeImageUrl,
                        ImageUrl = imageUrl,
                        Title = string.Format(await _localizationService.GetResourceAsync("Media.Manufacturer.ImageLinkTitleFormat"), modelMan.Name),
                        AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Manufacturer.ImageAlternateTextFormat"), modelMan.Name)
                    };

                    return pictureModel;
                });

                model.Add(modelMan);
            }

            return model;
        }

        /// <summary>
        /// Prepare manufacturer navigation model
        /// </summary>
        /// <param name="currentManufacturerId">Current manufacturer identifier</param>
        /// <returns>Manufacturer navigation model</returns>
        public virtual async Task<ManufacturerNavigationModel> PrepareManufacturerNavigationModelAsync(int currentManufacturerId)
        {
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ManufacturerNavigationModelKey,
                currentManufacturerId,
                await _workContext.GetWorkingLanguageAsync(),
                await _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()),
                await _storeContext.GetCurrentStoreAsync());
            var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var currentManufacturer = await _manufacturerService.GetManufacturerByIdAsync(currentManufacturerId);

                var manufacturers = await _manufacturerService.GetAllManufacturersAsync(storeId: (await _storeContext.GetCurrentStoreAsync()).Id,
                    pageSize: _catalogSettings.ManufacturersBlockItemsToDisplay);
                var model = new ManufacturerNavigationModel
                {
                    TotalManufacturers = manufacturers.TotalCount
                };

                foreach (var manufacturer in manufacturers)
                {
                    var modelMan = new ManufacturerBriefInfoModel
                    {
                        Id = manufacturer.Id,
                        Name = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(manufacturer),
                        IsActive = currentManufacturer != null && currentManufacturer.Id == manufacturer.Id,
                    };
                    model.Manufacturers.Add(modelMan);
                }

                return model;
            });

            return cachedModel;
        }

        #endregion

        #region Vendors

        /// <summary>
        /// Prepare vendor model
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Vendor model</returns>
        public virtual async Task<VendorModel> PrepareVendorModelAsync(Vendor vendor, CatalogPagingFilteringModel command)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            var model = new VendorModel
            {
                Id = vendor.Id,
                Name = await _localizationService.GetLocalizedAsync(vendor, x => x.Name),
                Description = await _localizationService.GetLocalizedAsync(vendor, x => x.Description),
                MetaKeywords = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaKeywords),
                MetaDescription = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaDescription),
                MetaTitle = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaTitle),
                SeName = await _urlRecordService.GetSeNameAsync(vendor),
                AllowCustomersToContactVendors = _vendorSettings.AllowCustomersToContactVendors
            };

            //sorting
            await PrepareSortingOptionsAsync(model.PagingFilteringContext, command);
            //view mode
            await PrepareViewModesAsync(model.PagingFilteringContext, command);
            //page size
            await PreparePageSizeOptionsAsync(model.PagingFilteringContext, command,
                vendor.AllowCustomersToSelectPageSize,
                vendor.PageSizeOptions,
                vendor.PageSize);

            //products
            var products = await _productService.SearchProductsAsync(command.PageNumber - 1, command.PageSize,
                vendorId: vendor.Id,
                storeId: (await _storeContext.GetCurrentStoreAsync()).Id,
                visibleIndividuallyOnly: true,
                orderBy: (ProductSortingEnum)command.OrderBy);
            model.Products = (await _productModelFactory.PrepareProductOverviewModelsAsync(products)).ToList();

            model.PagingFilteringContext.LoadPagedList(products);

            return model;
        }

        /// <summary>
        /// Prepare vendor all models
        /// </summary>
        /// <returns>List of vendor models</returns>
        public virtual async Task<List<VendorModel>> PrepareVendorAllModelsAsync()
        {
            var model = new List<VendorModel>();
            var vendors = await _vendorService.GetAllVendorsAsync();
            foreach (var vendor in vendors)
            {
                var vendorModel = new VendorModel
                {
                    Id = vendor.Id,
                    Name = await _localizationService.GetLocalizedAsync(vendor, x => x.Name),
                    Description = await _localizationService.GetLocalizedAsync(vendor, x => x.Description),
                    MetaKeywords = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaKeywords),
                    MetaDescription = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaDescription),
                    MetaTitle = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaTitle),
                    SeName = await _urlRecordService.GetSeNameAsync(vendor),
                    AllowCustomersToContactVendors = _vendorSettings.AllowCustomersToContactVendors
                };

                //prepare picture model
                var pictureSize = _mediaSettings.VendorThumbPictureSize;
                var pictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.VendorPictureModelKey,
                    vendor, pictureSize, true, await _workContext.GetWorkingLanguageAsync(), _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());
                vendorModel.PictureModel = await _staticCacheManager.GetAsync(pictureCacheKey, async () =>
                {
                    var picture = await _pictureService.GetPictureByIdAsync(vendor.PictureId);
                    string fullSizeImageUrl, imageUrl;

                    (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                    (imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);

                    var pictureModel = new PictureModel
                    {
                        FullSizeImageUrl = fullSizeImageUrl,
                        ImageUrl = imageUrl,
                        Title = string.Format(await _localizationService.GetResourceAsync("Media.Vendor.ImageLinkTitleFormat"), vendorModel.Name),
                        AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Vendor.ImageAlternateTextFormat"), vendorModel.Name)
                    };

                    return pictureModel;
                });

                model.Add(vendorModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare vendor navigation model
        /// </summary>
        /// <returns>Vendor navigation model</returns>
        public virtual async Task<VendorNavigationModel> PrepareVendorNavigationModelAsync()
        {
            var cacheKey = NopModelCacheDefaults.VendorNavigationModelKey;
            var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var vendors = await _vendorService.GetAllVendorsAsync(pageSize: _vendorSettings.VendorsBlockItemsToDisplay);
                var model = new VendorNavigationModel
                {
                    TotalVendors = vendors.TotalCount
                };

                foreach (var vendor in vendors)
                {
                    model.Vendors.Add(new VendorBriefInfoModel
                    {
                        Id = vendor.Id,
                        Name = await _localizationService.GetLocalizedAsync(vendor, x => x.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(vendor),
                    });
                }

                return model;
            });

            return cachedModel;
        }

        #endregion

        #region Product tags

        /// <summary>
        /// Prepare popular product tags model
        /// </summary>
        /// <returns>Product tags model</returns>
        public virtual async Task<PopularProductTagsModel> PreparePopularProductTagsModelAsync()
        {
            var model = new PopularProductTagsModel();

            //get all tags
            var tags = await (await _productTagService.GetAllProductTagsAsync())
                //filter by current store
                .WhereAwait(async x => await _productTagService.GetProductCountAsync(x.Id, (await _storeContext.GetCurrentStoreAsync()).Id) > 0)
                .ToListAsync();

            model.TotalTags = tags.Count;

            model.Tags.AddRange(await tags
                //order by product count
                .OrderByDescendingAwait(async x => await _productTagService.GetProductCountAsync(x.Id, (await _storeContext.GetCurrentStoreAsync()).Id))
                .Take(_catalogSettings.NumberOfProductTags)
                //sorting
                .OrderByAwait(async x => await _localizationService.GetLocalizedAsync(x, y => y.Name))
                .SelectAwait(async tag => new ProductTagModel
                {
                    Id = tag.Id,
                    Name = await _localizationService.GetLocalizedAsync(tag, y => y.Name),
                    SeName = await _urlRecordService.GetSeNameAsync(tag),
                    ProductCount = await _productTagService.GetProductCountAsync(tag.Id, (await _storeContext.GetCurrentStoreAsync()).Id)
                }).ToListAsync());

            return model;
        }

        /// <summary>
        /// Prepare products by tag model
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Products by tag model</returns>
        public virtual async Task<ProductsByTagModel> PrepareProductsByTagModelAsync(ProductTag productTag, CatalogPagingFilteringModel command)
        {
            if (productTag == null)
                throw new ArgumentNullException(nameof(productTag));

            var model = new ProductsByTagModel
            {
                Id = productTag.Id,
                TagName = await _localizationService.GetLocalizedAsync(productTag, y => y.Name),
                TagSeName = await _urlRecordService.GetSeNameAsync(productTag)
            };

            //sorting
            await PrepareSortingOptionsAsync(model.PagingFilteringContext, command);
            //view mode
            await PrepareViewModesAsync(model.PagingFilteringContext, command);
            //page size
            await PreparePageSizeOptionsAsync(model.PagingFilteringContext, command,
                _catalogSettings.ProductsByTagAllowCustomersToSelectPageSize,
                _catalogSettings.ProductsByTagPageSizeOptions,
                _catalogSettings.ProductsByTagPageSize);

            //products
            var (products, _) = await _productService.SearchProductsAsync(false,
                storeId: (await _storeContext.GetCurrentStoreAsync()).Id,
                productTagId: productTag.Id,
                visibleIndividuallyOnly: true,
                orderBy: (ProductSortingEnum)command.OrderBy,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);
            model.Products = (await _productModelFactory.PrepareProductOverviewModelsAsync(products)).ToList();

            model.PagingFilteringContext.LoadPagedList(products);
            return model;
        }

        /// <summary>
        /// Prepare product tags all model
        /// </summary>
        /// <returns>Popular product tags model</returns>
        public virtual async Task<PopularProductTagsModel> PrepareProductTagsAllModelAsync()
        {
            var model = new PopularProductTagsModel
            {
                Tags = await (await _productTagService.GetAllProductTagsAsync())
                //filter by current store
                .WhereAwait(async x => await _productTagService.GetProductCountAsync(x.Id, (await _storeContext.GetCurrentStoreAsync()).Id) > 0)
                //sort by name
                .OrderByAwait(async x => await _localizationService.GetLocalizedAsync(x, y => y.Name))
                .SelectAwait(async x =>
                {
                    var ptModel = new ProductTagModel
                    {
                        Id = x.Id,
                        Name = await _localizationService.GetLocalizedAsync(x, y => y.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(x),
                        ProductCount = await _productTagService.GetProductCountAsync(x.Id, (await _storeContext.GetCurrentStoreAsync()).Id)
                    };
                    return ptModel;
                })
                .ToListAsync()
            };
            return model;
        }

        #endregion

        #region Searching

        /// <summary>
        /// Prepare search model
        /// </summary>
        /// <param name="model">Search model</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Search model</returns>
        public virtual async Task<SearchModel> PrepareSearchModelAsync(SearchModel model, CatalogPagingFilteringModel command)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var searchTerms = model.q ?? string.Empty;

            searchTerms = searchTerms.Trim();

            //sorting
            await PrepareSortingOptionsAsync(model.PagingFilteringContext, command);
            //view mode
            await PrepareViewModesAsync(model.PagingFilteringContext, command);
            //page size
            await PreparePageSizeOptionsAsync(model.PagingFilteringContext, command,
                _catalogSettings.SearchPageAllowCustomersToSelectPageSize,
                _catalogSettings.SearchPagePageSizeOptions,
                _catalogSettings.SearchPageProductsPerPage);


            var categoriesModels = new List<SearchModel.CategoryModel>();
            //all categories
            var allCategories = await _categoryService.GetAllCategoriesAsync(storeId: (await _storeContext.GetCurrentStoreAsync()).Id);
            foreach (var c in allCategories)
            {
                //generate full category name (breadcrumb)
                var categoryBreadcrumb = string.Empty;
                var breadcrumb = await _categoryService.GetCategoryBreadCrumbAsync(c, allCategories);
                for (var i = 0; i <= breadcrumb.Count - 1; i++)
                {
                    categoryBreadcrumb += await _localizationService.GetLocalizedAsync(breadcrumb[i], x => x.Name);
                    if (i != breadcrumb.Count - 1)
                        categoryBreadcrumb += " >> ";
                }

                categoriesModels.Add(new SearchModel.CategoryModel
                {
                    Id = c.Id,
                    Breadcrumb = categoryBreadcrumb
                });
            }

            if (categoriesModels.Any())
            {
                //first empty entry
                model.AvailableCategories.Add(new SelectListItem
                {
                    Value = "0",
                    Text = await _localizationService.GetResourceAsync("Common.All")
                });
                //all other categories
                foreach (var c in categoriesModels)
                {
                    model.AvailableCategories.Add(new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Breadcrumb,
                        Selected = model.cid == c.Id
                    });
                }
            }

            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(storeId: (await _storeContext.GetCurrentStoreAsync()).Id);
            if (manufacturers.Any())
            {
                model.AvailableManufacturers.Add(new SelectListItem
                {
                    Value = "0",
                    Text = await _localizationService.GetResourceAsync("Common.All")
                });
                foreach (var m in manufacturers)
                    model.AvailableManufacturers.Add(new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = await _localizationService.GetLocalizedAsync(m, x => x.Name),
                        Selected = model.mid == m.Id
                    });
            }

            model.asv = _vendorSettings.AllowSearchByVendor;
            if (model.asv)
            {
                var vendors = await _vendorService.GetAllVendorsAsync();
                if (vendors.Any())
                {
                    model.AvailableVendors.Add(new SelectListItem
                    {
                        Value = "0",
                        Text = await _localizationService.GetResourceAsync("Common.All")
                    });
                    foreach (var vendor in vendors)
                        model.AvailableVendors.Add(new SelectListItem
                        {
                            Value = vendor.Id.ToString(),
                            Text = await _localizationService.GetLocalizedAsync(vendor, x => x.Name),
                            Selected = model.vid == vendor.Id
                        });
                }
            }

            IPagedList<Product> products = new PagedList<Product>(new List<Product>(), 0, 1);
            // only search if query string search keyword is set (used to aasync Task searching or displaying search term min length error message on /search page load)
            //we don't use "!string.IsNullOrEmpty(searchTerms)" in cases of "ProductSearchTermMinimumLength" set to 0 but searching by other parameters (e.g. category or price filter)
            var isSearchTermSpecified = _httpContextAccessor.HttpContext.Request.Query.ContainsKey("q");
            if (isSearchTermSpecified)
            {
                if (searchTerms.Length < _catalogSettings.ProductSearchTermMinimumLength)
                {
                    model.Warning =
                        string.Format(await _localizationService.GetResourceAsync("Search.SearchTermMinimumLengthIsNCharacters"),
                            _catalogSettings.ProductSearchTermMinimumLength);
                }
                else
                {
                    var categoryIds = new List<int>();
                    var manufacturerId = 0;
                    decimal? minPriceConverted = null;
                    decimal? maxPriceConverted = null;
                    var searchInDescriptions = false;
                    var vendorId = 0;
                    if (model.adv)
                    {
                        //advanced search
                        var categoryId = model.cid;
                        if (categoryId > 0)
                        {
                            categoryIds.Add(categoryId);
                            if (model.isc)
                            {
                                //include subcategories
                                categoryIds.AddRange(
                                    await _categoryService.GetChildCategoryIdsAsync(categoryId, (await _storeContext.GetCurrentStoreAsync()).Id));
                            }
                        }

                        manufacturerId = model.mid;

                        //min price
                        if (!string.IsNullOrEmpty(model.pf))
                        {
                            if (decimal.TryParse(model.pf, out var minPrice))
                                minPriceConverted =
                                    await _currencyService.ConvertToPrimaryStoreCurrencyAsync(minPrice,
                                        await _workContext.GetWorkingCurrencyAsync());
                        }

                        //max price
                        if (!string.IsNullOrEmpty(model.pt))
                        {
                            if (decimal.TryParse(model.pt, out var maxPrice))
                                maxPriceConverted =
                                    await _currencyService.ConvertToPrimaryStoreCurrencyAsync(maxPrice,
                                        await _workContext.GetWorkingCurrencyAsync());
                        }

                        if (model.asv)
                            vendorId = model.vid;

                        searchInDescriptions = model.sid;
                    }

                    //var searchInProductTags = false;
                    var searchInProductTags = searchInDescriptions;

                    //products
                    (products, _) = await _productService.SearchProductsAsync(false,
                        categoryIds: categoryIds,
                        manufacturerId: manufacturerId,
                        storeId: (await _storeContext.GetCurrentStoreAsync()).Id,
                        visibleIndividuallyOnly: true,
                        priceMin: minPriceConverted,
                        priceMax: maxPriceConverted,
                        keywords: searchTerms,
                        searchDescriptions: searchInDescriptions,
                        searchProductTags: searchInProductTags,
                        languageId: (await _workContext.GetWorkingLanguageAsync()).Id,
                        orderBy: (ProductSortingEnum)command.OrderBy,
                        pageIndex: command.PageNumber - 1,
                        pageSize: command.PageSize,
                        vendorId: vendorId);
                    model.Products = (await _productModelFactory.PrepareProductOverviewModelsAsync(products)).ToList();

                    model.NoResults = !model.Products.Any();

                    //search term statistics
                    if (!string.IsNullOrEmpty(searchTerms))
                    {
                        var searchTerm =
                            await _searchTermService.GetSearchTermByKeywordAsync(searchTerms, (await _storeContext.GetCurrentStoreAsync()).Id);
                        if (searchTerm != null)
                        {
                            searchTerm.Count++;
                            await _searchTermService.UpdateSearchTermAsync(searchTerm);
                        }
                        else
                        {
                            searchTerm = new SearchTerm
                            {
                                Keyword = searchTerms,
                                StoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
                                Count = 1
                            };
                            await _searchTermService.InsertSearchTermAsync(searchTerm);
                        }
                    }

                    //event
                    await _eventPublisher.PublishAsync(new ProductSearchEvent
                    {
                        SearchTerm = searchTerms,
                        SearchInDescriptions = searchInDescriptions,
                        CategoryIds = categoryIds,
                        ManufacturerId = manufacturerId,
                        WorkingLanguageId = (await _workContext.GetWorkingLanguageAsync()).Id,
                        VendorId = vendorId
                    });
                }
            }

            model.PagingFilteringContext.LoadPagedList(products);
            return model;
        }

        /// <summary>
        /// Prepare search box model
        /// </summary>
        /// <returns>Search box model</returns>
        public virtual Task<SearchBoxModel> PrepareSearchBoxModelAsync()
        {
            var model = new SearchBoxModel
            {
                AutoCompleteEnabled = _catalogSettings.ProductSearchAutoCompleteEnabled,
                ShowProductImagesInSearchAutoComplete = _catalogSettings.ShowProductImagesInSearchAutoComplete,
                SearchTermMinimumLength = _catalogSettings.ProductSearchTermMinimumLength,
                ShowSearchBox = _catalogSettings.ProductSearchEnabled
            };

            return Task.FromResult(model);
        }

        #endregion
    }
}