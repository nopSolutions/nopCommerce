using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Tax.Avalara.Models.ItemClassification;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Tax.Avalara.Controllers;

public class ItemClassificationController : BaseAdminController
{
    #region Fields

    protected readonly AvalaraTaxManager _avalaraTaxManager;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICountryService _countryService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IProductService _productService;
    protected readonly ITaxPluginManager _taxPluginManager;
    protected readonly ItemClassificationService _itemClassificationService;

    #endregion

    #region Ctor

    public ItemClassificationController(AvalaraTaxManager avalaraTaxManager,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICountryService countryService,
        IDateTimeHelper dateTimeHelper,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IProductService productService,
        ITaxPluginManager taxPluginManager,
        ItemClassificationService itemClassificationService)
    {
        _avalaraTaxManager = avalaraTaxManager;
        _baseAdminModelFactory = baseAdminModelFactory;
        _countryService = countryService;
        _dateTimeHelper = dateTimeHelper;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _productService = productService;
        _taxPluginManager = taxPluginManager;
        _itemClassificationService = itemClassificationService;
    }

    #endregion

    #region Methods

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> List(ItemClassificationSearchModel searchModel)
    {
        //get item classification
        var itemClassification = await _itemClassificationService.GetItemClassificationAsync(
            pageIndex: searchModel.Page - 1,
            pageSize: searchModel.PageSize,
            countryId: searchModel.SearchCountryId,
            productId: null);

        //prepare grid model
        var model = await new ItemClassificationListModel().PrepareToGridAsync(searchModel, itemClassification, () =>
        {
            return itemClassification.SelectAwait(async item => new ItemClassificationModel
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = (await _productService.GetProductByIdAsync(item.ProductId))?.Name ?? "",
                HSClassificationRequestId = item.HSClassificationRequestId,
                CountryId = item.CountryId,
                CountryName = (await _countryService.GetCountryByIdAsync(item.CountryId))?.Name ?? "*",
                HSCode = item.HSCode,
                UpdatedDate = await _dateTimeHelper.ConvertToUserTimeAsync(item.UpdatedOnUtc, DateTimeKind.Utc)
            });
        });

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> Update(ItemClassificationModel model)
    {
        var item = await _itemClassificationService.GetItemClassificationByIdAsync(model.Id)
            ?? throw new ArgumentException("No record found");

        item.HSCode = model.HSCode;

        await _itemClassificationService.UpdateItemClassificationAsync(item);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> DeleteSelected(List<int> selectedIds)
    {
        if (!selectedIds?.Any() ?? true)
            return NoContent();

        var recordsToDelete = new List<int>();

        foreach (var id in selectedIds)
        {
            var item = await _itemClassificationService.GetItemClassificationByIdAsync(id);
            if (item is null)
                continue;

            recordsToDelete.Add(item.Id);
        }
        await _itemClassificationService.DeleteItemsAsync(recordsToDelete);

        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> ClearAll()
    {
        await _itemClassificationService.ClearItemClassificationAsync();

        return Json(new { Result = true });
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> ProductToClassification()
    {
        var model = new AddProductToClassificationSearchModel();
        await _baseAdminModelFactory.PrepareProductTypesAsync(model.AvailableProductTypes);
        await _baseAdminModelFactory.PrepareCategoriesAsync(model.AvailableCategories);
        await _baseAdminModelFactory.PrepareManufacturersAsync(model.AvailableManufacturers);
        await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores);
        await _baseAdminModelFactory.PrepareVendorsAsync(model.AvailableVendors);
        model.SetPopupGridPageSize();

        return View("~/Plugins/Tax.Avalara/Views/ItemClassification/ProductToClassification.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> ProductListToClassification(AddProductToClassificationSearchModel searchModel)
    {
        var products = await _productService.SearchProductsAsync(showHidden: true,
            keywords: searchModel.SearchProductName,
            productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
            categoryIds: new List<int> { searchModel.SearchCategoryId },
            manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
            storeId: searchModel.SearchStoreId,
            vendorId: searchModel.SearchVendorId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        var model = new AddProductToClassificationListModel().PrepareToGrid(searchModel, products, () =>
        {
            return products.Select(product => new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Sku = product.Sku,
                Price = product.Price,
                Published = product.Published
            }).ToList();
        });

        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> ProductToClassification(AddProductToClassificationModel model)
    {
        await _itemClassificationService.AddItemClassificationAsync(model.SelectedProductIds?.ToList());

        ViewBag.RefreshPage = true;

        return View("~/Plugins/Tax.Avalara/Views/ItemClassification/ProductToClassification.cshtml", new AddProductToClassificationSearchModel());
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> StartClassification()
    {
        var items = (await _itemClassificationService.GetItemClassificationAsync())
            .Where(x => string.IsNullOrEmpty(x.HSClassificationRequestId))
            .ToList();
        foreach (var item in items)
        {
            var (classification, error) = await _avalaraTaxManager.ClassificationProductsAsync(item);

            if (!string.IsNullOrEmpty(error))
                return Json(new { success = false, message = error });

            if (!string.IsNullOrEmpty(classification?.Id))
            {
                //save classification id for future use (get hsCode)
                item.HSClassificationRequestId = classification.Id;
                item.UpdatedOnUtc = DateTime.UtcNow;
                await _itemClassificationService.UpdateItemClassificationAsync(item);
            }
        }

        return Json(new { success = true, message = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.ItemClassification.Sync.Success") });
    }

    #endregion
}