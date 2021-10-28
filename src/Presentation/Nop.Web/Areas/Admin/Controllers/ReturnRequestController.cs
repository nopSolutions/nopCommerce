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

        protected ICustomerActivityService CustomerActivityService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedEntityService LocalizedEntityService { get; }
        protected INotificationService NotificationService { get; }
        protected IOrderService OrderService { get; }
        protected IPermissionService PermissionService { get; }
        protected IReturnRequestModelFactory ReturnRequestModelFactory { get; }
        protected IReturnRequestService ReturnRequestService { get; }
        protected IWorkflowMessageService WorkflowMessageService { get; }

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
            CustomerActivityService = customerActivityService;
            LocalizationService = localizationService;
            LocalizedEntityService = localizedEntityService;
            NotificationService = notificationService;
            OrderService = orderService;
            PermissionService = permissionService;
            ReturnRequestModelFactory = returnRequestModelFactory;
            ReturnRequestService = returnRequestService;
            WorkflowMessageService = workflowMessageService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(ReturnRequestReason rrr, ReturnRequestReasonModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(rrr,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocalesAsync(ReturnRequestAction rra, ReturnRequestActionModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(rra,
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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //prepare model
            var model = await ReturnRequestModelFactory.PrepareReturnRequestSearchModelAsync(new ReturnRequestSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(ReturnRequestSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageReturnRequests))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ReturnRequestModelFactory.PrepareReturnRequestListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a return request with the specified id
            var returnRequest = await ReturnRequestService.GetReturnRequestByIdAsync(id);
            if (returnRequest == null)
                return RedirectToAction("List");

            //prepare model
            var model = await ReturnRequestModelFactory.PrepareReturnRequestModelAsync(null, returnRequest);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(ReturnRequestModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a return request with the specified id
            var returnRequest = await ReturnRequestService.GetReturnRequestByIdAsync(model.Id);
            if (returnRequest == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                returnRequest = model.ToEntity(returnRequest);
                returnRequest.UpdatedOnUtc = DateTime.UtcNow;
                
                await ReturnRequestService.UpdateReturnRequestAsync(returnRequest);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditReturnRequest",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditReturnRequest"), returnRequest.Id), returnRequest);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ReturnRequests.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = returnRequest.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await ReturnRequestModelFactory.PrepareReturnRequestModelAsync(model, returnRequest, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("notify-customer")]
        public virtual async Task<IActionResult> NotifyCustomer(ReturnRequestModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a return request with the specified id
            var returnRequest = await ReturnRequestService.GetReturnRequestByIdAsync(model.Id);
            if (returnRequest == null)
                return RedirectToAction("List");

            var orderItem = await OrderService.GetOrderItemByIdAsync(returnRequest.OrderItemId);
            if (orderItem is null)
            {
                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.ReturnRequests.OrderItemDeleted"));
                return RedirectToAction("Edit", new { id = returnRequest.Id });
            }

            var order = await OrderService.GetOrderByIdAsync(orderItem.OrderId);

            var queuedEmailIds = await WorkflowMessageService.SendReturnRequestStatusChangedCustomerNotificationAsync(returnRequest, orderItem, order);
            if (queuedEmailIds.Any())
                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ReturnRequests.Notified"));

            return RedirectToAction("Edit", new { id = returnRequest.Id });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a return request with the specified id
            var returnRequest = await ReturnRequestService.GetReturnRequestByIdAsync(id);
            if (returnRequest == null)
                return RedirectToAction("List");

            await ReturnRequestService.DeleteReturnRequestAsync(returnRequest);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteReturnRequest",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteReturnRequest"), returnRequest.Id), returnRequest);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ReturnRequests.Deleted"));

            return RedirectToAction("List");
        }

        #region Return request reasons

        public virtual async Task<IActionResult> ReturnRequestReasonList()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //select an appropriate card
            SaveSelectedCardName("ordersettings-return-request");

            //we just redirect a user to the order settings page
            return RedirectToAction("Order", "Setting");
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReturnRequestReasonList(ReturnRequestReasonSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ReturnRequestModelFactory.PrepareReturnRequestReasonListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> ReturnRequestReasonCreate()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await ReturnRequestModelFactory.PrepareReturnRequestReasonModelAsync(new ReturnRequestReasonModel(), null);
            
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> ReturnRequestReasonCreate(ReturnRequestReasonModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var returnRequestReason = model.ToEntity<ReturnRequestReason>();
                await ReturnRequestService.InsertReturnRequestReasonAsync(returnRequestReason);

                //locales
                await UpdateLocalesAsync(returnRequestReason, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Settings.Order.ReturnRequestReasons.Added"));

                return continueEditing 
                    ? RedirectToAction("ReturnRequestReasonEdit", new { id = returnRequestReason.Id })
                    : RedirectToAction("ReturnRequestReasonList");
            }

            //prepare model
            model = await ReturnRequestModelFactory.PrepareReturnRequestReasonModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> ReturnRequestReasonEdit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a return request reason with the specified id
            var returnRequestReason = await ReturnRequestService.GetReturnRequestReasonByIdAsync(id);
            if (returnRequestReason == null)
                return RedirectToAction("ReturnRequestReasonList");

            //prepare model
            var model = await ReturnRequestModelFactory.PrepareReturnRequestReasonModelAsync(null, returnRequestReason);
            
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> ReturnRequestReasonEdit(ReturnRequestReasonModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a return request reason with the specified id
            var returnRequestReason = await ReturnRequestService.GetReturnRequestReasonByIdAsync(model.Id);
            if (returnRequestReason == null)
                return RedirectToAction("ReturnRequestReasonList");

            if (ModelState.IsValid)
            {
                returnRequestReason = model.ToEntity(returnRequestReason);
                await ReturnRequestService.UpdateReturnRequestReasonAsync(returnRequestReason);

                //locales
                await UpdateLocalesAsync(returnRequestReason, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Settings.Order.ReturnRequestReasons.Updated"));

                if (!continueEditing)
                    return RedirectToAction("ReturnRequestReasonList");
                
                return RedirectToAction("ReturnRequestReasonEdit", new { id = returnRequestReason.Id });
            }

            //prepare model
            model = await ReturnRequestModelFactory.PrepareReturnRequestReasonModelAsync(model, returnRequestReason, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReturnRequestReasonDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a return request reason with the specified id
            var returnRequestReason = await ReturnRequestService.GetReturnRequestReasonByIdAsync(id) 
                ?? throw new ArgumentException("No return request reason found with the specified id", nameof(id));

            await ReturnRequestService.DeleteReturnRequestReasonAsync(returnRequestReason);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Settings.Order.ReturnRequestReasons.Deleted"));

            return RedirectToAction("ReturnRequestReasonList");
        }

        #endregion

        #region Return request actions

        public virtual async Task<IActionResult> ReturnRequestActionList()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //select an appropriate card
            SaveSelectedCardName("ordersettings-return-request");

            //we just redirect a user to the order settings page
            return RedirectToAction("Order", "Setting");
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReturnRequestActionList(ReturnRequestActionSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ReturnRequestModelFactory.PrepareReturnRequestActionListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> ReturnRequestActionCreate()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await ReturnRequestModelFactory.PrepareReturnRequestActionModelAsync(new ReturnRequestActionModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> ReturnRequestActionCreate(ReturnRequestActionModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var returnRequestAction = model.ToEntity<ReturnRequestAction>();
                await ReturnRequestService.InsertReturnRequestActionAsync(returnRequestAction);

                //locales
                await UpdateLocalesAsync(returnRequestAction, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Settings.Order.ReturnRequestActions.Added"));

                return continueEditing 
                    ? RedirectToAction("ReturnRequestActionEdit", new { id = returnRequestAction.Id }) 
                    : RedirectToAction("ReturnRequestActionList");
            }

            //prepare model
            model = await ReturnRequestModelFactory.PrepareReturnRequestActionModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> ReturnRequestActionEdit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a return request action with the specified id
            var returnRequestAction = await ReturnRequestService.GetReturnRequestActionByIdAsync(id);
            if (returnRequestAction == null)
                return RedirectToAction("ReturnRequestActionList");

            //prepare model
            var model = await ReturnRequestModelFactory.PrepareReturnRequestActionModelAsync(null, returnRequestAction);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> ReturnRequestActionEdit(ReturnRequestActionModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a return request action with the specified id
            var returnRequestAction = await ReturnRequestService.GetReturnRequestActionByIdAsync(model.Id);
            if (returnRequestAction == null)
                return RedirectToAction("ReturnRequestActionList");

            if (ModelState.IsValid)
            {
                returnRequestAction = model.ToEntity(returnRequestAction);
                await ReturnRequestService.UpdateReturnRequestActionAsync(returnRequestAction);

                //locales
                await UpdateLocalesAsync(returnRequestAction, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Settings.Order.ReturnRequestActions.Updated"));

                if (!continueEditing)
                    return RedirectToAction("ReturnRequestActionList");
                
                return RedirectToAction("ReturnRequestActionEdit", new { id = returnRequestAction.Id });
            }

            //prepare model
            model = await ReturnRequestModelFactory.PrepareReturnRequestActionModelAsync(model, returnRequestAction, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReturnRequestActionDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a return request action with the specified id
            var returnRequestAction = await ReturnRequestService.GetReturnRequestActionByIdAsync(id)
                ?? throw new ArgumentException("No return request action found with the specified id", nameof(id));

            await ReturnRequestService.DeleteReturnRequestActionAsync(returnRequestAction);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Settings.Order.ReturnRequestActions.Deleted"));

            return RedirectToAction("ReturnRequestActionList");
        }

        #endregion

        #endregion
    }
}