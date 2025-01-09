using System.Text;
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

namespace Nop.Web.Areas.Admin.Controllers;

public partial class CategoryController : BaseAdminController
{
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly ICategoryModelFactory _categoryModelFactory;
    protected readonly ICategoryService _categoryService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IDiscountService _discountService;
    protected readonly IExportManager _exportManager;
    protected readonly IImportManager _importManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPictureService _pictureService;
    protected readonly IProductService _productService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IStoreService _storeService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;

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
        _aclService = aclService;
        _categoryModelFactory = categoryModelFactory;
        _categoryService = categoryService;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _discountService = discountService;
        _exportManager = exportManager;
        _importManager = importManager;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _pictureService = pictureService;
        _productService = productService;
        _staticCacheManager = staticCacheManager;
        _storeMappingService = storeMappingService;
        _storeService = storeService;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateLocalesAsync(Category category, CategoryModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(category,
                x => x.Name,
                localized.Name,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(category,
                x => x.Description,
                localized.Description,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(category,
                x => x.MetaKeywords,
                localized.MetaKeywords,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(category,
                x => x.MetaDescription,
                localized.MetaDescription,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(category,
                x => x.MetaTitle,
                localized.MetaTitle,
                localized.LanguageId);

            //search engine name
            var seName = await _urlRecordService.ValidateSeNameAsync(category, localized.SeName, localized.Name, false);
            await _urlRecordService.SaveSlugAsync(category, seName, localized.LanguageId);
        }
    }

    protected virtual async Task UpdatePictureSeoNamesAsync(Category category)
    {
        var picture = await _pictureService.GetPictureByIdAsync(category.PictureId);
        if (picture != null)
            await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(category.Name));
    }
    
    protected virtual async Task SaveStoreMappingsAsync(Category category, CategoryModel model)
    {
        category.LimitedToStores = model.SelectedStoreIds.Any();
        await _categoryService.UpdateCategoryAsync(category);

        var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(category);
        var allStores = await _storeService.GetAllStoresAsync();
        foreach (var store in allStores)
        {
            if (model.SelectedStoreIds.Contains(store.Id))
            {
                //new store
                if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                    await _storeMappingService.InsertStoreMappingAsync(category, store.Id);
            }
            else
            {
                //remove store
                var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                if (storeMappingToDelete != null)
                    await _storeMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
            }
        }
    }

    #endregion

    #region List

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Catalog.CATEGORIES_VIEW)]
    public virtual async Task<IActionResult> List()
    {
        //prepare model
        var model = await _categoryModelFactory.PrepareCategorySearchModelAsync(new CategorySearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_VIEW)]
    public virtual async Task<IActionResult> List(CategorySearchModel searchModel)
    {
        //prepare model
        var model = await _categoryModelFactory.PrepareCategoryListModelAsync(searchModel);

        return Json(model);
    }

    #endregion

