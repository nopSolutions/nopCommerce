using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CategoryController : BaseAdminController
    {
        #region Fields

        protected IAclService AclService { get; }
        protected ICategoryModelFactory CategoryModelFactory { get; }
        protected ICategoryService CategoryService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ICustomerService CustomerService { get; }
        protected IDiscountService DiscountService { get; }
        protected IExportManager ExportManager { get; }
        protected IImportManager ImportManager { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedEntityService LocalizedEntityService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IPictureService PictureService { get; }
        protected IProductService ProductService { get; }
        protected IStaticCacheManager StaticCacheManager { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IStoreService StoreService { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IWorkContext WorkContext { get; }

        #endregion

        #region Ctor

        public CategoryController(IAclService aclService,
            ICategoryModelFactory categoryModelFactory,
            ICategoryService categoryService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDiscountService discountService,
            IExportManager exportManager,
            IImportManager importManager,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IProductService productService,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            AclService = aclService;
            CategoryModelFactory = categoryModelFactory;
            CategoryService = categoryService;
            CustomerActivityService = customerActivityService;
            CustomerService = customerService;
            DiscountService = discountService;
            ExportManager = exportManager;
            ImportManager = importManager;
            LocalizationService = localizationService;
            LocalizedEntityService = localizedEntityService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            PictureService = pictureService;
            ProductService = productService;
            StaticCacheManager = staticCacheManager;
            StoreMappingService = storeMappingService;
            StoreService = storeService;
            UrlRecordService = urlRecordService;
            WorkContext = workContext;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(Category category, CategoryModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(category,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(category,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(category,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(category,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);

                await LocalizedEntityService.SaveLocalizedValueAsync(category,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = await UrlRecordService.ValidateSeNameAsync(category, localized.SeName, localized.Name, false);
                await UrlRecordService.SaveSlugAsync(category, seName, localized.LanguageId);
            }
        }

        protected virtual async Task UpdatePictureSeoNamesAsync(Category category)
        {
            var picture = await PictureService.GetPictureByIdAsync(category.PictureId);
            if (picture != null)
                await PictureService.SetSeoFilenameAsync(picture.Id, await PictureService.GetPictureSeNameAsync(category.Name));
        }

        protected virtual async Task SaveCategoryAclAsync(Category category, CategoryModel model)
        {
            category.SubjectToAcl = model.SelectedCustomerRoleIds.Any();
            await CategoryService.UpdateCategoryAsync(category);

            var existingAclRecords = await AclService.GetAclRecordsAsync(category);
            var allCustomerRoles = await CustomerService.GetAllCustomerRolesAsync(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (!existingAclRecords.Any(acl => acl.CustomerRoleId == customerRole.Id))
                        await AclService.InsertAclRecordAsync(category, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        await AclService.DeleteAclRecordAsync(aclRecordToDelete);
                }
            }
        }

        protected virtual async Task SaveStoreMappingsAsync(Category category, CategoryModel model)
        {
            category.LimitedToStores = model.SelectedStoreIds.Any();
            await CategoryService.UpdateCategoryAsync(category);

            var existingStoreMappings = await StoreMappingService.GetStoreMappingsAsync(category);
            var allStores = await StoreService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                        await StoreMappingService.InsertStoreMappingAsync(category, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await StoreMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //prepare model
            var model = await CategoryModelFactory.PrepareCategorySearchModelAsync(new CategorySearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(CategorySearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await CategoryModelFactory.PrepareCategoryListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //prepare model
            var model = await CategoryModelFactory.PrepareCategoryModelAsync(new CategoryModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(CategoryModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var category = model.ToEntity<Category>();
                category.CreatedOnUtc = DateTime.UtcNow;
                category.UpdatedOnUtc = DateTime.UtcNow;
                await CategoryService.InsertCategoryAsync(category);

                //search engine name
                model.SeName = await UrlRecordService.ValidateSeNameAsync(category, model.SeName, category.Name, true);
                await UrlRecordService.SaveSlugAsync(category, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(category, model);

                //discounts
                var allDiscounts = await DiscountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                        await CategoryService.InsertDiscountCategoryMappingAsync(new DiscountCategoryMapping { DiscountId = discount.Id, EntityId = category.Id });
                }

                await CategoryService.UpdateCategoryAsync(category);

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(category);

                //ACL (customer roles)
                await SaveCategoryAclAsync(category, model);

                //stores
                await SaveStoreMappingsAsync(category, model);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewCategory",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewCategory"), category.Name), category);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Categories.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = category.Id });
            }

            //prepare model
            model = await CategoryModelFactory.PrepareCategoryModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a category with the specified id
            var category = await CategoryService.GetCategoryByIdAsync(id);
            if (category == null || category.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = await CategoryModelFactory.PrepareCategoryModelAsync(null, category);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(CategoryModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a category with the specified id
            var category = await CategoryService.GetCategoryByIdAsync(model.Id);
            if (category == null || category.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = category.PictureId;

                //if parent category changes, we need to clear cache for previous parent category
                if (category.ParentCategoryId != model.ParentCategoryId)
                {
                    await StaticCacheManager.RemoveByPrefixAsync(NopCatalogDefaults.CategoriesByParentCategoryPrefix, category.ParentCategoryId);
                    await StaticCacheManager.RemoveByPrefixAsync(NopCatalogDefaults.CategoriesChildIdsPrefix, category.ParentCategoryId);
                }

                category = model.ToEntity(category);
                category.UpdatedOnUtc = DateTime.UtcNow;
                await CategoryService.UpdateCategoryAsync(category);

                //search engine name
                model.SeName = await UrlRecordService.ValidateSeNameAsync(category, model.SeName, category.Name, true);
                await UrlRecordService.SaveSlugAsync(category, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(category, model);

                //discounts
                var allDiscounts = await DiscountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    {
                        //new discount
                        if (await CategoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id) is null)
                            await CategoryService.InsertDiscountCategoryMappingAsync(new DiscountCategoryMapping { DiscountId = discount.Id, EntityId = category.Id });
                    }
                    else
                    {
                        //remove discount
                        if (await CategoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id) is DiscountCategoryMapping mapping)
                            await CategoryService.DeleteDiscountCategoryMappingAsync(mapping);
                    }
                }

                await CategoryService.UpdateCategoryAsync(category);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != category.PictureId)
                {
                    var prevPicture = await PictureService.GetPictureByIdAsync(prevPictureId);
                    if (prevPicture != null)
                        await PictureService.DeletePictureAsync(prevPicture);
                }

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(category);

                //ACL
                await SaveCategoryAclAsync(category, model);

                //stores
                await SaveStoreMappingsAsync(category, model);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditCategory",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditCategory"), category.Name), category);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Categories.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = category.Id });
            }

            //prepare model
            model = await CategoryModelFactory.PrepareCategoryModelAsync(model, category, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a category with the specified id
            var category = await CategoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return RedirectToAction("List");

            await CategoryService.DeleteCategoryAsync(category);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteCategory",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteCategory"), category.Name), category);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Categories.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            await CategoryService.DeleteCategoriesAsync(await (await CategoryService.GetCategoriesByIdsAsync(selectedIds.ToArray())).WhereAwait(async p => await WorkContext.GetCurrentVendorAsync() == null).ToListAsync());

            return Json(new { Result = true });
        }

        #endregion

        #region Export / Import

        public virtual async Task<IActionResult> ExportXml()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            try
            {
                var xml = await ExportManager.ExportCategoriesToXmlAsync();

                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "categories.xml");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        public virtual async Task<IActionResult> ExportXlsx()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            try
            {
                var bytes = await ExportManager
                    .ExportCategoriesToXlsxAsync((await CategoryService.GetAllCategoriesAsync(showHidden: true)).ToList());

                return File(bytes, MimeTypes.TextXlsx, "categories.xlsx");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportFromXlsx(IFormFile importexcelfile)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //a vendor cannot import categories
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return AccessDeniedView();

            try
            {
                if (importexcelfile != null && importexcelfile.Length > 0)
                {
                    await ImportManager.ImportCategoriesFromXlsxAsync(importexcelfile.OpenReadStream());
                }
                else
                {
                    NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Categories.Imported"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        #endregion

        #region Products

        [HttpPost]
        public virtual async Task<IActionResult> ProductList(CategoryProductSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            //try to get a category with the specified id
            var category = await CategoryService.GetCategoryByIdAsync(searchModel.CategoryId)
                ?? throw new ArgumentException("No category found with the specified id");

            //prepare model
            var model = await CategoryModelFactory.PrepareCategoryProductListModelAsync(searchModel, category);

            return Json(model);
        }

        public virtual async Task<IActionResult> ProductUpdate(CategoryProductModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a product category with the specified id
            var productCategory = await CategoryService.GetProductCategoryByIdAsync(model.Id)
                ?? throw new ArgumentException("No product category mapping found with the specified id");

            //fill entity from product
            productCategory = model.ToEntity(productCategory);
            await CategoryService.UpdateProductCategoryAsync(productCategory);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> ProductDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a product category with the specified id
            var productCategory = await CategoryService.GetProductCategoryByIdAsync(id)
                ?? throw new ArgumentException("No product category mapping found with the specified id", nameof(id));

            await CategoryService.DeleteProductCategoryAsync(productCategory);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> ProductAddPopup(int categoryId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //prepare model
            var model = await CategoryModelFactory.PrepareAddProductToCategorySearchModelAsync(new AddProductToCategorySearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAddPopupList(AddProductToCategorySearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await CategoryModelFactory.PrepareAddProductToCategoryListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> ProductAddPopup(AddProductToCategoryModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //get selected products
            var selectedProducts = await ProductService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                var existingProductCategories = await CategoryService.GetProductCategoriesByCategoryIdAsync(model.CategoryId, showHidden: true);
                foreach (var product in selectedProducts)
                {
                    //whether product category with such parameters already exists
                    if (CategoryService.FindProductCategory(existingProductCategories, product.Id, model.CategoryId) != null)
                        continue;

                    //insert the new product category mapping
                    await CategoryService.InsertProductCategoryAsync(new ProductCategory
                    {
                        CategoryId = model.CategoryId,
                        ProductId = product.Id,
                        IsFeaturedProduct = false,
                        DisplayOrder = 1
                    });
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddProductToCategorySearchModel());
        }

        #endregion
    }
}