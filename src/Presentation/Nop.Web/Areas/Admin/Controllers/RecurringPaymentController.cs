using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class RecurringPaymentController : BaseAdminController
    {
        #region Fields

        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPaymentService _paymentService;
        private readonly IPermissionService _permissionService;

        #endregion Fields

        #region Ctor

        public RecurringPaymentController(IOrderService orderService,
            IOrderProcessingService orderProcessingService, ILocalizationService localizationService,
            IWorkContext workContext, IDateTimeHelper dateTimeHelper, IPaymentService paymentService,
            IPermissionService permissionService)
        {
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._dateTimeHelper = dateTimeHelper;
            this._paymentService = paymentService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Utilities

        protected virtual void PrepareRecurringPaymentModel(RecurringPaymentModel model, 
            RecurringPayment recurringPayment)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));
            
            model.Id = recurringPayment.Id;
            model.CycleLength = recurringPayment.CycleLength;
            model.CyclePeriodId = recurringPayment.CyclePeriodId;
            model.CyclePeriodStr = recurringPayment.CyclePeriod.GetLocalizedEnum(_localizationService, _workContext);
            model.TotalCycles = recurringPayment.TotalCycles;
            model.StartDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString();
            model.IsActive = recurringPayment.IsActive;
            model.NextPaymentDate = recurringPayment.NextPaymentDate.HasValue ? _dateTimeHelper.ConvertToUserTime(recurringPayment.NextPaymentDate.Value, DateTimeKind.Utc).ToString() : "";
            model.CyclesRemaining = recurringPayment.CyclesRemaining;
            model.InitialOrderId = recurringPayment.InitialOrder.Id;
            var customer = recurringPayment.InitialOrder.Customer;
            model.CustomerId = customer.Id;
            model.CustomerEmail = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
            model.PaymentType = _paymentService.GetRecurringPaymentType(recurringPayment.InitialOrder.PaymentMethodSystemName).GetLocalizedEnum(_localizationService, _workContext);
            model.CanCancelRecurringPayment = _orderProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment);
            model.LastPaymentFailed = recurringPayment.LastPaymentFailed;
        }

        protected virtual void PrepareRecurringPaymentHistoryModel(RecurringPaymentModel.RecurringPaymentHistoryModel model,
            RecurringPaymentHistory history)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (history == null)
                throw new ArgumentNullException(nameof(history));

            var order = _orderService.GetOrderById(history.OrderId);

            model.Id = history.Id;
            model.OrderId = history.OrderId;
            model.RecurringPaymentId = history.RecurringPaymentId;
            model.OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.PaymentStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(history.CreatedOnUtc, DateTimeKind.Utc);
            model.CustomOrderNumber = order.CustomOrderNumber;
        }

        #endregion

        #region Recurring payment

        //list
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedKendoGridJson();

            var payments = _orderService.SearchRecurringPayments(0, 0, 0, null, command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = payments.Select(x =>
                {
                    var m = new RecurringPaymentModel();
                    PrepareRecurringPaymentModel(m, x);
                    return m;
                }),
                Total = payments.TotalCount,
            };

            return Json(gridModel);
        }

        //edit
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            var payment = _orderService.GetRecurringPaymentById(id);
            if (payment == null || payment.Deleted)
                //No recurring payment found with the specified id
                return RedirectToAction("List");

            var model = new RecurringPaymentModel();
            PrepareRecurringPaymentModel(model, payment);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(RecurringPaymentModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            var payment = _orderService.GetRecurringPaymentById(model.Id);
            if (payment == null || payment.Deleted)
                //No recurring payment found with the specified id
                return RedirectToAction("List");

            payment.CycleLength = model.CycleLength;
            payment.CyclePeriodId = model.CyclePeriodId;
            payment.TotalCycles = model.TotalCycles;
            payment.IsActive = model.IsActive;
            _orderService.UpdateRecurringPayment(payment);

            SuccessNotification(_localizationService.GetResource("Admin.RecurringPayments.Updated"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit",  new {id = payment.Id});
            }
            return RedirectToAction("List");
        }

        //delete
        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            var payment = _orderService.GetRecurringPaymentById(id);
            if (payment == null)
                //No recurring payment found with the specified id
                return RedirectToAction("List");

            _orderService.DeleteRecurringPayment(payment);

            SuccessNotification(_localizationService.GetResource("Admin.RecurringPayments.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region History

        [HttpPost]
        public virtual IActionResult HistoryList(int recurringPaymentId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedKendoGridJson();

            var payment = _orderService.GetRecurringPaymentById(recurringPaymentId);
            if (payment == null)
                throw new ArgumentException("No recurring payment found with the specified id");

            var historyModel = payment.RecurringPaymentHistory.OrderBy(x => x.CreatedOnUtc)
                .Select(x =>
                {
                    var m = new RecurringPaymentModel.RecurringPaymentHistoryModel();
                    PrepareRecurringPaymentHistoryModel(m, x);
                    return m;
                })
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = historyModel,
                Total = historyModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("processnextpayment")]
        public virtual IActionResult ProcessNextPayment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            var payment = _orderService.GetRecurringPaymentById(id);
            if (payment == null)
                //No recurring payment found with the specified id
                return RedirectToAction("List");
            
            try
            {
                var errors = _orderProcessingService.ProcessNextRecurringPayment(payment);

                var model = new RecurringPaymentModel();
                PrepareRecurringPaymentModel(model, payment);

                if (errors.Any())
                    errors.ToList().ForEach(error => ErrorNotification(error, false));
                else
                    SuccessNotification(_localizationService.GetResource("Admin.RecurringPayments.NextPaymentProcessed"), false);

                //selected tab
                SaveSelectedTabName(persistForTheNextRequest: false);

                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new RecurringPaymentModel();
                PrepareRecurringPaymentModel(model, payment);
                ErrorNotification(exc, false);

                //selected tab
                SaveSelectedTabName(persistForTheNextRequest: false);

                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("cancelpayment")]
        public virtual IActionResult CancelRecurringPayment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageRecurringPayments))
                return AccessDeniedView();

            var payment = _orderService.GetRecurringPaymentById(id);
            if (payment == null)
                //No recurring payment found with the specified id
                return RedirectToAction("List");

            try
            {
                var errors = _orderProcessingService.CancelRecurringPayment(payment);
                var model = new RecurringPaymentModel();
                PrepareRecurringPaymentModel(model, payment);
                if (errors.Any())
                {
                    foreach (var error in errors)
                        ErrorNotification(error, false);
                }
                else
                    SuccessNotification(_localizationService.GetResource("Admin.RecurringPayments.Cancelled"), false);

                //selected tab
                SaveSelectedTabName(persistForTheNextRequest: false);

                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new RecurringPaymentModel();
                PrepareRecurringPaymentModel(model, payment);
                ErrorNotification(exc, false);

                //selected tab
                SaveSelectedTabName(persistForTheNextRequest: false);

                return View(model);
            }
        }

        #endregion
    }
}