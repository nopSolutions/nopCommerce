using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Seo;
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
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;

namespace Nop.Web.Factories
{
    public partial class CatalogModelFactory : ICatalogModelFactory
    {
        #region Fields

        protected readonly BlogSettings _blogSettings;
        protected readonly CatalogSettings _catalogSettings;
        protected readonly DisplayDefaultMenuItemSettings _displayDefaultMenuItemSettings;
        protected readonly ForumSettings _forumSettings;
        protected readonly ICategoryService _categoryService;
        protected readonly ICategoryTemplateService _categoryTemplateService;
        protected readonly ICurrencyService _currencyService;
        protected readonly ICustomerService _customerService;
        protected readonly IEventPublisher _eventPublisher;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IJsonLdModelFactory _jsonLdModelFactory;
        protected readonly ILocalizationService _localizationService;
        protected readonly IManufacturerService _manufacturerService;
        protected readonly IManufacturerTemplateService _manufacturerTemplateService;
        protected readonly INopUrlHelper _nopUrlHelper;
        protected readonly IPictureService _pictureService;
        protected readonly IProductModelFactory _productModelFactory;
        protected readonly IProductService _productService;
        protected readonly IProductTagService _productTagService;
        protected readonly ISearchTermService _searchTermService;
        protected readonly ISpecificationAttributeService _specificationAttributeService;
        protected readonly IStaticCacheManager _staticCacheManager;
        protected readonly IStoreContext _storeContext;
        protected readonly ITopicService _topicService;
        protected readonly IUrlRecordService _urlRecordService;
        protected readonly IVendorService _vendorService;
        protected readonly IWebHelper _webHelper;
        protected readonly IWorkContext _workContext;
        protected readonly MediaSettings _mediaSettings;
        protected readonly SeoSettings _seoSettings;
        protected readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public CatalogModelFactory(BlogSettings blogSettings,
            CatalogSettings catalogSettings,
            DisplayDefaultMenuItemSettings displayDefaultMenuItemSettings,
            ForumSettings forumSettings,
            ICategoryService categoryService,
            ICategoryTemplateService categoryTemplateService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IHttpContextAccessor httpContextAccessor,
            IJsonLdModelFactory jsonLdModelFactory,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IManufacturerTemplateService manufacturerTemplateService,
            INopUrlHelper nopUrlHelper,
            IPictureService pictureService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IProductTagService productTagService,
            ISearchTermService searchTermService,
            ISpecificationAttributeService specificationAttributeService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            ITopicService topicService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            SeoSettings seoSettings,
            VendorSettings vendorSettings)
        {
            _blogSettings = blogSettings;
            _catalogSettings = catalogSettings;
            _displayDefaultMenuItemSettings = displayDefaultMenuItemSettings;
            _forumSettings = forumSettings;
            _categoryService = categoryService;
            _categoryTemplateService = categoryTemplateService;
            _currencyService = currencyService;
            _customerService = customerService;
            _eventPublisher = eventPublisher;
            _httpContextAccessor = httpContextAccessor;
            _jsonLdModelFactory = jsonLdModelFactory;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _manufacturerTemplateService = manufacturerTemplateService;
            _nopUrlHelper = nopUrlHelper;
            _pictureService = pictureService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _productTagService = productTagService;
            _searchTermService = searchTermService;
            _specificationAttributeService = specificationAttributeService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _topicService = topicService;
            _urlRecordService = urlRecordService;
            _vendorService = vendorService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _seoSettings = seoSettings;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets the category simple model
        /// </summary>
        /// <param name="elem">Category (simple) xml</param>
        /// <returns>Category simple model</returns>
        protected virtual CategorySimpleModel GetCategorySimpleModel(XElement elem)
        {
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
                Route = _nopUrlHelper.RouteGenericUrlAsync<Category>(new { SeName = elem.XPathSelectElement("SeName").Value }).Result
            };

            return model;
        }

        /// <summary>
        /// Gets the price range converted to primary store currency
        /// </summary>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the <see cref="Task"/> containing the price range converted to primary store currency
        /// </returns>
        protected virtual async Task<PriceRangeModel> GetConvertedPriceRangeAsync(CatalogProductsCommand command)
        {
            var result = new PriceRangeModel();

            if (string.IsNullOrWhiteSpace(command.Price))
                return result;

            var fromTo = command.Price.Trim().Split(new[] { '-' });
            if (fromTo.Length == 2)
            {
                var rawFromPrice = fromTo[0]?.Trim();
                if (!string.IsNullOrEmpty(rawFromPrice) && decimal.TryParse(rawFromPrice, out var from))
                    result.From = from;

                var rawToPrice = fromTo[1]?.Trim();
                if (!string.IsNullOrEmpty(rawToPrice) && decimal.TryParse(rawToPrice, out var to))
                    result.To = to;

                if (result.From > result.To)
                    result.From = result.To;

                var workingCurrency = await _workContext.GetWorkingCurrencyAsync();

                if (result.From.HasValue)
                    result.From = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(result.From.Value, workingCurrency);

                if (result.To.HasValue)
                    result.To = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(result.To.Value, workingCurrency);
            }

            return result;
        }