    #region Create / Edit / Delete

    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _categoryModelFactory.PrepareCategoryModelAsync(new CategoryModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(CategoryModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var category = model.ToEntity<Category>();
            category.CreatedOnUtc = DateTime.UtcNow;
            category.UpdatedOnUtc = DateTime.UtcNow;
            await _categoryService.InsertCategoryAsync(category);

            //search engine name
            model.SeName = await _urlRecordService.ValidateSeNameAsync(category, model.SeName, category.Name, true);
            await _urlRecordService.SaveSlugAsync(category, model.SeName, 0);

            //locales
            await UpdateLocalesAsync(category, model);

            //discounts
            var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories, showHidden: true, isActive: null);
            foreach (var discount in allDiscounts)
            {
                if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    await _categoryService.InsertDiscountCategoryMappingAsync(new DiscountCategoryMapping { DiscountId = discount.Id, EntityId = category.Id });
            }

            await _categoryService.UpdateCategoryAsync(category);

            //update picture seo file name
            await UpdatePictureSeoNamesAsync(category);
            
            //stores
            await SaveStoreMappingsAsync(category, model);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewCategory",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCategory"), category.Name), category);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = category.Id });
        }

        //prepare model
        model = await _categoryModelFactory.PrepareCategoryModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.CATEGORIES_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        //try to get a category with the specified id
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null || category.Deleted)
            return RedirectToAction("List");

        //prepare model
        var model = await _categoryModelFactory.PrepareCategoryModelAsync(null, category);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(CategoryModel model, bool continueEditing)
    {
        //try to get a category with the specified id
        var category = await _categoryService.GetCategoryByIdAsync(model.Id);
        if (category == null || category.Deleted)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            var prevPictureId = category.PictureId;

            //if parent category changes, we need to clear cache for previous parent category
            if (category.ParentCategoryId != model.ParentCategoryId)
            {
                await _staticCacheManager.RemoveByPrefixAsync(NopCatalogDefaults.CategoriesByParentCategoryPrefix, category.ParentCategoryId);
                await _staticCacheManager.RemoveByPrefixAsync(NopCatalogDefaults.CategoriesChildIdsPrefix, category.ParentCategoryId);
                await _staticCacheManager.RemoveByPrefixAsync(NopCatalogDefaults.ChildCategoryIdLookupPrefix);
            }

            category = model.ToEntity(category);
            category.UpdatedOnUtc = DateTime.UtcNow;
            await _categoryService.UpdateCategoryAsync(category);

            //search engine name
            model.SeName = await _urlRecordService.ValidateSeNameAsync(category, model.SeName, category.Name, true);
            await _urlRecordService.SaveSlugAsync(category, model.SeName, 0);

            //locales
            await UpdateLocalesAsync(category, model);

            //discounts
            var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories, showHidden: true, isActive: null);
            foreach (var discount in allDiscounts)
            {
                if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                {
                    //new discount
                    if (await _categoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id) is null)
                        await _categoryService.InsertDiscountCategoryMappingAsync(new DiscountCategoryMapping { DiscountId = discount.Id, EntityId = category.Id });
                }
                else
                {
                    //remove discount
                    if (await _categoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id) is DiscountCategoryMapping mapping)
                        await _categoryService.DeleteDiscountCategoryMappingAsync(mapping);
                }
            }

            await _categoryService.UpdateCategoryAsync(category);

            //delete an old picture (if deleted or updated)
            if (prevPictureId > 0 && prevPictureId != category.PictureId)
            {
                var prevPicture = await _pictureService.GetPictureByIdAsync(prevPictureId);
                if (prevPicture != null)
                    await _pictureService.DeletePictureAsync(prevPicture);
            }

            //update picture seo file name
            await UpdatePictureSeoNamesAsync(category);
            
            //stores
            await SaveStoreMappingsAsync(category, model);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditCategory",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCategory"), category.Name), category);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = category.Id });
        }

        //prepare model
        model = await _categoryModelFactory.PrepareCategoryModelAsync(model, category, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        //try to get a category with the specified id
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
            return RedirectToAction("List");

        await _categoryService.DeleteCategoryAsync(category);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteCategory",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCategory"), category.Name), category);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Deleted"));

        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var categories = await _categoryService.GetCategoriesByIdsAsync(selectedIds.ToArray());

        await _categoryService.DeleteCategoriesAsync(categories);

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.DeleteCategory");

        foreach (var category in categories)
            await _customerActivityService.InsertActivityAsync("DeleteCategory",
                string.Format(activityLogFormat, category.Name), category);

        return Json(new { Result = true });
    }

    #endregion

    #region Export / Import

    [CheckPermission(StandardPermission.Catalog.CATEGORIES_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXml()
    {
        try
        {
            var xml = await _exportManager.ExportCategoriesToXmlAsync();

            return File(Encoding.UTF8.GetBytes(xml), "application/xml", "categories.xml");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }

    [CheckPermission(StandardPermission.Catalog.CATEGORIES_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXlsx()
    {
        try
        {
            var bytes = await _exportManager
                .ExportCategoriesToXlsxAsync((await _categoryService.GetAllCategoriesAsync(showHidden: true)).ToList());

            return File(bytes, MimeTypes.TextXlsx, "categories.xlsx");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ImportFromXlsx(IFormFile importexcelfile)
    {
        //a vendor cannot import categories
        if (await _workContext.GetCurrentVendorAsync() != null)
            return AccessDeniedView();

        try
        {
            if (importexcelfile != null && importexcelfile.Length > 0)
            {
                await _importManager.ImportCategoriesFromXlsxAsync(importexcelfile.OpenReadStream());
            }
            else
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));
                return RedirectToAction("List");
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Imported"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }

    #endregion

    #region Products

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_VIEW)]
    public virtual async Task<IActionResult> ProductList(CategoryProductSearchModel searchModel)
    {
        //try to get a category with the specified id
        var category = await _categoryService.GetCategoryByIdAsync(searchModel.CategoryId)
            ?? throw new ArgumentException("No category found with the specified id");

        //prepare model
        var model = await _categoryModelFactory.PrepareCategoryProductListModelAsync(searchModel, category);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductUpdate(CategoryProductModel model)
    {
        //try to get a product category with the specified id
        var productCategory = await _categoryService.GetProductCategoryByIdAsync(model.Id)
            ?? throw new ArgumentException("No product category mapping found with the specified id");

        //fill entity from product
        productCategory = model.ToEntity(productCategory);
        await _categoryService.UpdateProductCategoryAsync(productCategory);

        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductDelete(int id)
    {
        //try to get a product category with the specified id
        var productCategory = await _categoryService.GetProductCategoryByIdAsync(id)
            ?? throw new ArgumentException("No product category mapping found with the specified id", nameof(id));

        await _categoryService.DeleteProductCategoryAsync(productCategory);

        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopup(int categoryId)
    {
        //prepare model
        var model = await _categoryModelFactory.PrepareAddProductToCategorySearchModelAsync(new AddProductToCategorySearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopupList(AddProductToCategorySearchModel searchModel)
    {
        //prepare model
        var model = await _categoryModelFactory.PrepareAddProductToCategoryListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopup(AddProductToCategoryModel model)
    {
        //get selected products
        var selectedProducts = await _productService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
        if (selectedProducts.Any())
        {
            var existingProductCategories = await _categoryService.GetProductCategoriesByCategoryIdAsync(model.CategoryId, showHidden: true);
            foreach (var product in selectedProducts)
            {
                //whether product category with such parameters already exists
                if (_categoryService.FindProductCategory(existingProductCategories, product.Id, model.CategoryId) != null)
                    continue;

                //insert the new product category mapping
                await _categoryService.InsertProductCategoryAsync(new ProductCategory
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