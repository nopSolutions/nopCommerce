using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ReturnRequestController : BaseAdminController
    {
        #region Fields

        private readonly IReturnRequestService _returnRequestService;
        private readonly IOrderService _orderService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;

        #endregion Fields

        #region Ctor

        public ReturnRequestController(IReturnRequestService returnRequestService,
            IOrderService orderService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            ICustomerActivityService customerActivityService, 
            IPermissionService permissionService,
            IDownloadService downloadService)
        {
            this._returnRequestService = returnRequestService;
            this._orderService = orderService;
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._downloadService = downloadService;
        }

        #endregion

        #region Utilities

        protected virtual void PrepareReturnRequestModel(ReturnRequestModel model,
            ReturnRequest returnRequest, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (returnRequest == null)
                throw new ArgumentNullException(nameof(returnRequest));

            var orderItem = _orderService.GetOrderItemById(returnRequest.OrderItemId);
            if (orderItem != null)
            {
                model.ProductId = orderItem.ProductId;
                model.ProductName = orderItem.Product.Name;
                model.OrderId = orderItem.OrderId;
                model.AttributeInfo = orderItem.AttributeDescription;
                model.CustomOrderNumber = orderItem.Order.CustomOrderNumber;
            }
            model.Id = returnRequest.Id;
            model.CustomNumber = returnRequest.CustomNumber;
            model.CustomerId = returnRequest.CustomerId;
            var customer = returnRequest.Customer;
            model.CustomerInfo = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
            model.Quantity = returnRequest.Quantity;
            model.ReturnRequestStatusStr = returnRequest.ReturnRequestStatus.GetLocalizedEnum(_localizationService, _workContext);

            var download = _downloadService.GetDownloadById(returnRequest.UploadedFileId);
            model.UploadedFileGuid = download != null ? download.DownloadGuid : Guid.Empty;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(returnRequest.CreatedOnUtc, DateTimeKind.Utc);
            if (!excludeProperties)
            {
                model.ReasonForReturn = returnRequest.ReasonForReturn;
                model.RequestedAction = returnRequest.RequestedAction;
                model.CustomerComments = returnRequest.CustomerComments;
                model.StaffNotes = returnRequest.StaffNotes;
                model.ReturnRequestStatusId = returnRequest.ReturnRequestStatusId;
            }
        }

        #endregion

        #region Methods

        //list
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var model = new ReturnRequestListModel
            {
                ReturnRequestStatusList = ReturnRequestStatus.Cancelled.ToSelectList(false).ToList(),
                ReturnRequestStatusId = -1
            };

            model.ReturnRequestStatusList.Insert(0, new SelectListItem
            {
                Value = "-1",
                Text = _localizationService.GetResource("Admin.ReturnRequests.SearchReturnRequestStatus.All"),
                Selected = true
            });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command, ReturnRequestListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedKendoGridJson();

            var rrs = model.ReturnRequestStatusId == -1 ? null : (ReturnRequestStatus?) model.ReturnRequestStatusId;
            
            var startDateValue = model.StartDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = model.EndDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var returnRequests = _returnRequestService.SearchReturnRequests(0, 0, 0, model.CustomNumber, rrs, startDateValue, endDateValue, command.Page - 1, command.PageSize);
            var returnRequestModels = new List<ReturnRequestModel>();
            foreach (var rr in returnRequests)
            {
                var m = new ReturnRequestModel();
                PrepareReturnRequestModel(m, rr, false);
                returnRequestModels.Add(m);
            }
            var gridModel = new DataSourceResult
            {
                Data = returnRequestModels,
                Total = returnRequests.TotalCount,
            };

            return Json(gridModel);
        }

        //edit
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var returnRequest = _returnRequestService.GetReturnRequestById(id);
            if (returnRequest == null)
                //No return request found with the specified id
                return RedirectToAction("List");
            
            var model = new ReturnRequestModel();
            PrepareReturnRequestModel(model, returnRequest, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(ReturnRequestModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var returnRequest = _returnRequestService.GetReturnRequestById(model.Id);
            if (returnRequest == null)
                //No return request found with the specified id
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
                _customerActivityService.InsertActivity("EditReturnRequest", _localizationService.GetResource("ActivityLog.EditReturnRequest"), returnRequest.Id);

                SuccessNotification(_localizationService.GetResource("Admin.ReturnRequests.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = returnRequest.Id}) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareReturnRequestModel(model, returnRequest, true);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("notify-customer")]
        public virtual IActionResult NotifyCustomer(ReturnRequestModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var returnRequest = _returnRequestService.GetReturnRequestById(model.Id);
            if (returnRequest == null)
                //No return request found with the specified id
                return RedirectToAction("List");

            var orderItem = _orderService.GetOrderItemById(returnRequest.OrderItemId);
            if (orderItem == null)
            {
                ErrorNotification(_localizationService.GetResource("Admin.ReturnRequests.OrderItemDeleted"));
                return RedirectToAction("Edit", new { id = returnRequest.Id });
            }
            
            var queuedEmailId = _workflowMessageService.SendReturnRequestStatusChangedCustomerNotification(returnRequest, orderItem, orderItem.Order.CustomerLanguageId);
            if (queuedEmailId > 0)
                SuccessNotification(_localizationService.GetResource("Admin.ReturnRequests.Notified"));
            return RedirectToAction("Edit",  new {id = returnRequest.Id});
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var returnRequest = _returnRequestService.GetReturnRequestById(id);
            if (returnRequest == null)
                //No return request found with the specified id
                return RedirectToAction("List");

            _returnRequestService.DeleteReturnRequest(returnRequest);

            //activity log
            _customerActivityService.InsertActivity("DeleteReturnRequest", _localizationService.GetResource("ActivityLog.DeleteReturnRequest"), returnRequest.Id);

            SuccessNotification(_localizationService.GetResource("Admin.ReturnRequests.Deleted"));
            return RedirectToAction("List");
        }

        #endregion
    }
}