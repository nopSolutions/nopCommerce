using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.PriceLists;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.PriceLists;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.PriceLists;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class PriceListController : BaseAdminController
{
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IExportManager _exportManager;
    protected readonly IImportManager _importManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPriceListModelFactory _priceListModelFactory;
    protected readonly IPriceListService _priceListService;
    protected readonly IProductService _productService;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public PriceListController(ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IExportManager exportManager,
        IImportManager importManager,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPriceListModelFactory priceListModelFactory,
        IPriceListService priceListService,
        IProductService productService)
    {
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _exportManager = exportManager;
        _importManager = importManager;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _priceListModelFactory = priceListModelFactory;
        _priceListService = priceListService;
        _productService = productService;
    }

    #endregion

    #region Methods

    #region List

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
        //prepare model
        var model = await _priceListModelFactory.PreparePriceListSearchModelAsync(new PriceListSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_VIEW)]
    public virtual async Task<IActionResult> PriceListList(PriceListSearchModel searchModel)
    {
        //prepare model
        var model = await _priceListModelFactory.PreparePriceListListModelAsync(searchModel);

        return Json(model);
    }

    #endregion

    #region Create / Edit / Delete

    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _priceListModelFactory.PreparePriceListModelAsync(new PriceListModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(PriceListModel model, bool continueEditing)
    {
        //validate customer roles
        var allCustomerRoles = await _customerService.GetAllCustomerRolesAsync(true);
        var newCustomerRoles = new List<CustomerRole>();
        foreach (var customerRole in allCustomerRoles)
        {
            if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                newCustomerRoles.Add(customerRole);
        }

        if (ModelState.IsValid)
        {
            var priceList = model.ToEntity<PriceList>();

            await _priceListService.InsertPriceListAsync(priceList);

            //customer roles
            foreach (var customerRole in newCustomerRoles)
            {
                await _priceListService.AddCustomerRoleMappingAsync(new PriceListCustomerRole { PriceListId = priceList.Id, CustomerRoleId = customerRole.Id });
            }

            await _priceListService.UpdatePriceListAsync(priceList);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewPriceList",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewPriceList"), priceList.Id), priceList);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.PriceLists.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = priceList.Id });
        }

        //prepare model
        model = await _priceListModelFactory.PreparePriceListModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        //try to get a price list with the specified id
        var priceList = await _priceListService.GetPriceListByIdAsync(id);
        if (priceList == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _priceListModelFactory.PreparePriceListModelAsync(null, priceList);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(PriceListModel model, bool continueEditing)
    {
        //try to get a price list with the specified id
        var priceList = await _priceListService.GetPriceListByIdAsync(model.Id);
        if (priceList == null)
            return RedirectToAction("List");

        //validate customer roles
        var allCustomerRoles = await _customerService.GetAllCustomerRolesAsync(true);
        var newCustomerRoles = new List<CustomerRole>();
        foreach (var customerRole in allCustomerRoles)
        {
            if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                newCustomerRoles.Add(customerRole);
        }

        if (ModelState.IsValid)
        {
            priceList = model.ToEntity(priceList);

            var currentCustomerRoleIds = await _priceListService.GetCustomerRoleIdsAsync(priceList);

            //customer roles
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (currentCustomerRoleIds.All(roleId => roleId != customerRole.Id))
                        await _priceListService.AddCustomerRoleMappingAsync(new PriceListCustomerRole { PriceListId = priceList.Id, CustomerRoleId = customerRole.Id });
                }
                else
                {
                    //remove role
                    if (currentCustomerRoleIds.Any(roleId => roleId == customerRole.Id))
                        await _priceListService.RemoveCustomerRoleMappingAsync(priceList, customerRole);
                }
            }

            await _priceListService.UpdatePriceListAsync(priceList);


            //activity log
            await _customerActivityService.InsertActivityAsync("EditPriceList",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditPriceList"), priceList.Id), priceList);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.PriceLists.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = priceList.Id });
        }

        //prepare model
        model = await _priceListModelFactory.PreparePriceListModelAsync(model, priceList, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        //try to get a price list with the specified id
        var priceList = await _priceListService.GetPriceListByIdAsync(id);
        if (priceList == null)
            return RedirectToAction("List");

        await _priceListService.DeletePriceListAsync(priceList);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeletePriceList",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeletePriceList"), priceList.Id), priceList);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.PriceLists.Deleted"));

        return RedirectToAction("List");
    }

    #endregion

    #region Products mapping

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductList(PriceListItemSearchModel searchModel)
    {
        //try to get a price list with the specified id
        var priceList = await _priceListService.GetPriceListByIdAsync(searchModel.PriceListId)
            ?? throw new ArgumentException("No price list found with the specified id");

        //prepare model
        var model = await _priceListModelFactory.PreparePriceListItemListModelAsync(searchModel, priceList);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductDelete(int id)
    {
        //try to get a price list item with the specified id
        var priceListItem = await _priceListService.GetPriceListItemByIdAsync(id)
            ?? throw new ArgumentException("No price list item found with the specified id", nameof(id));

        await _priceListService.DeletePriceListItemAsync(priceListItem);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_CREATE_EDIT_DELETE)]
    public async Task<IActionResult> ProductUpdate(PriceListItemModel model)
    {
        //try to get a price list item with the specified id
        var priceListItem = await _priceListService.GetPriceListItemByIdAsync(model.Id)
            ?? throw new ArgumentException("No price list item found with the specified id", nameof(model.Id));

        //fill entity from model
        priceListItem.ManualPrice = model.ManualPrice;

        await _priceListService.UpdatePriceListItemAsync(priceListItem);

        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopup(int priceListId)
    {
        //prepare model
        var model = await _priceListModelFactory.PrepareAddProductToPriceListSearchModelAsync(new AddProductToPriceListSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopupList(AddProductToPriceListSearchModel searchModel)
    {
        //prepare model
        var model = await _priceListModelFactory.PrepareAddProductToPriceListListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopup(AddProductToPriceListModel model)
    {
        //get selected products
        var selectedProducts = await _productService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
        if (selectedProducts.Any())
        {
            var existingPriceListItems = await _priceListService.GetPriceListItemsByPriceListIdAsync(model.PriceListId);
            foreach (var product in selectedProducts)
            {
                //whether price list item with such parameters already exists
                if (existingPriceListItems.FirstOrDefault(pc => pc.ProductId == product.Id && pc.PriceListId == model.PriceListId) != null)
                    continue;

                //insert the new price list item
                await _priceListService.InsertPriceListItemAsync(new PriceListItem
                {
                    PriceListId = model.PriceListId,
                    ProductId = product.Id
                });
            }
        }

        ViewBag.RefreshPage = true;

        return View(new AddProductToPriceListSearchModel());
    }

    #endregion

    #region Customer mapping

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CustomerList(PriceListCustomerSearchModel searchModel)
    {
        //try to get a price list with the specified id
        var priceList = await _priceListService.GetPriceListByIdAsync(searchModel.PriceListId)
            ?? throw new ArgumentException("No price list found with the specified id");

        //prepare model
        var model = await _priceListModelFactory.PreparePriceListCustomerListModelAsync(searchModel, priceList);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CustomerDelete(int id)
    {
        //try to get a price list item with the specified id
        var priceListCustomer = await _priceListService.GetPriceListCustomerByIdAsync(id)
            ?? throw new ArgumentException("No price list item found with the specified id", nameof(id));

        await _priceListService.DeletePriceListCustomerAsync(priceListCustomer);

        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CustomerAddPopup(int priceListId)
    {
        //prepare model
        var model = await _priceListModelFactory.PrepareAddCustomerToPriceListSearchModelAsync(new AddCustomerToPriceListSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CustomerAddPopupList(AddCustomerToPriceListSearchModel searchModel)
    {
        //prepare model
        var model = await _priceListModelFactory.PrepareAddCustomerToPriceListListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CustomerAddPopup(AddCustomerToPriceListModel model)
    {
        //get selected customers
        var selectedCustomers = await _customerService.GetCustomersByIdsAsync(model.SelectedCustomerIds.ToArray());
        if (selectedCustomers.Any())
        {
            var existingPriceListCustomers = await _priceListService.GetPriceListCustomersByPriceListIdAsync(model.PriceListId);
            foreach (var customer in selectedCustomers)
            {
                //whether price list item with such parameters already exists
                if (existingPriceListCustomers.FirstOrDefault(pc => pc.CustomerId == customer.Id && pc.PriceListId == model.PriceListId) != null)
                    continue;

                //insert the new price list item
                await _priceListService.InsertPriceListCustomerAsync(new PriceListCustomer
                {
                    PriceListId = model.PriceListId,
                    CustomerId = customer.Id
                });
            }
        }

        ViewBag.RefreshPage = true;

        return View(new AddCustomerToPriceListSearchModel());
    }

    #endregion

    #region Export / Import

    [HttpPost, ActionName("ExportExcel")]
    [FormValueRequired("exportexcel-all")]
    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportExcelAll(PriceListSearchModel model)
    {
        var customerRoleIds = model.SelectedCustomerRoleIds != null && !model.SelectedCustomerRoleIds.Contains(0)
            ? model.SelectedCustomerRoleIds.ToArray()
            : null;

        //load price lists
        var priceLists = await _priceListService.GetAllPriceListsAsync(customerRoleIds: customerRoleIds,
            customerIds: null,
            isActive: model.SearchIsActive);

        try
        {
            var bytes = await _exportManager.ExportPriceListsToXlsxAsync(priceLists);

            return File(bytes, MimeTypes.TextXlsx, "priceLists.xlsx");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);

            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportExcelSelected(string selectedIds)
    {
        var priceLists = new List<PriceList>();
        if (selectedIds != null)
        {
            var ids = selectedIds
                .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Convert.ToInt32(x))
                .ToArray();
            priceLists.AddRange(await _priceListService.GetPriceListsByIdsAsync(ids));
        }

        try
        {
            var bytes = await _exportManager.ExportPriceListsToXlsxAsync(priceLists);

            return File(bytes, MimeTypes.TextXlsx, "priceLists.xlsx");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRICE_LISTS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ImportExcel(IFormFile importexcelfile)
    {
        try
        {
            if (importexcelfile != null && importexcelfile.Length > 0)
            {
                await _importManager.ImportPriceListsFromXlsxAsync(importexcelfile.OpenReadStream());
            }
            else
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));

                return RedirectToAction("List");
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.PriceLists.Imported"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);

            return RedirectToAction("List");
        }
    }

    #endregion

    #endregion
}
