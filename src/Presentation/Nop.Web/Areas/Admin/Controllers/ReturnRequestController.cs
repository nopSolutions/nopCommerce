using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
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

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ReturnRequestController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly IReturnRequestModelFactory _returnRequestModelFactory;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IWorkflowMessageService _workflowMessageService;

        #endregion Fields

        #region Ctor

        public ReturnRequestController(ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IOrderService orderService,
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
            _permissionService = permissionService;
            _returnRequestModelFactory = returnRequestModelFactory;
            _returnRequestService = returnRequestService;
            _workflowMessageService = workflowMessageService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocales(ReturnRequestReason rrr, ReturnRequestReasonModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValue(rrr,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocales(ReturnRequestAction rra, ReturnRequestActionModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValue(rra,
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

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //prepare model
            var model = await _returnRequestModelFactory.PrepareReturnRequestSearchModel(new ReturnRequestSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(ReturnRequestSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _returnRequestModelFactory.PrepareReturnRequestListModel(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a return request with the specified id
            var returnRequest = await _returnRequestService.GetReturnRequestById(id);
            if (returnRequest == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _returnRequestModelFactory.PrepareReturnRequestModel(null, returnRequest);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(ReturnRequestModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a return request with the specified id
            var returnRequest = await _returnRequestService.GetReturnRequestById(model.Id);
            if (returnRequest == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                returnRequest = model.ToEntity(returnRequest);
                returnRequest.UpdatedOnUtc = DateTime.UtcNow;
                
                await _returnRequestService.UpdateReturnRequest(returnRequest);

                //activity log
                await _customerActivityService.InsertActivity("EditReturnRequest",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditReturnRequest"), returnRequest.Id), returnRequest);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.ReturnRequests.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = returnRequest.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _returnRequestModelFactory.PrepareReturnRequestModel(model, returnRequest, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("notify-customer")]
        public virtual async Task<IActionResult> NotifyCustomer(ReturnRequestModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a return request with the specified id
            var returnRequest = await _returnRequestService.GetReturnRequestById(model.Id);
            if (returnRequest == null)
                return RedirectToAction("List");

            var orderItem = await _orderService.GetOrderItemById(returnRequest.OrderItemId);
            if (orderItem is null)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResource("Admin.ReturnRequests.OrderItemDeleted"));
                return RedirectToAction("Edit", new { id = returnRequest.Id });
            }

            var order = await _orderService.GetOrderById(orderItem.OrderId);

            var queuedEmailIds = await _workflowMessageService.SendReturnRequestStatusChangedCustomerNotification(returnRequest, orderItem, order);
            if (queuedEmailIds.Any())
                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.ReturnRequests.Notified"));

            return RedirectToAction("Edit", new { id = returnRequest.Id });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a return request with the specified id
            var returnRequest = await _returnRequestService.GetReturnRequestById(id);
            if (returnRequest == null)
                return RedirectToAction("List");

            await _returnRequestService.DeleteReturnRequest(returnRequest);

            //activity log
            await _customerActivityService.InsertActivity("DeleteReturnRequest",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteReturnRequest"), returnRequest.Id), returnRequest);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.ReturnRequests.Deleted"));

            return RedirectToAction("List");
        }

        #region Return request reasons

        public virtual async Task<IActionResult> ReturnRequestReasonList()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //select an appropriate panel
            SaveSelectedPanelName("ordersettings-return-request");

            //we just redirect a user to the order settings page
            return RedirectToAction("Order", "Setting");
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReturnRequestReasonList(ReturnRequestReasonSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _returnRequestModelFactory.PrepareReturnRequestReasonListModel(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> ReturnRequestReasonCreate()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _returnRequestModelFactory.PrepareReturnRequestReasonModel(new ReturnRequestReasonModel(), null);
            
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> ReturnRequestReasonCreate(ReturnRequestReasonModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var returnRequestReason = model.ToEntity<ReturnRequestReason>();
                await _returnRequestService.InsertReturnRequestReason(returnRequestReason);

                //locales
                await UpdateLocales(returnRequestReason, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestReasons.Added"));

                return continueEditing 
                    ? RedirectToAction("ReturnRequestReasonEdit", new { id = returnRequestReason.Id })
                    : RedirectToAction("ReturnRequestReasonList");
            }

            //prepare model
            model = await _returnRequestModelFactory.PrepareReturnRequestReasonModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> ReturnRequestReasonEdit(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a return request reason with the specified id
            var returnRequestReason = await _returnRequestService.GetReturnRequestReasonById(id);
            if (returnRequestReason == null)
                return RedirectToAction("ReturnRequestReasonList");

            //prepare model
            var model = await _returnRequestModelFactory.PrepareReturnRequestReasonModel(null, returnRequestReason);
            
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> ReturnRequestReasonEdit(ReturnRequestReasonModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a return request reason with the specified id
            var returnRequestReason = await _returnRequestService.GetReturnRequestReasonById(model.Id);
            if (returnRequestReason == null)
                return RedirectToAction("ReturnRequestReasonList");

            if (ModelState.IsValid)
            {
                returnRequestReason = model.ToEntity(returnRequestReason);
                await _returnRequestService.UpdateReturnRequestReason(returnRequestReason);

                //locales
                await UpdateLocales(returnRequestReason, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestReasons.Updated"));

                if (!continueEditing)
                    return RedirectToAction("ReturnRequestReasonList");
                
                return RedirectToAction("ReturnRequestReasonEdit", new { id = returnRequestReason.Id });
            }

            //prepare model
            model = await _returnRequestModelFactory.PrepareReturnRequestReasonModel(model, returnRequestReason, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReturnRequestReasonDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a return request reason with the specified id
            var returnRequestReason = await _returnRequestService.GetReturnRequestReasonById(id) 
                ?? throw new ArgumentException("No return request reason found with the specified id", nameof(id));

            await _returnRequestService.DeleteReturnRequestReason(returnRequestReason);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestReasons.Deleted"));

            return RedirectToAction("ReturnRequestReasonList");
        }

        #endregion

        #region Return request actions

        public virtual async Task<IActionResult> ReturnRequestActionList()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //select an appropriate panel
            SaveSelectedPanelName("ordersettings-return-request");

            //we just redirect a user to the order settings page
            return RedirectToAction("Order", "Setting");
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReturnRequestActionList(ReturnRequestActionSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _returnRequestModelFactory.PrepareReturnRequestActionListModel(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> ReturnRequestActionCreate()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _returnRequestModelFactory.PrepareReturnRequestActionModel(new ReturnRequestActionModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> ReturnRequestActionCreate(ReturnRequestActionModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var returnRequestAction = model.ToEntity<ReturnRequestAction>();
                await _returnRequestService.InsertReturnRequestAction(returnRequestAction);

                //locales
                await UpdateLocales(returnRequestAction, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestActions.Added"));

                return continueEditing 
                    ? RedirectToAction("ReturnRequestActionEdit", new { id = returnRequestAction.Id }) 
                    : RedirectToAction("ReturnRequestActionList");
            }

            //prepare model
            model = await _returnRequestModelFactory.PrepareReturnRequestActionModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> ReturnRequestActionEdit(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a return request action with the specified id
            var returnRequestAction = await _returnRequestService.GetReturnRequestActionById(id);
            if (returnRequestAction == null)
                return RedirectToAction("ReturnRequestActionList");

            //prepare model
            var model = await _returnRequestModelFactory.PrepareReturnRequestActionModel(null, returnRequestAction);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> ReturnRequestActionEdit(ReturnRequestActionModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a return request action with the specified id
            var returnRequestAction = await _returnRequestService.GetReturnRequestActionById(model.Id);
            if (returnRequestAction == null)
                return RedirectToAction("ReturnRequestActionList");

            if (ModelState.IsValid)
            {
                returnRequestAction = model.ToEntity(returnRequestAction);
                await _returnRequestService.UpdateReturnRequestAction(returnRequestAction);

                //locales
                await UpdateLocales(returnRequestAction, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestActions.Updated"));

                if (!continueEditing)
                    return RedirectToAction("ReturnRequestActionList");
                
                return RedirectToAction("ReturnRequestActionEdit", new { id = returnRequestAction.Id });
            }

            //prepare model
            model = await _returnRequestModelFactory.PrepareReturnRequestActionModel(model, returnRequestAction, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReturnRequestActionDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a return request action with the specified id
            var returnRequestAction = await _returnRequestService.GetReturnRequestActionById(id)
                ?? throw new ArgumentException("No return request action found with the specified id", nameof(id));

            await _returnRequestService.DeleteReturnRequestAction(returnRequestAction);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestActions.Deleted"));

            return RedirectToAction("ReturnRequestActionList");
        }

        #endregion

        #endregion
    }
}