        /// <summary>
        /// Prepares the specification filter model
        /// </summary>
        /// <param name="selectedOptions">The selected options to filter the products</param>
        /// <param name="availableOptions">The available options to filter the products</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the specification filter model
        /// </returns>
        protected virtual async Task<SpecificationFilterModel> PrepareSpecificationFilterModel(IList<int> selectedOptions, IList<SpecificationAttributeOption> availableOptions)
        {
            var model = new SpecificationFilterModel();

            if (availableOptions?.Any() == true)
            {
                model.Enabled = true;

                var workingLanguage = await _workContext.GetWorkingLanguageAsync();

                foreach (var option in availableOptions)
                {
                    var attributeFilter = model.Attributes.FirstOrDefault(model => model.Id == option.SpecificationAttributeId);
                    if (attributeFilter == null)
                    {
                        var attribute = await _specificationAttributeService
                            .GetSpecificationAttributeByIdAsync(option.SpecificationAttributeId);
                        attributeFilter = new SpecificationAttributeFilterModel
                        {
                            Id = attribute.Id,
                            Name = await _localizationService
                                .GetLocalizedAsync(attribute, x => x.Name, workingLanguage.Id)
                        };
                        model.Attributes.Add(attributeFilter);
                    }

                    attributeFilter.Values.Add(new SpecificationAttributeValueFilterModel
                    {
                        Id = option.Id,
                        Name = await _localizationService
                            .GetLocalizedAsync(option, x => x.Name, workingLanguage.Id),
                        Selected = selectedOptions?.Any(optionId => optionId == option.Id) == true,
                        ColorSquaresRgb = option.ColorSquaresRgb
                    });
                }
            }

            return model;
        }

        /// <summary>
        /// Prepares the manufacturer filter model
        /// </summary>
        /// <param name="selectedManufacturers">The selected manufacturers to filter the products</param>
        /// <param name="availableManufacturers">The available manufacturers to filter the products</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the specification filter model
        /// </returns>
        protected virtual async Task<ManufacturerFilterModel> PrepareManufacturerFilterModel(IList<int> selectedManufacturers, IList<Manufacturer> availableManufacturers)
        {
            var model = new ManufacturerFilterModel();

            if (availableManufacturers?.Any() == true)
            {
                model.Enabled = true;

                var workingLanguage = await _workContext.GetWorkingLanguageAsync();

                foreach (var manufacturer in availableManufacturers)
                {
                    model.Manufacturers.Add(new SelectListItem
                    {
                        Value = manufacturer.Id.ToString(),
                        Text = await _localizationService
                            .GetLocalizedAsync(manufacturer, x => x.Name, workingLanguage.Id),
                        Selected = selectedManufacturers?
                            .Any(manufacturerId => manufacturerId == manufacturer.Id) == true
                    });
                }
            }

            return model;
        }

        /// <summary>
        /// Prepares the price range filter
        /// </summary>
        /// <param name="selectedPriceRange">The selected price range to filter the products</param>
        /// <param name="availablePriceRange">The available price range to filter the products</param>
        /// <returns>The price range filter</returns>
        protected virtual async Task<PriceRangeFilterModel> PreparePriceRangeFilterAsync(PriceRangeModel selectedPriceRange, PriceRangeModel availablePriceRange)
        {
            var model = new PriceRangeFilterModel();

            if (!availablePriceRange.To.HasValue || availablePriceRange.To <= 0
                || availablePriceRange.To == availablePriceRange.From)
            {
                // filter by price isn't available
                selectedPriceRange.From = null;
                selectedPriceRange.To = null;

                return model;
            }

            if (selectedPriceRange.From < availablePriceRange.From)
                selectedPriceRange.From = availablePriceRange.From;

            if (selectedPriceRange.To > availablePriceRange.To)
                selectedPriceRange.To = availablePriceRange.To;

            var workingCurrency = await _workContext.GetWorkingCurrencyAsync();

            Task<decimal> toWorkingCurrencyAsync(decimal? price)
                => _currencyService.ConvertFromPrimaryStoreCurrencyAsync(price.Value, workingCurrency);

            model.Enabled = true;
            model.AvailablePriceRange.From = availablePriceRange.From > decimal.Zero
                ? Math.Floor(await toWorkingCurrencyAsync(availablePriceRange.From))
                : decimal.Zero;
            model.AvailablePriceRange.To = Math.Ceiling(await toWorkingCurrencyAsync(availablePriceRange.To));

            if (!selectedPriceRange.From.HasValue || availablePriceRange.From == selectedPriceRange.From)
            {
                //already converted
                model.SelectedPriceRange.From = model.AvailablePriceRange.From;
            }
            else if (selectedPriceRange.From > decimal.Zero)
                model.SelectedPriceRange.From = Math.Floor(await toWorkingCurrencyAsync(selectedPriceRange.From));

            if (!selectedPriceRange.To.HasValue || availablePriceRange.To == selectedPriceRange.To)
            {
                //already converted
                model.SelectedPriceRange.To = model.AvailablePriceRange.To;
            }
            else if (selectedPriceRange.To > decimal.Zero)
                model.SelectedPriceRange.To = Math.Ceiling(await toWorkingCurrencyAsync(selectedPriceRange.To));

            return model;
        }

