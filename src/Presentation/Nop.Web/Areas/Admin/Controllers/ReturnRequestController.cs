using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ReturnRequestController : BaseAdminController
{
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IOrderService _orderService;
    protected readonly IProductService _productService;
    protected readonly IPermissionService _permissionService;
    protected readonly IReturnRequestModelFactory _returnRequestModelFactory;
    protected readonly IReturnRequestService _returnRequestService;
    protected readonly IWorkflowMessageService _workflowMessageService;

    #endregion Fields

    #region Ctor

    public ReturnRequestController(ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        IOrderService orderService,
        IProductService productService,
        IPermissionService permissionService,
        IReturnRequestModelFactory returnRequestModelFactory,
        IReturnRequestService returnRequestService,
        IWorkflowMessageService workflowMessageService)
    {
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _notificationService = notificationService;
        _orderService = orderService;
        _productService = productService;
        _permissionService = permissionService;
        _returnRequestModelFactory = returnRequestModelFactory;
        _returnRequestService = returnRequestService;
        _workflowMessageService = workflowMessageService;
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateLocalesAsync(ReturnRequestReason rrr, ReturnRequestReasonModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(rrr,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
    }

    protected virtual async Task UpdateLocalesAsync(ReturnRequestAction rra, ReturnRequestActionModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(rra,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Orders.RETURN_REQUESTS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
        //prepare model
        var model = await _returnRequestModelFactory.PrepareReturnRequestSearchModelAsync(new ReturnRequestSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.RETURN_REQUESTS_VIEW)]
    public virtual async Task<IActionResult> List(ReturnRequestSearchModel searchModel)
    {
        //prepare model
        var model = await _returnRequestModelFactory.PrepareReturnRequestListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Orders.RETURN_REQUESTS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        //try to get a return request with the specified id
        var returnRequest = await _returnRequestService.GetReturnRequestByIdAsync(id);
        if (returnRequest == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _returnRequestModelFactory.PrepareReturnRequestModelAsync(null, returnRequest);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.Orders.RETURN_REQUESTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(ReturnRequestModel model, bool continueEditing)
    {
        //try to get a return request with the specified id
        var returnRequest = await _returnRequestService.GetReturnRequestByIdAsync(model.Id);
        if (returnRequest == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            var quantityToReturn = model.ReturnedQuantity - returnRequest.ReturnedQuantity;
            if (quantityToReturn < 0)
                _notificationService.ErrorNotification(string.Format(await _localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.ReturnedQuantity.CannotBeLessThanQuantityAlreadyReturned"), returnRequest.ReturnedQuantity));
            else
            {
                if (quantityToReturn > 0)
                {
                    var orderItem = await _orderService.GetOrderItemByIdAsync(returnRequest.OrderItemId);
                    if (orderItem != null)
                    {
                        var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
                        if (product != null)
                        {
                            var productStockChangedMessage = string.Format(await _localizationService.GetResourceAsync("Admin.ReturnRequests.QuantityReturnedToStock"), quantityToReturn);

                            await _productService.AdjustInventoryAsync(product, quantityToReturn, orderItem.AttributesXml, productStockChangedMessage);

                            _notificationService.SuccessNotification(productStockChangedMessage);
                        }
                    }
                }

                returnRequest = model.ToEntity(returnRequest);
                returnRequest.UpdatedOnUtc = DateTime.UtcNow;

                await _returnRequestService.UpdateReturnRequestAsync(returnRequest);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditReturnRequest",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditReturnRequest"), returnRequest.Id), returnRequest);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ReturnRequests.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = returnRequest.Id }) : RedirectToAction("List");
            }
        }

        //prepare model
        model = await _returnRequestModelFactory.PrepareReturnRequestModelAsync(model, returnRequest, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("notify-customer")]
    [CheckPermission(StandardPermission.Orders.RETURN_REQUESTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> NotifyCustomer(ReturnRequestModel model)
    {
        //try to get a return request with the specified id
        var returnRequest = await _returnRequestService.GetReturnRequestByIdAsync(model.Id);
        if (returnRequest == null)
            return RedirectToAction("List");

        var orderItem = await _orderService.GetOrderItemByIdAsync(returnRequest.OrderItemId);
        if (orderItem is null)
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.ReturnRequests.OrderItemDeleted"));
            return RedirectToAction("Edit", new { id = returnRequest.Id });
        }

        var order = await _orderService.GetOrderByIdAsync(orderItem.OrderId);

        var queuedEmailIds = await _workflowMessageService.SendReturnRequestStatusChangedCustomerNotificationAsync(returnRequest, orderItem, order);
        if (queuedEmailIds.Any())
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ReturnRequests.Notified"));

        return RedirectToAction("Edit", new { id = returnRequest.Id });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.RETURN_REQUESTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        //try to get a return request with the specified id
        var returnRequest = await _returnRequestService.GetReturnRequestByIdAsync(id);
        if (returnRequest == null)
            return RedirectToAction("List");

        await _returnRequestService.DeleteReturnRequestAsync(returnRequest);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteReturnRequest",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteReturnRequest"), returnRequest.Id), returnRequest);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ReturnRequests.Deleted"));

        return RedirectToAction("List");
    }

    #region Return request reasons

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual IActionResult ReturnRequestReasonList()
    {
        //select an appropriate card
        SaveSelectedCardName("ordersettings-return-request");

        //we just redirect a user to the order settings page
        return RedirectToAction("Order", "Setting");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ReturnRequestReasonList(ReturnRequestReasonSearchModel searchModel)
    {
        //prepare model
        var model = await _returnRequestModelFactory.PrepareReturnRequestReasonListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ReturnRequestReasonCreate()
    {
        //prepare model
        var model = await _returnRequestModelFactory.PrepareReturnRequestReasonModelAsync(new ReturnRequestReasonModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ReturnRequestReasonCreate(ReturnRequestReasonModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var returnRequestReason = model.ToEntity<ReturnRequestReason>();
            await _returnRequestService.InsertReturnRequestReasonAsync(returnRequestReason);

            //locales
            await UpdateLocalesAsync(returnRequestReason, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Order.ReturnRequestReasons.Added"));

            return continueEditing
                ? RedirectToAction("ReturnRequestReasonEdit", new { id = returnRequestReason.Id })
                : RedirectToAction("ReturnRequestReasonList");
        }

        //prepare model
        model = await _returnRequestModelFactory.PrepareReturnRequestReasonModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ReturnRequestReasonEdit(int id)
    {
        //try to get a return request reason with the specified id
        var returnRequestReason = await _returnRequestService.GetReturnRequestReasonByIdAsync(id);
        if (returnRequestReason == null)
            return RedirectToAction("ReturnRequestReasonList");

        //prepare model
        var model = await _returnRequestModelFactory.PrepareReturnRequestReasonModelAsync(null, returnRequestReason);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ReturnRequestReasonEdit(ReturnRequestReasonModel model, bool continueEditing)
    {
        //try to get a return request reason with the specified id
        var returnRequestReason = await _returnRequestService.GetReturnRequestReasonByIdAsync(model.Id);
        if (returnRequestReason == null)
            return RedirectToAction("ReturnRequestReasonList");

        if (ModelState.IsValid)
        {
            returnRequestReason = model.ToEntity(returnRequestReason);
            await _returnRequestService.UpdateReturnRequestReasonAsync(returnRequestReason);

            //locales
            await UpdateLocalesAsync(returnRequestReason, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Order.ReturnRequestReasons.Updated"));

            if (!continueEditing)
                return RedirectToAction("ReturnRequestReasonList");

            return RedirectToAction("ReturnRequestReasonEdit", new { id = returnRequestReason.Id });
        }

        //prepare model
        model = await _returnRequestModelFactory.PrepareReturnRequestReasonModelAsync(model, returnRequestReason, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ReturnRequestReasonDelete(int id)
    {
        //try to get a return request reason with the specified id
        var returnRequestReason = await _returnRequestService.GetReturnRequestReasonByIdAsync(id)
                                  ?? throw new ArgumentException("No return request reason found with the specified id", nameof(id));

        await _returnRequestService.DeleteReturnRequestReasonAsync(returnRequestReason);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Order.ReturnRequestReasons.Deleted"));

        return RedirectToAction("ReturnRequestReasonList");
    }

    #endregion

    #region Return request actions

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual IActionResult ReturnRequestActionList()
    {
        //select an appropriate card
        SaveSelectedCardName("ordersettings-return-request");

        //we just redirect a user to the order settings page
        return RedirectToAction("Order", "Setting");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ReturnRequestActionList(ReturnRequestActionSearchModel searchModel)
    {
        //prepare model
        var model = await _returnRequestModelFactory.PrepareReturnRequestActionListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ReturnRequestActionCreate()
    {
        //prepare model
        var model = await _returnRequestModelFactory.PrepareReturnRequestActionModelAsync(new ReturnRequestActionModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ReturnRequestActionCreate(ReturnRequestActionModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var returnRequestAction = model.ToEntity<ReturnRequestAction>();
            await _returnRequestService.InsertReturnRequestActionAsync(returnRequestAction);

            //locales
            await UpdateLocalesAsync(returnRequestAction, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Order.ReturnRequestActions.Added"));

            return continueEditing
                ? RedirectToAction("ReturnRequestActionEdit", new { id = returnRequestAction.Id })
                : RedirectToAction("ReturnRequestActionList");
        }

        //prepare model
        model = await _returnRequestModelFactory.PrepareReturnRequestActionModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ReturnRequestActionEdit(int id)
    {
        //try to get a return request action with the specified id
        var returnRequestAction = await _returnRequestService.GetReturnRequestActionByIdAsync(id);
        if (returnRequestAction == null)
            return RedirectToAction("ReturnRequestActionList");

        //prepare model
        var model = await _returnRequestModelFactory.PrepareReturnRequestActionModelAsync(null, returnRequestAction);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ReturnRequestActionEdit(ReturnRequestActionModel model, bool continueEditing)
    {
        //try to get a return request action with the specified id
        var returnRequestAction = await _returnRequestService.GetReturnRequestActionByIdAsync(model.Id);
        if (returnRequestAction == null)
            return RedirectToAction("ReturnRequestActionList");

        if (ModelState.IsValid)
        {
            returnRequestAction = model.ToEntity(returnRequestAction);
            await _returnRequestService.UpdateReturnRequestActionAsync(returnRequestAction);

            //locales
            await UpdateLocalesAsync(returnRequestAction, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Order.ReturnRequestActions.Updated"));

            if (!continueEditing)
                return RedirectToAction("ReturnRequestActionList");

            return RedirectToAction("ReturnRequestActionEdit", new { id = returnRequestAction.Id });
        }

        //prepare model
        model = await _returnRequestModelFactory.PrepareReturnRequestActionModelAsync(model, returnRequestAction, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ReturnRequestActionDelete(int id)
    {
        //try to get a return request action with the specified id
        var returnRequestAction = await _returnRequestService.GetReturnRequestActionByIdAsync(id)
                                  ?? throw new ArgumentException("No return request action found with the specified id", nameof(id));

        await _returnRequestService.DeleteReturnRequestActionAsync(returnRequestAction);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Order.ReturnRequestActions.Deleted"));

        return RedirectToAction("ReturnRequestActionList");
    }

    #endregion

    #endregion
}