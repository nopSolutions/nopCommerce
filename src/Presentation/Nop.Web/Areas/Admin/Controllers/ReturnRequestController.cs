using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ReturnRequestController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly IReturnRequestModelFactory _returnRequestModelFactory;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IWorkflowMessageService _workflowMessageService;

        #endregion Fields

        #region Ctor

        public ReturnRequestController(ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IOrderService orderService,
            IPermissionService permissionService,
            IReturnRequestModelFactory returnRequestModelFactory,
            IReturnRequestService returnRequestService,
            IWorkflowMessageService workflowMessageService)
        {
            this._customerActivityService = customerActivityService;
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._orderService = orderService;
            this._permissionService = permissionService;
            this._returnRequestModelFactory = returnRequestModelFactory;
            this._returnRequestService = returnRequestService;
            this._workflowMessageService = workflowMessageService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //prepare model
            var model = _returnRequestModelFactory.PrepareReturnRequestSearchModel(new ReturnRequestSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(ReturnRequestSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _returnRequestModelFactory.PrepareReturnRequestListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a return request with the specified id
            var returnRequest = _returnRequestService.GetReturnRequestById(id);
            if (returnRequest == null)
                return RedirectToAction("List");

            //prepare model
            var model = _returnRequestModelFactory.PrepareReturnRequestModel(null, returnRequest);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(ReturnRequestModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a return request with the specified id
            var returnRequest = _returnRequestService.GetReturnRequestById(model.Id);
            if (returnRequest == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                returnRequest.Quantity = model.Quantity;
                returnRequest.ReasonForReturn = model.ReasonForReturn;
                returnRequest.RequestedAction = model.RequestedAction;
                returnRequest.CustomerComments = model.CustomerComments;
                returnRequest.StaffNotes = model.StaffNotes;
                returnRequest.ReturnRequestStatusId = model.ReturnRequestStatusId;
                returnRequest.UpdatedOnUtc = DateTime.UtcNow;
                _customerService.UpdateCustomer(returnRequest.Customer);

                //activity log
                _customerActivityService.InsertActivity("EditReturnRequest",
                    string.Format(_localizationService.GetResource("ActivityLog.EditReturnRequest"), returnRequest.Id), returnRequest);

                SuccessNotification(_localizationService.GetResource("Admin.ReturnRequests.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = returnRequest.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model = _returnRequestModelFactory.PrepareReturnRequestModel(model, returnRequest, true);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("notify-customer")]
        public virtual IActionResult NotifyCustomer(ReturnRequestModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a return request with the specified id
            var returnRequest = _returnRequestService.GetReturnRequestById(model.Id);
            if (returnRequest == null)
                return RedirectToAction("List");

            var orderItem = _orderService.GetOrderItemById(returnRequest.OrderItemId);
            if (orderItem == null)
            {
                ErrorNotification(_localizationService.GetResource("Admin.ReturnRequests.OrderItemDeleted"));
                return RedirectToAction("Edit", new { id = returnRequest.Id });
            }

            var queuedEmailIds = _workflowMessageService.SendReturnRequestStatusChangedCustomerNotification(returnRequest, orderItem, orderItem.Order.CustomerLanguageId);
            if (queuedEmailIds.Any())
                SuccessNotification(_localizationService.GetResource("Admin.ReturnRequests.Notified"));

            return RedirectToAction("Edit", new { id = returnRequest.Id });
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a return request with the specified id
            var returnRequest = _returnRequestService.GetReturnRequestById(id);
            if (returnRequest == null)
                return RedirectToAction("List");

            _returnRequestService.DeleteReturnRequest(returnRequest);

            //activity log
            _customerActivityService.InsertActivity("DeleteReturnRequest",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteReturnRequest"), returnRequest.Id), returnRequest);

            SuccessNotification(_localizationService.GetResource("Admin.ReturnRequests.Deleted"));

            return RedirectToAction("List");
        }

        #endregion
    }
}