        /// <summary>
        /// Prepares catalog products
        /// </summary>
        /// <param name="model">Catalog products model</param>
        /// <param name="products">The products</param>
        /// <param name="isFiltering">A value indicating that filtering has been applied</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareCatalogProductsAsync(CatalogProductsModel model, IPagedList<Product> products, bool isFiltering = false)
        {
            if (!string.IsNullOrEmpty(model.WarningMessage))
                return;

            if (products.Count == 0 && isFiltering)
                model.NoResultMessage = await _localizationService.GetResourceAsync("Catalog.Products.NoResult");
            else
            {
                model.Products = (await _productModelFactory.PrepareProductOverviewModelsAsync(products)).ToList();
                model.LoadPagedList(products);
            }
        }

        #endregion

        #region Categories

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category model
        /// </returns>
        public virtual async Task<CategoryModel> PrepareCategoryModelAsync(Category category, CatalogProductsCommand command)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new CategoryModel
            {
                Id = category.Id,
                Name = await _localizationService.GetLocalizedAsync(category, x => x.Name),
                Description = await _localizationService.GetLocalizedAsync(category, x => x.Description),
                MetaKeywords = await _localizationService.GetLocalizedAsync(category, x => x.MetaKeywords),
                MetaDescription = await _localizationService.GetLocalizedAsync(category, x => x.MetaDescription),
                MetaTitle = await _localizationService.GetLocalizedAsync(category, x => x.MetaTitle),
                SeName = await _urlRecordService.GetSeNameAsync(category),
                CatalogProductsModel = await PrepareCategoryProductsModelAsync(category, command)
            };

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

                if (_seoSettings.MicrodataEnabled)
                {
                    var categoryBreadcrumb = model.CategoryBreadcrumb.Select(c => new CategorySimpleModel { Id = c.Id, Name = c.Name, SeName = c.SeName }).ToList();
                    var jsonLdModel = await _jsonLdModelFactory.PrepareJsonLdCategoryBreadcrumbAsync(categoryBreadcrumb);
                    model.JsonLd = JsonConvert
                        .SerializeObject(jsonLdModel, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }
            }

