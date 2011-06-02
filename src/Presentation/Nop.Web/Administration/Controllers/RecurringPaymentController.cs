using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Admin.Models.Orders;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class RecurringPaymentController : BaseNopController
    {
        #region Fields

        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPaymentService _paymentService;

        #endregion Fields

        #region Constructors

        public RecurringPaymentController(IOrderService orderService,
            IOrderProcessingService orderProcessingService, ILocalizationService localizationService,
            IWorkContext workContext, IDateTimeHelper dateTimeHelper, IPaymentService paymentService)
        {
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._dateTimeHelper = dateTimeHelper;
            this._paymentService = paymentService;
        }

        #endregion Constructors

        #region Utilities

        [NonAction]
        private void PrepareRecurringPaymentModel(RecurringPaymentModel model, 
            RecurringPayment recurringPayment, bool includeHistory)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");
            
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
            model.PaymentType = _paymentService.GetRecurringPaymentType(recurringPayment.InitialOrder.PaymentMethodSystemName).GetLocalizedEnum(_localizationService, _workContext);
            model.CanCancelRecurringPayment = _orderProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment);
                    
            if (includeHistory)
                foreach (var rph in recurringPayment.RecurringPaymentHistory.OrderBy(x => x.CreatedOnUtc))
                {
                    var rphModel = new RecurringPaymentModel.RecurringPaymentHistoryModel();
                    PrepareRecurringPaymentHistoryModel(rphModel, rph);
                    model.History.Add(rphModel);
                }
        }

        [NonAction]
        private void PrepareRecurringPaymentHistoryModel(RecurringPaymentModel.RecurringPaymentHistoryModel model,
            RecurringPaymentHistory history)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (history == null)
                throw new ArgumentNullException("history");

            var order = _orderService.GetOrderById(history.OrderId);

            model.Id = history.Id;
            model.OrderId = history.OrderId;
            model.RecurringPaymentId = history.RecurringPaymentId;
            model.OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.PaymentStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(history.CreatedOnUtc, DateTimeKind.Utc);
        }

        #endregion

        #region Recurring payment

        //list
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var gridModel = new GridModel<RecurringPaymentModel>();
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var payments = _orderService.SearchRecurringPayments(0, 0, null, true);
            var gridModel = new GridModel<RecurringPaymentModel>
            {
                Data = payments.PagedForCommand(command).Select(x =>
                {
                    var m = new RecurringPaymentModel();
                    PrepareRecurringPaymentModel(m, x, false);
                    return m;
                }),
                Total = payments.Count,
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        //edit
        public ActionResult Edit(int id)
        {
            var payment = _orderService.GetRecurringPaymentById(id);
            if (payment == null || payment.Deleted)
                throw new ArgumentException("No recurring payment found with the specified id", "id");
            var model = new RecurringPaymentModel();
            PrepareRecurringPaymentModel(model, payment, true);
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(RecurringPaymentModel model, bool continueEditing)
        {
            var payment = _orderService.GetRecurringPaymentById(model.Id);
            if (payment == null || payment.Deleted)
                throw new ArgumentException("No recurring payment found with the specified id");

            payment.CycleLength = model.CycleLength;
            payment.CyclePeriodId = model.CyclePeriodId;
            payment.TotalCycles = model.TotalCycles;
            payment.IsActive = model.IsActive;
            _orderService.UpdateRecurringPayment(payment);

            return continueEditing ? RedirectToAction("Edit", payment.Id) : RedirectToAction("List");
        }

        //delete
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var payment = _orderService.GetRecurringPaymentById(id);
            _orderService.DeleteRecurringPayment(payment);
            return RedirectToAction("List");
        }

        #endregion

        #region History

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult HistoryList(int recurringPaymentId, GridCommand command)
        {
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
            var model = new GridModel<RecurringPaymentModel.RecurringPaymentHistoryModel>
            {
                Data = historyModel,
                Total = historyModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("processnextpayment")]
        public ActionResult ProcessNextPayment(int id)
        {
            var payment = _orderService.GetRecurringPaymentById(id);
            if (payment == null)
                throw new ArgumentException("No recurring payment found with the specified id");

            ViewData["selectedTab"] = "history";

            try
            {
                _orderProcessingService.ProcessNextRecurringPayment(payment);
                var model = new RecurringPaymentModel();
                PrepareRecurringPaymentModel(model, payment, true);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new RecurringPaymentModel();
                PrepareRecurringPaymentModel(model, payment, true);
                model.ProcessPaymentErrors.Add(exc.Message);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("cancelpayment")]
        public ActionResult CancelRecurringPayment(int id)
        {
            var payment = _orderService.GetRecurringPaymentById(id);
            if (payment == null)
                throw new ArgumentException("No recurring payment found with the specified id");

            ViewData["selectedTab"] = "history";

            try
            {
                var errors = _orderProcessingService.CancelRecurringPayment(payment);
                var model = new RecurringPaymentModel();
                PrepareRecurringPaymentModel(model, payment, true);
                model.ProcessPaymentErrors = errors.ToList();
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new RecurringPaymentModel();
                PrepareRecurringPaymentModel(model, payment, true);
                model.ProcessPaymentErrors.Add(exc.Message);
                return View(model);
            }
        }

        #endregion
    }
}
