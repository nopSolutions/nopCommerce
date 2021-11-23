using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class CatalogController : BasePublicController
    {
        #region Fields

        protected CatalogSettings CatalogSettings { get; }
        protected IAclService AclService { get; }
        protected ICatalogModelFactory CatalogModelFactory { get; }
        protected ICategoryService CategoryService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IManufacturerService ManufacturerService { get; }
        protected IPermissionService PermissionService { get; }
        protected IProductModelFactory ProductModelFactory { get; }
        protected IProductService ProductService { get; }
        protected IProductTagService ProductTagService { get; }
        protected IStoreContext StoreContext { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IVendorService VendorService { get; }
        protected IWebHelper WebHelper { get; }
        protected IWorkContext WorkContext { get; }
        protected MediaSettings MediaSettings { get; }
        protected VendorSettings VendorSettings { get; }

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
            IPermissionService permissionService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IProductTagService productTagService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            VendorSettings vendorSettings)
        {
            CatalogSettings = catalogSettings;
            AclService = aclService;
            CatalogModelFactory = catalogModelFactory;
            CategoryService = categoryService;
            CustomerActivityService = customerActivityService;
            GenericAttributeService = genericAttributeService;
            LocalizationService = localizationService;
            ManufacturerService = manufacturerService;
            PermissionService = permissionService;
            ProductModelFactory = productModelFactory;
            ProductService = productService;
            ProductTagService = productTagService;
            StoreContext = storeContext;
            StoreMappingService = storeMappingService;
            VendorService = vendorService;
            WebHelper = webHelper;
            WorkContext = workContext;
            MediaSettings = mediaSettings;
            VendorSettings = vendorSettings;
        }

        #endregion

        #region Categories

        public virtual async Task<IActionResult> Category(int categoryId, CatalogProductsCommand command)
        {
            var category = await CategoryService.GetCategoryByIdAsync(categoryId);

            if (!await CheckCategoryAvailabilityAsync(category))
                return InvokeHttp404();

            var store = await StoreContext.GetCurrentStoreAsync();

            //'Continue shopping' URL
            await GenericAttributeService.SaveAttributeAsync(await WorkContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                WebHelper.GetThisPageUrl(false),
                store.Id);

            //display "edit" (manage) link
            if (await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                DisplayEditLink(Url.Action("Edit", "Category", new { id = category.Id, area = AreaNames.Admin }));

            //activity log
            await CustomerActivityService.InsertActivityAsync("PublicStore.ViewCategory",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.PublicStore.ViewCategory"), category.Name), category);

            //model
            var model = await CatalogModelFactory.PrepareCategoryModelAsync(category, command);

            //template
            var templateViewPath = await CatalogModelFactory.PrepareCategoryTemplateViewPathAsync(category.CategoryTemplateId);
            return View(templateViewPath, model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> GetCategoryProducts(int categoryId, CatalogProductsCommand command)
        {
            var category = await CategoryService.GetCategoryByIdAsync(categoryId);

            if (!await CheckCategoryAvailabilityAsync(category))
                return NotFound();

            var model = await CatalogModelFactory.PrepareCategoryProductsModelAsync(category, command);

            return PartialView("_ProductsInGridOrLines", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetCatalogRoot()
        {
            var model = await CatalogModelFactory.PrepareRootCategoriesAsync();

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetCatalogSubCategories(int id)
        {
            var model = await CatalogModelFactory.PrepareSubCategoriesAsync(id);

            return Json(model);
        }

        #endregion

        #region Manufacturers

        public virtual async Task<IActionResult> Manufacturer(int manufacturerId, CatalogProductsCommand command)
        {
            var manufacturer = await ManufacturerService.GetManufacturerByIdAsync(manufacturerId);

            if (!await CheckManufacturerAvailabilityAsync(manufacturer))
                return InvokeHttp404();

            var store = await StoreContext.GetCurrentStoreAsync();

            //'Continue shopping' URL
            await GenericAttributeService.SaveAttributeAsync(await WorkContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                WebHelper.GetThisPageUrl(false),
                store.Id);

            //display "edit" (manage) link
            if (await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                DisplayEditLink(Url.Action("Edit", "Manufacturer", new { id = manufacturer.Id, area = AreaNames.Admin }));

            //activity log
            await CustomerActivityService.InsertActivityAsync("PublicStore.ViewManufacturer",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.PublicStore.ViewManufacturer"), manufacturer.Name), manufacturer);

            //model
            var model = await CatalogModelFactory.PrepareManufacturerModelAsync(manufacturer, command);

            //template
            var templateViewPath = await CatalogModelFactory.PrepareManufacturerTemplateViewPathAsync(manufacturer.ManufacturerTemplateId);

            return View(templateViewPath, model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> GetManufacturerProducts(int manufacturerId, CatalogProductsCommand command)
        {
            var manufacturer = await ManufacturerService.GetManufacturerByIdAsync(manufacturerId);

            if (!await CheckManufacturerAvailabilityAsync(manufacturer))
                return NotFound();

            var model = await CatalogModelFactory.PrepareManufacturerProductsModelAsync(manufacturer, command);

            return PartialView("_ProductsInGridOrLines", model);
        }

        public virtual async Task<IActionResult> ManufacturerAll()
        {
            var model = await CatalogModelFactory.PrepareManufacturerAllModelsAsync();

            return View(model);
        }

        #endregion

        #region Vendors

        public virtual async Task<IActionResult> Vendor(int vendorId, CatalogProductsCommand command)
        {
            var vendor = await VendorService.GetVendorByIdAsync(vendorId);

            if (!await CheckVendorAvailabilityAsync(vendor))
                return InvokeHttp404();

            var store = await StoreContext.GetCurrentStoreAsync();

            //'Continue shopping' URL
            await GenericAttributeService.SaveAttributeAsync(await WorkContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                WebHelper.GetThisPageUrl(false),
                store.Id);

            //display "edit" (manage) link
            if (await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                DisplayEditLink(Url.Action("Edit", "Vendor", new { id = vendor.Id, area = AreaNames.Admin }));

            //model
            var model = await CatalogModelFactory.PrepareVendorModelAsync(vendor, command);

            return View(model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> GetVendorProducts(int vendorId, CatalogProductsCommand command)
        {
            var vendor = await VendorService.GetVendorByIdAsync(vendorId);

            if (!await CheckVendorAvailabilityAsync(vendor))
                return NotFound();

            var model = await CatalogModelFactory.PrepareVendorProductsModelAsync(vendor, command);

            return PartialView("_ProductsInGridOrLines", model);
        }

        public virtual async Task<IActionResult> VendorAll()
        {
            //we don't allow viewing of vendors if "vendors" block is hidden
            if (VendorSettings.VendorsBlockItemsToDisplay == 0)
                return RedirectToRoute("Homepage");

            var model = await CatalogModelFactory.PrepareVendorAllModelsAsync();
            return View(model);
        }

        #endregion

        #region Product tags

        public virtual async Task<IActionResult> ProductsByTag(int productTagId, CatalogProductsCommand command)
        {
            var productTag = await ProductTagService.GetProductTagByIdAsync(productTagId);
            if (productTag == null)
                return InvokeHttp404();

            var model = await CatalogModelFactory.PrepareProductsByTagModelAsync(productTag, command);

            return View(model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> GetTagProducts(int tagId, CatalogProductsCommand command)
        {
            var productTag = await ProductTagService.GetProductTagByIdAsync(tagId);
            if (productTag == null)
                return NotFound();

            var model = await CatalogModelFactory.PrepareTagProductsModelAsync(productTag, command);

            return PartialView("_ProductsInGridOrLines", model);
        }

        public virtual async Task<IActionResult> ProductTagsAll()
        {
            var model = await CatalogModelFactory.PreparePopularProductTagsModelAsync();

            return View(model);
        }

        #endregion

        #region Searching

        public virtual async Task<IActionResult> Search(SearchModel model, CatalogProductsCommand command)
        {
            var store = await StoreContext.GetCurrentStoreAsync();

            //'Continue shopping' URL
            await GenericAttributeService.SaveAttributeAsync(await WorkContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                WebHelper.GetThisPageUrl(true),
                store.Id);

            if (model == null)
                model = new SearchModel();

            model = await CatalogModelFactory.PrepareSearchModelAsync(model, command);

            return View(model);
        }

        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> SearchTermAutoComplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Content("");

            term = term.Trim();

            if (string.IsNullOrWhiteSpace(term) || term.Length < CatalogSettings.ProductSearchTermMinimumLength)
                return Content("");

            //products
            var productNumber = CatalogSettings.ProductSearchAutoCompleteNumberOfProducts > 0 ?
                CatalogSettings.ProductSearchAutoCompleteNumberOfProducts : 10;
            var store = await StoreContext.GetCurrentStoreAsync();
            var products = await ProductService.SearchProductsAsync(0,
                storeId: store.Id,
                keywords: term,
                languageId: (await WorkContext.GetWorkingLanguageAsync()).Id,
                visibleIndividuallyOnly: true,
                pageSize: productNumber);

            var showLinkToResultSearch = CatalogSettings.ShowLinkToAllResultInSearchAutoComplete && (products.TotalCount > productNumber);

            var models = (await ProductModelFactory.PrepareProductOverviewModelsAsync(products, false, CatalogSettings.ShowProductImagesInSearchAutoComplete, MediaSettings.AutoCompleteSearchThumbPictureSize)).ToList();
            var result = (from p in models
                          select new
                          {
                              label = p.Name,
                              producturl = Url.RouteUrl("Product", new { SeName = p.SeName }),
                              productpictureurl = p.DefaultPictureModel.ImageUrl,
                              showlinktoresultsearch = showLinkToResultSearch
                          })
                .ToList();
            return Json(result);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> SearchProducts(SearchModel searchModel, CatalogProductsCommand command)
        {
            if (searchModel == null)
                searchModel = new SearchModel();

            var model = await CatalogModelFactory.PrepareSearchProductsModelAsync(searchModel, command);

            return PartialView("_ProductsInGridOrLines", model);
        }

        #endregion

        #region Utilities

        private async Task<bool> CheckCategoryAvailabilityAsync(Category category)
        {
            var isAvailable = true;

            if (category == null || category.Deleted)
                isAvailable = false;

            var notAvailable =
                //published?
                !category.Published ||
                //ACL (access control list) 
                !await AclService.AuthorizeAsync(category) ||
                //Store mapping
                !await StoreMappingService.AuthorizeAsync(category);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories);
            if (notAvailable && !hasAdminAccess)
                isAvailable = false;

            return isAvailable;
        }

        private async Task<bool> CheckManufacturerAvailabilityAsync(Manufacturer manufacturer)
        {
            var isAvailable = true;

            if (manufacturer == null || manufacturer.Deleted)
                isAvailable = false;

            var notAvailable =
                //published?
                !manufacturer.Published ||
                //ACL (access control list) 
                !await AclService.AuthorizeAsync(manufacturer) ||
                //Store mapping
                !await StoreMappingService.AuthorizeAsync(manufacturer);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers);
            if (notAvailable && !hasAdminAccess)
                isAvailable = false;

            return isAvailable;
        }

        private Task<bool> CheckVendorAvailabilityAsync(Vendor vendor)
        {
            var isAvailable = true;

            if (vendor == null || vendor.Deleted || !vendor.Active)
                isAvailable = false;

            return Task.FromResult(isAvailable);
        }

        #endregion
    }
}