            var currentStore = await _storeContext.GetCurrentStoreAsync();
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
                        currentStore);

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
                var featuredProducts = await _productService.GetCategoryFeaturedProductsAsync(category.Id, currentStore.Id);
                if (featuredProducts != null)
                    model.FeaturedProducts = (await _productModelFactory.PrepareProductOverviewModelsAsync(featuredProducts)).ToList();
            }

            return model;
        }

        /// <summary>
        /// Prepare category template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category template view path
        /// </returns>
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category navigation model
        /// </returns>
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the op menu model
        /// </returns>
        public virtual async Task<TopMenuModel> PrepareTopMenuModelAsync()
        {
            var cachedCategoriesModel = new List<CategorySimpleModel>();
            //categories
            if (!_catalogSettings.UseAjaxLoadMenu)
                cachedCategoriesModel = await PrepareCategorySimpleModelsAsync();

            var store = await _storeContext.GetCurrentStoreAsync();

            //top menu topics
            var topicModel = await (await _topicService.GetAllTopicsAsync(store.Id, onlyIncludedInTopMenu: true))
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of homepage category models
        /// </returns>
        public virtual async Task<List<CategoryModel>> PrepareHomepageCategoryModelsAsync()
        {
            var language = await _workContext.GetWorkingLanguageAsync();
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
            var store = await _storeContext.GetCurrentStoreAsync();
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;
            var categoriesCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoryHomepageKey,
                store, customerRoleIds, pictureSize, language, _webHelper.IsCurrentConnectionSecured());

            var model = await _staticCacheManager.GetAsync(categoriesCacheKey, async () =>
            {
                var homepageCategories = await _categoryService.GetAllCategoriesDisplayedOnHomepageAsync();
                return await homepageCategories.SelectAwait(async category =>
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
                    var secured = _webHelper.IsCurrentConnectionSecured();
                    var categoryPictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoryPictureModelKey,
                        category, pictureSize, true, language, secured, store);
                    catModel.PictureModel = await _staticCacheManager.GetAsync(categoryPictureCacheKey, async () =>
                    {
                        var picture = await _pictureService.GetPictureByIdAsync(category.PictureId);
                        string fullSizeImageUrl, imageUrl;

                        (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                        (imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);

                        var titleLocale = await _localizationService.GetResourceAsync("Media.Category.ImageLinkTitleFormat");
                        var altLocale = await _localizationService.GetResourceAsync("Media.Category.ImageAlternateTextFormat");
                        return new PictureModel
                        {
                            FullSizeImageUrl = fullSizeImageUrl,
                            ImageUrl = imageUrl,
                            Title = string.Format(titleLocale, catModel.Name),
                            AlternateText = string.Format(altLocale, catModel.Name)
                        };
                    });

                    return catModel;
                }).ToListAsync();
            });

            return model;
        }

        /// <summary>
        /// Prepare root categories for menu
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of category (simple) models
        /// </returns>
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the 
        /// </returns>
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

        /// <summary>
        /// Prepares the category products model
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category products model
        /// </returns>
        public virtual async Task<CatalogProductsModel> PrepareCategoryProductsModelAsync(Category category, CatalogProductsCommand command)
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

            if (_catalogSettings.EnableSpecificationAttributeFiltering)
            {
                model.SpecificationFilter = await PrepareSpecificationFilterModel(command.SpecificationOptionIds, filterableOptions);
            }

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
            await PrepareCatalogProductsAsync(model, products, isFiltering);

            return model;
        }

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of category (simple) models
        /// </returns>
        public virtual async Task<List<CategorySimpleModel>> PrepareCategorySimpleModelsAsync()
        {
            //load and cache them
            var language = await _workContext.GetWorkingLanguageAsync();
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
            var store = await _storeContext.GetCurrentStoreAsync();
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoryAllModelKey,
                language, customerRoleIds, store);

            return await _staticCacheManager.GetAsync(cacheKey, async () => await PrepareCategorySimpleModelsAsync(0));
        }

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <param name="rootCategoryId">Root category identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of category (simple) models
        /// </returns>
        public virtual async Task<List<CategorySimpleModel>> PrepareCategorySimpleModelsAsync(int rootCategoryId, bool loadSubCategories = true)
        {
            var result = new List<CategorySimpleModel>();

            //little hack for performance optimization
            //we know that this method is used to load top and left menu for categories.
            //it'll load all categories anyway.
            //so there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
            //so we load all categories at once (we know they are cached)
            var store = await _storeContext.GetCurrentStoreAsync();
            var allCategories = await _categoryService.GetAllCategoriesAsync(storeId: store.Id);
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
                            await _categoryService.GetChildCategoryIdsAsync(category.Id, store.Id));

                    categoryModel.NumberOfProducts =
                        await _productService.GetNumberOfProductsInCategoryAsync(categoryIds, store.Id);
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the xml document of category (simple) models
        /// </returns>
        public virtual async Task<XDocument> PrepareCategoryXmlDocumentAsync()
        {
            var language = await _workContext.GetWorkingLanguageAsync();
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
            var store = await _storeContext.GetCurrentStoreAsync();
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoryXmlAllModelKey,
                language, customerRoleIds, store);

            return await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var categories = await PrepareCategorySimpleModelsAsync();

                var xsSubmit = new XmlSerializer(typeof(List<CategorySimpleModel>));

                var settings = new XmlWriterSettings
                {
                    Async = true,
                    ConformanceLevel = ConformanceLevel.Auto
                };

                await using var strWriter = new StringWriter();
                await using var writer = XmlWriter.Create(strWriter, settings);
                xsSubmit.Serialize(writer, categories);
                var xml = strWriter.ToString();

                return XDocument.Parse(xml);
            });
        }

        #endregion

        #region Manufacturers

        /// <summary>
        /// Prepare manufacturer model
        /// </summary>
        /// <param name="manufacturer">Manufacturer identifier</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer model
        /// </returns>
        public virtual async Task<ManufacturerModel> PrepareManufacturerModelAsync(Manufacturer manufacturer, CatalogProductsCommand command)
        {
            if (manufacturer == null)
                throw new ArgumentNullException(nameof(manufacturer));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new ManufacturerModel
            {
                Id = manufacturer.Id,
                Name = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name),
                Description = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Description),
                MetaKeywords = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaKeywords),
                MetaDescription = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaDescription),
                MetaTitle = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaTitle),
                SeName = await _urlRecordService.GetSeNameAsync(manufacturer),
                CatalogProductsModel = await PrepareManufacturerProductsModelAsync(manufacturer, command)
            };

            //featured products
            if (!_catalogSettings.IgnoreFeaturedProducts)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                var storeId = store.Id;
                var featuredProducts = await _productService.GetManufacturerFeaturedProductsAsync(manufacturer.Id, storeId);
                if (featuredProducts != null)
                    model.FeaturedProducts = (await _productModelFactory.PrepareProductOverviewModelsAsync(featuredProducts)).ToList();
            }

            return model;
        }

        /// <summary>
        /// Prepares the manufacturer products model
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer products model
        /// </returns>
        public virtual async Task<CatalogProductsModel> PrepareManufacturerProductsModelAsync(Manufacturer manufacturer, CatalogProductsCommand command)
        {
            if (manufacturer == null)
                throw new ArgumentNullException(nameof(manufacturer));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new CatalogProductsModel
            {
                UseAjaxLoading = _catalogSettings.UseAjaxCatalogProductsLoading
            };

            var manufacturerIds = new List<int> { manufacturer.Id };
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            //sorting
            await PrepareSortingOptionsAsync(model, command);
            //view mode
            await PrepareViewModesAsync(model, command);
            //page size
            await PreparePageSizeOptionsAsync(model, command, manufacturer.AllowCustomersToSelectPageSize,
                manufacturer.PageSizeOptions, manufacturer.PageSize);

            //price range
            PriceRangeModel selectedPriceRange = null;
            if (_catalogSettings.EnablePriceRangeFiltering && manufacturer.PriceRangeFiltering)
            {
                selectedPriceRange = await GetConvertedPriceRangeAsync(command);

                PriceRangeModel availablePriceRange = null;
                if (!manufacturer.ManuallyPriceRange)
                {
                    async Task<decimal?> getProductPriceAsync(ProductSortingEnum orderBy)
                    {
                        var products = await _productService.SearchProductsAsync(0, 1,
                            manufacturerIds: manufacturerIds,
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
                        From = manufacturer.PriceFrom,
                        To = manufacturer.PriceTo
                    };
                }

                model.PriceRangeFilter = await PreparePriceRangeFilterAsync(selectedPriceRange, availablePriceRange);
            }

            // filterable options
            var filterableOptions = await _specificationAttributeService
                .GetFiltrableSpecificationAttributeOptionsByManufacturerIdAsync(manufacturer.Id);

            if (_catalogSettings.EnableSpecificationAttributeFiltering)
            {
                model.SpecificationFilter = await PrepareSpecificationFilterModel(command.SpecificationOptionIds, filterableOptions);
            }

            var filteredSpecs = command.SpecificationOptionIds is null ? null : filterableOptions.Where(fo => command.SpecificationOptionIds.Contains(fo.Id)).ToList();

            //products
            var products = await _productService.SearchProductsAsync(
                command.PageNumber - 1,
                command.PageSize,
                manufacturerIds: manufacturerIds,
                storeId: currentStore.Id,
                visibleIndividuallyOnly: true,
                excludeFeaturedProducts: !_catalogSettings.IgnoreFeaturedProducts && !_catalogSettings.IncludeFeaturedProductsInNormalLists,
                priceMin: selectedPriceRange?.From,
                priceMax: selectedPriceRange?.To,
                filteredSpecOptions: filteredSpecs,
                orderBy: (ProductSortingEnum)command.OrderBy);

            var isFiltering = filterableOptions.Any() || selectedPriceRange?.From is not null;
            await PrepareCatalogProductsAsync(model, products, isFiltering);

            return model;
        }

        /// <summary>
        /// Prepare manufacturer template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer template view path
        /// </returns>
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of manufacturer models
        /// </returns>
        public virtual async Task<List<ManufacturerModel>> PrepareManufacturerAllModelsAsync()
        {
            var model = new List<ManufacturerModel>();

            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(storeId: currentStore.Id);
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
                    _webHelper.IsCurrentConnectionSecured(), currentStore);
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer navigation model
        /// </returns>
        public virtual async Task<ManufacturerNavigationModel> PrepareManufacturerNavigationModelAsync(int currentManufacturerId)
        {
            var language = await _workContext.GetWorkingLanguageAsync();
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
            var store = await _storeContext.GetCurrentStoreAsync();
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ManufacturerNavigationModelKey,
                currentManufacturerId, language, customerRoleIds, store);
            var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var currentManufacturer = await _manufacturerService.GetManufacturerByIdAsync(currentManufacturerId);

                var manufacturers = await _manufacturerService.GetAllManufacturersAsync(storeId: store.Id,
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
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor model
        /// </returns>
        public virtual async Task<VendorModel> PrepareVendorModelAsync(Vendor vendor, CatalogProductsCommand command)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new VendorModel
            {
                Id = vendor.Id,
                Name = await _localizationService.GetLocalizedAsync(vendor, x => x.Name),
                Description = await _localizationService.GetLocalizedAsync(vendor, x => x.Description),
                MetaKeywords = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaKeywords),
                MetaDescription = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaDescription),
                MetaTitle = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaTitle),
                SeName = await _urlRecordService.GetSeNameAsync(vendor),
                AllowCustomersToContactVendors = _vendorSettings.AllowCustomersToContactVendors,
                CatalogProductsModel = await PrepareVendorProductsModelAsync(vendor, command)
            };

            return model;
        }

        /// <summary>
        /// Prepares the vendor products model
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor products model
        /// </returns>
        public virtual async Task<CatalogProductsModel> PrepareVendorProductsModelAsync(Vendor vendor, CatalogProductsCommand command)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new CatalogProductsModel
            {
                UseAjaxLoading = _catalogSettings.UseAjaxCatalogProductsLoading
            };

            //sorting
            await PrepareSortingOptionsAsync(model, command);
            //view mode
            await PrepareViewModesAsync(model, command);
            //page size
            await PreparePageSizeOptionsAsync(model, command, vendor.AllowCustomersToSelectPageSize,
                vendor.PageSizeOptions, vendor.PageSize);

            //price range
            PriceRangeModel selectedPriceRange = null;
            var store = await _storeContext.GetCurrentStoreAsync();
            if (_catalogSettings.EnablePriceRangeFiltering && vendor.PriceRangeFiltering)
            {
                selectedPriceRange = await GetConvertedPriceRangeAsync(command);

                PriceRangeModel availablePriceRange;
                if (!vendor.ManuallyPriceRange)
                {
                    async Task<decimal?> getProductPriceAsync(ProductSortingEnum orderBy)
                    {
                        var products = await _productService.SearchProductsAsync(0, 1,
                            vendorId: vendor.Id,
                            storeId: store.Id,
                            visibleIndividuallyOnly: true,
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
                        From = vendor.PriceFrom,
                        To = vendor.PriceTo
                    };
                }

                model.PriceRangeFilter = await PreparePriceRangeFilterAsync(selectedPriceRange, availablePriceRange);
            }

            //products
            var products = await _productService.SearchProductsAsync(
                command.PageNumber - 1,
                command.PageSize,
                vendorId: vendor.Id,
                priceMin: selectedPriceRange?.From,
                priceMax: selectedPriceRange?.To,
                storeId: store.Id,
                visibleIndividuallyOnly: true,
                orderBy: (ProductSortingEnum)command.OrderBy);

            var isFiltering = selectedPriceRange?.From is not null;
            await PrepareCatalogProductsAsync(model, products, isFiltering);

            return model;
        }

        /// <summary>
        /// Prepare vendor all models
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of vendor models
        /// </returns>
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor navigation model
        /// </returns>
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
        /// <param name="numberTagsToReturn">The number of tags to be returned; pass 0 to get all tags</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tags model
        /// </returns>
        public virtual async Task<PopularProductTagsModel> PreparePopularProductTagsModelAsync(int numberTagsToReturn = 0)
        {
            var model = new PopularProductTagsModel();

            var currentStore = await _storeContext.GetCurrentStoreAsync();

            var tagStats = await _productTagService.GetProductCountAsync(currentStore.Id);

            model.TotalTags = tagStats.Count;

            model.Tags.AddRange(await tagStats
                //Take the most popular tags if specified
                .OrderByDescending(x => x.Value).Take(numberTagsToReturn > 0 ? numberTagsToReturn : tagStats.Count)
                .SelectAwait(async tagStat =>
                {
                    var tag = await _productTagService.GetProductTagByIdAsync(tagStat.Key);

                    return new ProductTagModel
                    {
                        Id = tag.Id,
                        Name = await _localizationService.GetLocalizedAsync(tag, t => t.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(tag),
                        ProductCount = tagStat.Value
                    };
                })
                //sorting result
                .OrderBy(x => x.Name)
                .ToListAsync());

            return model;
        }

        /// <summary>
        /// Prepare products by tag model
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the products by tag model
        /// </returns>
        public virtual async Task<ProductsByTagModel> PrepareProductsByTagModelAsync(ProductTag productTag, CatalogProductsCommand command)
        {
            if (productTag == null)
                throw new ArgumentNullException(nameof(productTag));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new ProductsByTagModel
            {
                Id = productTag.Id,
                TagName = await _localizationService.GetLocalizedAsync(productTag, y => y.Name),
                TagSeName = await _urlRecordService.GetSeNameAsync(productTag),
                CatalogProductsModel = await PrepareTagProductsModelAsync(productTag, command)
            };

            return model;
        }

        /// <summary>
        /// Prepares the tag products model
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ag products model
        /// </returns>
        public virtual async Task<CatalogProductsModel> PrepareTagProductsModelAsync(ProductTag productTag, CatalogProductsCommand command)
        {
            if (productTag == null)
                throw new ArgumentNullException(nameof(productTag));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new CatalogProductsModel
            {
                UseAjaxLoading = _catalogSettings.UseAjaxCatalogProductsLoading
            };

            //sorting
            await PrepareSortingOptionsAsync(model, command);
            //view mode
            await PrepareViewModesAsync(model, command);
            //page size
            await PreparePageSizeOptionsAsync(model, command, _catalogSettings.ProductsByTagAllowCustomersToSelectPageSize,
                _catalogSettings.ProductsByTagPageSizeOptions, _catalogSettings.ProductsByTagPageSize);

            //price range
            PriceRangeModel selectedPriceRange = null;
            var store = await _storeContext.GetCurrentStoreAsync();
            if (_catalogSettings.EnablePriceRangeFiltering && _catalogSettings.ProductsByTagPriceRangeFiltering)
            {
                selectedPriceRange = await GetConvertedPriceRangeAsync(command);

                PriceRangeModel availablePriceRange;
                if (!_catalogSettings.ProductsByTagManuallyPriceRange)
                {
                    async Task<decimal?> getProductPriceAsync(ProductSortingEnum orderBy)
                    {
                        var products = await _productService.SearchProductsAsync(0, 1,
                            storeId: store.Id,
                            productTagId: productTag.Id,
                            visibleIndividuallyOnly: true,
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
                        From = _catalogSettings.ProductsByTagPriceFrom,
                        To = _catalogSettings.ProductsByTagPriceTo
                    };
                }

                model.PriceRangeFilter = await PreparePriceRangeFilterAsync(selectedPriceRange, availablePriceRange);
            }

            //products
            var products = await _productService.SearchProductsAsync(
                command.PageNumber - 1,
                command.PageSize,
                priceMin: selectedPriceRange?.From,
                priceMax: selectedPriceRange?.To,
                storeId: store.Id,
                productTagId: productTag.Id,
                visibleIndividuallyOnly: true,
                orderBy: (ProductSortingEnum)command.OrderBy);

            var isFiltering = selectedPriceRange?.From is not null;
            await PrepareCatalogProductsAsync(model, products, isFiltering);

            return model;
        }

        #endregion

        #region New products

        /// <summary>
        /// Prepare new products model
        /// </summary>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the new products model
        /// </returns>
        public virtual async Task<CatalogProductsModel> PrepareNewProductsModelAsync(CatalogProductsCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new CatalogProductsModel
            {
                UseAjaxLoading = _catalogSettings.UseAjaxCatalogProductsLoading
            };

            var currentStore = await _storeContext.GetCurrentStoreAsync();

            //page size
            await PreparePageSizeOptionsAsync(model, command, _catalogSettings.NewProductsAllowCustomersToSelectPageSize,
                _catalogSettings.NewProductsPageSizeOptions, _catalogSettings.NewProductsPageSize);

            //products
            var products = await _productService.GetProductsMarkedAsNewAsync(storeId: currentStore.Id,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);

            await PrepareCatalogProductsAsync(model, products);

            return model;
        }

        #endregion

        #region Searching

        /// <summary>
        /// Prepare search model
        /// </summary>
        /// <param name="model">Search model</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the search model
        /// </returns>
        public virtual async Task<SearchModel> PrepareSearchModelAsync(SearchModel model, CatalogProductsCommand command)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var categoriesModels = new List<SearchModel.CategoryModel>();
            //all categories
            var allCategories = await _categoryService.GetAllCategoriesAsync(storeId: currentStore.Id);
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

            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(storeId: currentStore.Id);
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

            model.CatalogProductsModel = await PrepareSearchProductsModelAsync(model, command);

            return model;
        }

        /// <summary>
        /// Prepares the search products model
        /// </summary>
        /// <param name="model">Search model</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the search products model
        /// </returns>
        public virtual async Task<CatalogProductsModel> PrepareSearchProductsModelAsync(SearchModel searchModel, CatalogProductsCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new CatalogProductsModel
            {
                UseAjaxLoading = _catalogSettings.UseAjaxCatalogProductsLoading
            };

            //sorting
            await PrepareSortingOptionsAsync(model, command);
            //view mode
            await PrepareViewModesAsync(model, command);
            //page size
            await PreparePageSizeOptionsAsync(model, command, _catalogSettings.SearchPageAllowCustomersToSelectPageSize,
                _catalogSettings.SearchPagePageSizeOptions, _catalogSettings.SearchPageProductsPerPage);

            var searchTerms = searchModel.q == null
                ? string.Empty
                : searchModel.q.Trim();

            IPagedList<Product> products = new PagedList<Product>(new List<Product>(), 0, 1);
            //only search if query string search keyword is set (used to avoid searching or displaying search term min length error message on /search page load)
            //we don't use "!string.IsNullOrEmpty(searchTerms)" in cases of "ProductSearchTermMinimumLength" set to 0 but searching by other parameters (e.g. category or price filter)
            var isSearchTermSpecified = _httpContextAccessor.HttpContext.Request.Query.ContainsKey("q");
            if (isSearchTermSpecified)
            {
                var currentStore = await _storeContext.GetCurrentStoreAsync();

                if (searchTerms.Length < _catalogSettings.ProductSearchTermMinimumLength)
                {
                    model.WarningMessage =
                        string.Format(await _localizationService.GetResourceAsync("Search.SearchTermMinimumLengthIsNCharacters"),
                            _catalogSettings.ProductSearchTermMinimumLength);
                }
                else
                {
                    var categoryIds = new List<int>();
                    var manufacturerId = 0;
                    var searchInDescriptions = false;
                    var vendorId = 0;
                    if (searchModel.advs)
                    {
                        //advanced search
                        var categoryId = searchModel.cid;
                        if (categoryId > 0)
                        {
                            categoryIds.Add(categoryId);
                            if (searchModel.isc)
                            {
                                //include subcategories
                                categoryIds.AddRange(
                                    await _categoryService.GetChildCategoryIdsAsync(categoryId, currentStore.Id));
                            }
                        }

                        manufacturerId = searchModel.mid;

                        if (searchModel.asv)
                            vendorId = searchModel.vid;

                        searchInDescriptions = searchModel.sid;
                    }

                    //var searchInProductTags = false;
                    var searchInProductTags = searchInDescriptions;
                    var workingLanguage = await _workContext.GetWorkingLanguageAsync();

                    //price range
                    PriceRangeModel selectedPriceRange = null;
                    if (_catalogSettings.EnablePriceRangeFiltering && _catalogSettings.SearchPagePriceRangeFiltering)
                    {
                        selectedPriceRange = await GetConvertedPriceRangeAsync(command);

                        PriceRangeModel availablePriceRange;
                        async Task<decimal?> getProductPriceAsync(ProductSortingEnum orderBy)
                        {
                            var products = await _productService.SearchProductsAsync(0, 1,
                                categoryIds: categoryIds,
                                manufacturerIds: new List<int> { manufacturerId },
                                storeId: currentStore.Id,
                                visibleIndividuallyOnly: true,
                                keywords: searchTerms,
                                searchDescriptions: searchInDescriptions,
                                searchProductTags: searchInProductTags,
                                languageId: workingLanguage.Id,
                                vendorId: vendorId,
                                orderBy: orderBy);

                            return products?.FirstOrDefault()?.Price ?? 0;
                        }

                        if (_catalogSettings.SearchPageManuallyPriceRange)
                        {
                            var to = await getProductPriceAsync(ProductSortingEnum.PriceDesc);

                            availablePriceRange = new PriceRangeModel
                            {
                                From = _catalogSettings.SearchPagePriceFrom,
                                To = to == 0 ? 0 : _catalogSettings.SearchPagePriceTo
                            };
                        }
                        else
                            availablePriceRange = new PriceRangeModel
                            {
                                From = await getProductPriceAsync(ProductSortingEnum.PriceAsc),
                                To = await getProductPriceAsync(ProductSortingEnum.PriceDesc)
                            };

                        model.PriceRangeFilter = await PreparePriceRangeFilterAsync(selectedPriceRange, availablePriceRange);
                    }

                    //products
                    products = await _productService.SearchProductsAsync(
                        command.PageNumber - 1,
                        command.PageSize,
                        categoryIds: categoryIds,
                        manufacturerIds: new List<int> { manufacturerId },
                        storeId: currentStore.Id,
                        visibleIndividuallyOnly: true,
                        keywords: searchTerms,
                        priceMin: selectedPriceRange?.From,
                        priceMax: selectedPriceRange?.To,
                        searchDescriptions: searchInDescriptions,
                        searchProductTags: searchInProductTags,
                        languageId: workingLanguage.Id,
                        orderBy: (ProductSortingEnum)command.OrderBy,
                        vendorId: vendorId);

                    //search term statistics
                    if (!string.IsNullOrEmpty(searchTerms))
                    {
                        var searchTerm =
                            await _searchTermService.GetSearchTermByKeywordAsync(searchTerms, currentStore.Id);
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
                                StoreId = currentStore.Id,
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
                        WorkingLanguageId = workingLanguage.Id,
                        VendorId = vendorId
                    });
                }
            }

            var isFiltering = !string.IsNullOrEmpty(searchTerms);
            await PrepareCatalogProductsAsync(model, products, isFiltering);

            return model;
        }

        /// <summary>
        /// Prepare search box model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the search box model
        /// </returns>
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

        #region Common

        /// <summary>
        /// Prepare sorting options
        /// </summary>
        /// <param name="model">Catalog products model</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task PrepareSortingOptionsAsync(CatalogProductsModel model, CatalogProductsCommand command)
        {
            //get active sorting options
            var activeSortingOptionsIds = Enum.GetValues(typeof(ProductSortingEnum)).Cast<int>()
                .Except(_catalogSettings.ProductSortingEnumDisabled).ToList();

            //order sorting options
            var orderedActiveSortingOptions = activeSortingOptionsIds
                .Select(id => new { Id = id, Order = _catalogSettings.ProductSortingEnumDisplayOrder.TryGetValue(id, out var order) ? order : id })
                .OrderBy(option => option.Order).ToList();

            //set the default option
            model.OrderBy = command.OrderBy;
            command.OrderBy = orderedActiveSortingOptions.FirstOrDefault()?.Id ?? (int)ProductSortingEnum.Position;

            //ensure that product sorting is enabled
            if (!_catalogSettings.AllowProductSorting)
                return;

            model.AllowProductSorting = true;
            command.OrderBy = model.OrderBy ?? command.OrderBy;

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
        }

        /// <summary>
        /// Prepare view modes
        /// </summary>
        /// <param name="model">Catalog products model</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task PrepareViewModesAsync(CatalogProductsModel model, CatalogProductsCommand command)
        {
            model.AllowProductViewModeChanging = _catalogSettings.AllowProductViewModeChanging;

            var viewMode = !string.IsNullOrEmpty(command.ViewMode)
                ? command.ViewMode
                : _catalogSettings.DefaultViewMode;
            model.ViewMode = viewMode;
            if (model.AllowProductViewModeChanging)
            {
                //grid
                model.AvailableViewModes.Add(new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Catalog.ViewMode.Grid"),
                    Value = "grid",
                    Selected = viewMode == "grid"
                });
                //list
                model.AvailableViewModes.Add(new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Catalog.ViewMode.List"),
                    Value = "list",
                    Selected = viewMode == "list"
                });
            }
        }

        /// <summary>
        /// Prepare page size options
        /// </summary>
        /// <param name="model">Catalog products model</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <param name="allowCustomersToSelectPageSize">Are customers allowed to select page size?</param>
        /// <param name="pageSizeOptions">Page size options</param>
        /// <param name="fixedPageSize">Fixed page size</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual Task PreparePageSizeOptionsAsync(CatalogProductsModel model, CatalogProductsCommand command,
            bool allowCustomersToSelectPageSize, string pageSizeOptions, int fixedPageSize)
        {
            if (command.PageNumber <= 0)
                command.PageNumber = 1;

            model.AllowCustomersToSelectPageSize = false;
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
                                command.PageSize = temp;
                        }
                    }

                    foreach (var pageSize in pageSizes)
                    {
                        if (!int.TryParse(pageSize, out var temp))
                            continue;

                        if (temp <= 0)
                            continue;

                        model.PageSizeOptions.Add(new SelectListItem
                        {
                            Text = pageSize,
                            Value = pageSize,
                            Selected = pageSize.Equals(command.PageSize.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        });
                    }

                    if (model.PageSizeOptions.Any())
                    {
                        model.PageSizeOptions = model.PageSizeOptions.OrderBy(x => int.Parse(x.Value)).ToList();
                        model.AllowCustomersToSelectPageSize = true;

                        if (command.PageSize <= 0)
                            command.PageSize = int.Parse(model.PageSizeOptions.First().Value);
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

        #endregion
    }
}
