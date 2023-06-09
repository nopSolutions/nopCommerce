using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Core.Rss;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class CatalogController : BasePublicController
    {
        #region Fields

        protected readonly CatalogSettings _catalogSettings;
        protected readonly IAclService _aclService;
        protected readonly ICatalogModelFactory _catalogModelFactory;
        protected readonly ICategoryService _categoryService;
        protected readonly ICustomerActivityService _customerActivityService;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly ILocalizationService _localizationService;
        protected readonly IManufacturerService _manufacturerService;
        protected readonly INopUrlHelper _nopUrlHelper;
        protected readonly IPermissionService _permissionService;
        protected readonly IProductModelFactory _productModelFactory;
        protected readonly IProductService _productService;
        protected readonly IProductTagService _productTagService;
        protected readonly IStoreContext _storeContext;
        protected readonly IStoreMappingService _storeMappingService;
        protected readonly IUrlRecordService _urlRecordService;
        protected readonly IVendorService _vendorService;
        protected readonly IWebHelper _webHelper;
        protected readonly IWorkContext _workContext;
        protected readonly MediaSettings _mediaSettings;
        protected readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public CatalogController(CatalogSettings catalogSettings,
            IAclService aclService,
            ICatalogModelFactory catalogModelFactory,
            ICategoryService categoryService,
            ICustomerActivityService customerActivityService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            INopUrlHelper nopUrlHelper,
            IPermissionService permissionService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IProductTagService productTagService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            VendorSettings vendorSettings)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _catalogModelFactory = catalogModelFactory;
            _categoryService = categoryService;
            _customerActivityService = customerActivityService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _nopUrlHelper = nopUrlHelper;
            _permissionService = permissionService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _productTagService = productTagService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _urlRecordService = urlRecordService;
            _vendorService = vendorService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Categories

        public virtual async Task<IActionResult> Category(int categoryId, CatalogProductsCommand command)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);

            if (!await CheckCategoryAvailabilityAsync(category))
                return InvokeHttp404();

            var store = await _storeContext.GetCurrentStoreAsync();

            //'Continue shopping' URL
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                store.Id);

            //display "edit" (manage) link
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                DisplayEditLink(Url.Action("Edit", "Category", new { id = category.Id, area = AreaNames.Admin }));

            //activity log
            await _customerActivityService.InsertActivityAsync("PublicStore.ViewCategory",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ViewCategory"), category.Name), category);

            //model
            var model = await _catalogModelFactory.PrepareCategoryModelAsync(category, command);

            //template
            var templateViewPath = await _catalogModelFactory.PrepareCategoryTemplateViewPathAsync(category.CategoryTemplateId);
            return View(templateViewPath, model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> GetCategoryProducts(int categoryId, CatalogProductsCommand command)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);

            if (!await CheckCategoryAvailabilityAsync(category))
                return NotFound();

            var model = await _catalogModelFactory.PrepareCategoryProductsModelAsync(category, command);

            return PartialView("_ProductsInGridOrLines", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetCatalogRoot()
        {
            var model = await _catalogModelFactory.PrepareRootCategoriesAsync();

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetCatalogSubCategories(int id)
        {
            var model = await _catalogModelFactory.PrepareSubCategoriesAsync(id);

            return Json(model);
        }

        #endregion

        #region Manufacturers

        public virtual async Task<IActionResult> Manufacturer(int manufacturerId, CatalogProductsCommand command)
        {
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId);

            if (!await CheckManufacturerAvailabilityAsync(manufacturer))
                return InvokeHttp404();

            var store = await _storeContext.GetCurrentStoreAsync();

            //'Continue shopping' URL
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                store.Id);

            //display "edit" (manage) link
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                DisplayEditLink(Url.Action("Edit", "Manufacturer", new { id = manufacturer.Id, area = AreaNames.Admin }));

            //activity log
            await _customerActivityService.InsertActivityAsync("PublicStore.ViewManufacturer",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ViewManufacturer"), manufacturer.Name), manufacturer);

            //model
            var model = await _catalogModelFactory.PrepareManufacturerModelAsync(manufacturer, command);

            //template
            var templateViewPath = await _catalogModelFactory.PrepareManufacturerTemplateViewPathAsync(manufacturer.ManufacturerTemplateId);

            return View(templateViewPath, model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> GetManufacturerProducts(int manufacturerId, CatalogProductsCommand command)
        {
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId);

            if (!await CheckManufacturerAvailabilityAsync(manufacturer))
                return NotFound();

            var model = await _catalogModelFactory.PrepareManufacturerProductsModelAsync(manufacturer, command);

            return PartialView("_ProductsInGridOrLines", model);
        }

        public virtual async Task<IActionResult> ManufacturerAll()
        {
            var model = await _catalogModelFactory.PrepareManufacturerAllModelsAsync();

            return View(model);
        }

        #endregion

        #region Vendors

        public virtual async Task<IActionResult> Vendor(int vendorId, CatalogProductsCommand command)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(vendorId);

            if (!await CheckVendorAvailabilityAsync(vendor))
                return InvokeHttp404();

            var store = await _storeContext.GetCurrentStoreAsync();

            //'Continue shopping' URL
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                store.Id);

            //display "edit" (manage) link
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                DisplayEditLink(Url.Action("Edit", "Vendor", new { id = vendor.Id, area = AreaNames.Admin }));

            //model
            var model = await _catalogModelFactory.PrepareVendorModelAsync(vendor, command);

            return View(model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> GetVendorProducts(int vendorId, CatalogProductsCommand command)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(vendorId);

            if (!await CheckVendorAvailabilityAsync(vendor))
                return NotFound();

            var model = await _catalogModelFactory.PrepareVendorProductsModelAsync(vendor, command);

            return PartialView("_ProductsInGridOrLines", model);
        }

        public virtual async Task<IActionResult> VendorAll()
        {
            //we don't allow viewing of vendors if "vendors" block is hidden
            if (_vendorSettings.VendorsBlockItemsToDisplay == 0)
                return RedirectToRoute("Homepage");

            var model = await _catalogModelFactory.PrepareVendorAllModelsAsync();
            return View(model);
        }

        #endregion

        #region Product tags

        public virtual async Task<IActionResult> ProductsByTag(int productTagId, CatalogProductsCommand command)
        {
            var productTag = await _productTagService.GetProductTagByIdAsync(productTagId);
            if (productTag == null)
                return InvokeHttp404();

            var model = await _catalogModelFactory.PrepareProductsByTagModelAsync(productTag, command);

            return View(model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> GetTagProducts(int tagId, CatalogProductsCommand command)
        {
            var productTag = await _productTagService.GetProductTagByIdAsync(tagId);
            if (productTag == null)
                return NotFound();

            var model = await _catalogModelFactory.PrepareTagProductsModelAsync(productTag, command);

            return PartialView("_ProductsInGridOrLines", model);
        }

        public virtual async Task<IActionResult> ProductTagsAll()
        {
            var model = await _catalogModelFactory.PreparePopularProductTagsModelAsync();

            return View(model);
        }

        #endregion

        #region New (recently added) products page

        public virtual async Task<IActionResult> NewProducts(CatalogProductsCommand command)
        {
            if (!_catalogSettings.NewProductsEnabled)
                return InvokeHttp404();

            var model = new NewProductsModel
            {
                CatalogProductsModel = await _catalogModelFactory.PrepareNewProductsModelAsync(command)
            };

            return View(model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> GetNewProducts(CatalogProductsCommand command)
        {
            if (!_catalogSettings.NewProductsEnabled)
                return NotFound();

            var model = await _catalogModelFactory.PrepareNewProductsModelAsync(command);

            return PartialView("_ProductsInGridOrLines", model);
        }

        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> NewProductsRss()
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var feed = new RssFeed(
                $"{await _localizationService.GetLocalizedAsync(store, x => x.Name)}: New products",
                "Information about products",
                new Uri(_webHelper.GetStoreLocation()),
                DateTime.UtcNow);

            if (!_catalogSettings.NewProductsEnabled)
                return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));

            var items = new List<RssItem>();

            var storeId = store.Id;
            var products = await _productService.GetProductsMarkedAsNewAsync(storeId: storeId);

            foreach (var product in products)
            {
                var seName = await _urlRecordService.GetSeNameAsync(product);
                var productUrl = await _nopUrlHelper.RouteGenericUrlAsync<Product>(new { SeName = seName }, _webHelper.GetCurrentRequestProtocol());
                var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);
                var productDescription = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription);
                var item = new RssItem(productName, productDescription, new Uri(productUrl), $"urn:store:{store.Id}:newProducts:product:{product.Id}", product.CreatedOnUtc);
                items.Add(item);
                //uncomment below if you want to add RSS enclosure for pictures
                //var picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
                //if (picture != null)
                //{
                //    var imageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ProductDetailsPictureSize);
                //    item.ElementExtensions.Add(new XElement("enclosure", new XAttribute("type", "image/jpeg"), new XAttribute("url", imageUrl), new XAttribute("length", picture.PictureBinary.Length)));
                //}

            }
            feed.Items = items;
            return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
        }

        #endregion

        #region Searching

        public virtual async Task<IActionResult> Search(SearchModel model, CatalogProductsCommand command)
        {
            var store = await _storeContext.GetCurrentStoreAsync();

            //'Continue shopping' URL
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(true),
                store.Id);

            if (model == null)
                model = new SearchModel();

            model = await _catalogModelFactory.PrepareSearchModelAsync(model, command);

            return View(model);
        }

        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> SearchTermAutoComplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Content("");

            term = term.Trim();

            if (string.IsNullOrWhiteSpace(term) || term.Length < _catalogSettings.ProductSearchTermMinimumLength)
                return Content("");

            //products
            var productNumber = _catalogSettings.ProductSearchAutoCompleteNumberOfProducts > 0 ?
                _catalogSettings.ProductSearchAutoCompleteNumberOfProducts : 10;
            var store = await _storeContext.GetCurrentStoreAsync();
            var products = await _productService.SearchProductsAsync(0,
                storeId: store.Id,
                keywords: term,
                languageId: (await _workContext.GetWorkingLanguageAsync()).Id,
                visibleIndividuallyOnly: true,
                pageSize: productNumber);

            var showLinkToResultSearch = _catalogSettings.ShowLinkToAllResultInSearchAutoComplete && (products.TotalCount > productNumber);

            var models = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, false, _catalogSettings.ShowProductImagesInSearchAutoComplete, _mediaSettings.AutoCompleteSearchThumbPictureSize)).ToList();
            var result = (from p in models
                          select new
                          {
                              label = p.Name,
                              producturl = Url.RouteUrl<Product>(new { SeName = p.SeName }),
                              productpictureurl = p.PictureModels.FirstOrDefault()?.ImageUrl,
                              showlinktoresultsearch = showLinkToResultSearch
                          })
                .ToList();
            return Json(result);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> SearchProducts(SearchModel searchModel, CatalogProductsCommand command)
        {
            if (searchModel == null)
                searchModel = new SearchModel();

            var model = await _catalogModelFactory.PrepareSearchProductsModelAsync(searchModel, command);

            return PartialView("_ProductsInGridOrLines", model);
        }

        #endregion

        #region Utilities

        protected virtual async Task<bool> CheckCategoryAvailabilityAsync(Category category)
        {
            if (category is null)
                return false;

            var isAvailable = true;

            if (category.Deleted)
                isAvailable = false;

            var notAvailable =
                //published?
                !category.Published ||
                //ACL (access control list) 
                !await _aclService.AuthorizeAsync(category) ||
                //Store mapping
                !await _storeMappingService.AuthorizeAsync(category);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories);
            if (notAvailable && !hasAdminAccess)
                isAvailable = false;

            return isAvailable;
        }

        protected virtual async Task<bool> CheckManufacturerAvailabilityAsync(Manufacturer manufacturer)
        {
            var isAvailable = true;

            if (manufacturer == null || manufacturer.Deleted)
                isAvailable = false;

            var notAvailable =
                //published?
                !manufacturer.Published ||
                //ACL (access control list) 
                !await _aclService.AuthorizeAsync(manufacturer) ||
                //Store mapping
                !await _storeMappingService.AuthorizeAsync(manufacturer);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers);
            if (notAvailable && !hasAdminAccess)
                isAvailable = false;

            return isAvailable;
        }

        protected virtual Task<bool> CheckVendorAvailabilityAsync(Vendor vendor)
        {
            var isAvailable = true;

            if (vendor == null || vendor.Deleted || !vendor.Active)
                isAvailable = false;

            return Task.FromResult(isAvailable);
        }

        #endregion
    }
}